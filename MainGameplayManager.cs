using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum PlayerValue { NULL, X, O };

[System.Serializable]
public class WinningLineCombination
{
    public Vector3[] winningLine;
    public Dictionary<Vector3, PlayerValue> winningPointsOnLine;

    public WinningLineCombination(List<Vector3> newWinningLine)
    {
        winningLine = newWinningLine.ToArray();
        winningPointsOnLine = new Dictionary<Vector3, PlayerValue>();

        //when we first create the line, fill out each spot on the line with a NULL value -
        //we'll fill these in with player X or O later
        for (int i = 0; i < winningLine.Length; i++)
        {
            winningPointsOnLine.Add(newWinningLine[i], PlayerValue.NULL);
        }
    }

    //if the "newPoint" is in the WinningLineCOmbination, then change that point's value to the newPlayerValue - 
    //this will happen when a player "claims" a cube
    public void AssignNewCubeValueToPointIfAvailable(Vector3 newPoint, PlayerValue newPlayerValue)
    {
        if (winningPointsOnLine.ContainsKey(newPoint) && winningPointsOnLine[newPoint] == PlayerValue.NULL)
        {
            winningPointsOnLine[newPoint] = newPlayerValue;
        }
    }

    //if all the values on a line are the same and not NULL, then that means a player has completed a whole line!
    public bool IsLineAllSameValue()
    {
        for (int i = 0; i < winningPointsOnLine.Count - 1; i++)
        {
            if (winningPointsOnLine[winningLine[i]] == PlayerValue.NULL ||
                winningPointsOnLine[winningLine[i]] != winningPointsOnLine[winningLine[i + 1]])
            {
                return false;
            }
        }
        //Debug.Log("PLAYER WINS!!!");
        return true;
    }
}

public class MainGameplayManager : MonoBehaviour
{
    public static MainGameplayManager instance;

    GameManager gm;

    public Transform cubeParentHolder;

    [SerializeField] int matrixSideLength = 3, lineLengthToWin = 3;
    [SerializeField] GameObject newCubeObject;
    [SerializeField] List<WinningLineCombination> winningLineCombinations = new List<WinningLineCombination>();

    int cubeCount = 0, maxCubeCount;

    public PlayerValue currPlayerTurn;

    [SerializeField] MatrixCubeScript[] allCubeScripts;

    [SerializeField] PlayerMessenger playerMessenger;

    //all of these "searchDirections" are the directions a cube will search for a full winning line -
    //don't need to do every possible direction, otherwise we will get much more repeats
    Vector3[] searchDirections = new Vector3[16] {  Vector3.up, /*Vector3.down,*/ Vector3.left, Vector3.right,Vector3.forward,
                                                    Vector3.up + Vector3.left, Vector3.up + Vector3.right, Vector3.down + Vector3.left, Vector3.down + Vector3.right,
                                                    Vector3.forward + Vector3.left, Vector3.forward + Vector3.right, Vector3.forward + Vector3.up, Vector3.forward + Vector3.down,
                                                    Vector3.forward + Vector3.up + Vector3.left, Vector3.forward + Vector3.up + Vector3.right,
                                                    Vector3.forward + Vector3.down + Vector3.left, Vector3.forward + Vector3.down + Vector3.right,
                                                    };

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    void Start()
    {
        gm = GameManager.instance;

        matrixSideLength = GameManager.instance.matrixSideLength;
        lineLengthToWin = GameManager.instance.winningLineLength;

        //when we spawn all the cubes in the matrix, we literally count them out
        maxCubeCount = Mathf.RoundToInt(Mathf.Pow(matrixSideLength, 3));
        //"X" will always be the starting player
        currPlayerTurn = PlayerValue.X;

        GameObject startingCube = Instantiate(newCubeObject, Vector3.zero, Quaternion.identity);
        MatrixCubeScript startingCubeScript = startingCube.GetComponent<MatrixCubeScript>();
        startingCubeScript.SpawnAdjacentCubes(Vector3.one * (matrixSideLength - 1));
    }

    public void AddNewCubeCount(Transform newCubeTransform)
    {
        newCubeTransform.parent = cubeParentHolder;
        cubeCount++;

        //this part will run once all of the cubes have spawned
        if (cubeCount == maxCubeCount)
        {
            allCubeScripts = FindObjectsByType<MatrixCubeScript>(FindObjectsSortMode.None);

            //once all the cubes are spawned, reposition the whole matrix & the camera, and find all of the winning line combinations in the game
            cubeParentHolder.position -= Vector3.one * ((matrixSideLength - 1) / 2f);
            Camera.main.transform.position = Vector3.forward * -(matrixSideLength * 2f);
            winningLineCombinations = FindAllWinningLineCombinations();

            playerMessenger.ShowPlayerTurnMessage(PlayerNameBasedOnValue(currPlayerTurn));
        }
    }

