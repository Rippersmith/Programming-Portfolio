using UnityEngine;

//this struct will have 2 materials - a normal material, and a "transparent" material
//The cube will change to it's transparent material so that the player can look inside the matrix,
//and change back when we aren't
struct PlayerMaterialStruct
{
    Material normPlayerMat;
    Material transPlayerMat;

    public PlayerMaterialStruct(Material newNormPlayerMat, Material newTransPlayerMat)
    {
        this.normPlayerMat = newNormPlayerMat;
        this.transPlayerMat = newTransPlayerMat;
    }

    public Material GetMaterial(bool isTransparent)
    {
        if (isTransparent == true)
            return transPlayerMat;
        return normPlayerMat;
    }
}

public class MatrixCubeScript : MonoBehaviour
{
    [SerializeField] public Vector3 cubeLocValue;

    [SerializeField] GameObject newCubeObject;

    [SerializeField] LayerMask cubeLayer, transCubeLayer;
    int cubeLayerInt, transCubeLayerInt;

    [SerializeField] Renderer cubeMesh;
    [SerializeField] Material neutralMat, neutralTransMat, hiliteMat;
    [SerializeField] Material xPlayerMat, oPlayerMat, xPlayerTransMat, oPlayerTransMat;

    public PlayerValue claimedPlayerValue = PlayerValue.NULL;

    PlayerMaterialStruct neutralMats, playerXMats, playerOMats;
    PlayerMaterialStruct currentCubeMats;
    bool isTransparent = false;

    void Start()
    {
        //this cube renaming doesn't change anything gamewise, but it is very helpful for bugtesting
        gameObject.name = "Cube - " + transform.localPosition.ToString("F0");

        cubeLayerInt = Mathf.RoundToInt(Mathf.Log(cubeLayer, 2));
        transCubeLayerInt = Mathf.RoundToInt(Mathf.Log(transCubeLayer, 2));

        cubeLocValue = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

        //assign the new PlayerMaterialStructs here instead of manually assigning them beforehand
        neutralMats = new PlayerMaterialStruct(neutralMat, neutralTransMat);
        playerXMats = new PlayerMaterialStruct(xPlayerMat, xPlayerTransMat);
        playerOMats = new PlayerMaterialStruct(oPlayerMat, oPlayerTransMat);

        //when a cube is first created, they will all be "Neutral" (not claimed by either player)
        currentCubeMats = neutralMats;

        MainGameplayManager.instance.AddNewCubeCount(this.transform);
    }

    //If we haven't reached the edges of the matrix, then spawn an adjacent cube
    //above, in front, and to the right of the current cube
    public void SpawnAdjacentCubes(Vector3 eachSideLength)
    {
        if (eachSideLength.x > 0)
            CheckIfSpawnPointIsEmpty(Vector3.right, eachSideLength);
        if (eachSideLength.y > 0)
            CheckIfSpawnPointIsEmpty(Vector3.up, eachSideLength);
        if (eachSideLength.z > 0)
            CheckIfSpawnPointIsEmpty(Vector3.forward, eachSideLength);
    }

    //Check if the location is free (not occupied by other cubes) - if it's available,
    //spawn a new cube & make it continue to spawn more cubes from its position
    void CheckIfSpawnPointIsEmpty(Vector3 direction, Vector3 remainingSpawnDirs)
    {
        if (!(Physics.CheckSphere((transform.position + direction), 0.45f, cubeLayer)))
        {
            GameObject newCube = Instantiate(newCubeObject, transform.position + direction, Quaternion.identity);
            MatrixCubeScript newCubeScript = newCube.GetComponent<MatrixCubeScript>();
            newCubeScript.SpawnAdjacentCubes(remainingSpawnDirs - direction);
        }
    }

    //When a cube is "claimed" by a player, we have to change the cube material,
    //& the player that claimed the cube
    public void SetNewPlayerValue(PlayerValue newCubeValue)
    {
        switch (newCubeValue)
        {
            case PlayerValue.X: 
                cubeMesh.material = playerXMats.GetMaterial(false);
                claimedPlayerValue = PlayerValue.X;
                currentCubeMats = playerXMats;
                break;
            case PlayerValue.O:
                cubeMesh.material = playerOMats.GetMaterial(false);
                claimedPlayerValue = PlayerValue.O;
                currentCubeMats = playerOMats;
                break;
            case PlayerValue.NULL:
            default:
                Debug.LogError("Error! Tried Assigning Invalid Value to " + gameObject.name + ".");
                break;

        }
    }

    public void ChangeCubeTransparency(bool isCubeTransparent)
    {
        isTransparent = isCubeTransparent;
        cubeMesh.material = currentCubeMats.GetMaterial(isTransparent);
        gameObject.layer = isTransparent ? transCubeLayerInt : cubeLayerInt;
    }

    public void ChangeToHilitedMaterial(bool isCubeHilited)
    {
        if (isCubeHilited && !isTransparent)
        {
            if (claimedPlayerValue == PlayerValue.NULL)
                cubeMesh.material = hiliteMat;
        }
        else
        {
            cubeMesh.material = currentCubeMats.GetMaterial(isTransparent);
        }
    }
}