    //this code runs when a player "claims" a cube on the matrix - it determines if the player has successfully
    //filled out a whole line. If so, go to the "Vectory Sequence." If not, continue to the next player's turn
    public void PlayerClaimsNewCube(Vector3 cubeValue, PlayerValue playerValue)
    {
        bool continueGame = true;

        for (int lc = 0; lc < winningLineCombinations.Count; lc++)
        {
            //this goes through each winning combination, and if the player's newly claimed cube is in that combination,
            //mark that cube as the player's
            winningLineCombinations[lc].AssignNewCubeValueToPointIfAvailable(cubeValue, playerValue);

            //this checks if every value in the winning combination line is the same playerValue. If so, that player is the winner
            if (winningLineCombinations[lc].IsLineAllSameValue())
            {
                //victory sequence
                VictorySequence(playerValue);
                continueGame = false;
                break;
            }
        }

        if (continueGame)
        {
            currPlayerTurn = GetOtherPlayer(currPlayerTurn);
            playerMessenger.ShowPlayerTurnMessage(PlayerNameBasedOnValue(currPlayerTurn));
        }
    }

    //this code will change the material on all of the cubes, based on which face the player is looking at
    //and how "deep" in the matrix the player is looking (based on the "mouseScrollValue")
    public void ChangeAllCubeMaterials(MatrixFaceDirection currentMatrixFace, int mouseScrollValue)
    {
        MouseController.instance.ResetHilitedCube();

        //"vectorToMeasure" corresponds to the Vector3 value of the cube - Vector3.x = 0, Vector3.y = 1, Vector3.z = 2
        int vectorToMeasure = (int)currentMatrixFace.faceDirValue % 10;
        int valueToCompare = (int)currentMatrixFace.direction[vectorToMeasure] * matrixSideLength;

        //subtract mouseScrollValue by 1, otherwise it can get kind of buggy
        mouseScrollValue--;
        mouseScrollValue = (int)currentMatrixFace.faceDirValue >= 10 ? -mouseScrollValue : mouseScrollValue;

        //we have to use different comparisions based on which side of the matrix we are looking at
        if (valueToCompare > 0)
        {
            for (int i = 0; i < allCubeScripts.Length; i++)
            {
                if (allCubeScripts[i].cubeLocValue[vectorToMeasure] < valueToCompare + mouseScrollValue - 1)
                    allCubeScripts[i].ChangeCubeTransparency(false);
                else
                    allCubeScripts[i].ChangeCubeTransparency(true);
            }
        }
        else
        {
            for (int i = 0; i < allCubeScripts.Length; i++)
            {
                if (allCubeScripts[i].cubeLocValue[vectorToMeasure] > valueToCompare + mouseScrollValue)
                    allCubeScripts[i].ChangeCubeTransparency(false);
                else
                    allCubeScripts[i].ChangeCubeTransparency(true);
            }
        }
    }


    
    //here. we will find all of the possible winning line combinations
    //in the whole matrix
    List<WinningLineCombination> FindAllWinningLineCombinations()
    {
        List<List<Vector3>> newWinningLinesList = new List<List<Vector3>>();
        List<WinningLineCombination> allWinningLineCombinations = new List<WinningLineCombination>();

        for (int x = 0; x < matrixSideLength; x++)
        {
            for (int y = 0; y < matrixSideLength; y++)
            {
                for (int z = 0; z < matrixSideLength; z++)
                {
                    newWinningLinesList.AddRange(CreateNewWinningCombinationsFromStartingPoint(new Vector3(x, y, z)));
                }
            }
        }

        newWinningLinesList = newWinningLinesList.Distinct().ToList();

        for (int i = 0;  i < newWinningLinesList.Count; i++)
        {
            allWinningLineCombinations.Add(new WinningLineCombination(newWinningLinesList[i]));
        }

        return allWinningLineCombinations;
    }   

    //This code will find all winning lines that has the cube at "cubeStartingPoint" at one end
    //of the line.
    //don't have to bother on 1 whole axis - just go down 2 and they will both measure the other naturally
    List<List<Vector3>> CreateNewWinningCombinationsFromStartingPoint(Vector3 cubeStartingPoint)
    {     
        List<List<Vector3>> newWinningLines = new List<List<Vector3>>();

        foreach (Vector3 direction in searchDirections)
        {
            Vector3 directionModifier = direction * (lineLengthToWin - 1);
            Vector3 newFullLineLength = cubeStartingPoint + directionModifier;

            if ((newFullLineLength.x >= 0 && newFullLineLength.x < matrixSideLength) &&
                (newFullLineLength.y >= 0 && newFullLineLength.y < matrixSideLength) &&
                (newFullLineLength.z >= 0 && newFullLineLength.z < matrixSideLength))
            {
                List<Vector3> newWinningLinePoints = new List<Vector3>();
                for (int a = 0; a < lineLengthToWin; a++)
                {
                    newWinningLinePoints.Add(cubeStartingPoint + (direction * a));
                }

                newWinningLines.Add(newWinningLinePoints);
            }
        }

        return newWinningLines;
    }

    string PlayerNameBasedOnValue(PlayerValue playerValue)
    {
        return playerValue == PlayerValue.X ? gm.player1Name : gm.player2Name;
    }

    PlayerValue GetOtherPlayer(PlayerValue playerValue)
    {
        return playerValue == PlayerValue.X ? PlayerValue.O : PlayerValue.X;
    }

    void VictorySequence(PlayerValue winningPlayer)
    {
        playerMessenger.ShowVictoryMessage(PlayerNameBasedOnValue(winningPlayer));
    }

}
