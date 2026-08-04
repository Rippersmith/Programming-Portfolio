using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Slider sideLengthSlider, winLengthSlider;
    [SerializeField] TMP_Text sideLengthNum, winLengthNum;

    GameManager gm;

    void Start()
    {
        gm = GameManager.instance;

        UpdateSlidersToGMValues();

        UpdateSideLengthNum(gm.matrixSideLength);
        UpdateWinLengthNum(gm.winningLineLength);
    }

    public void StartNewGame()
    {
        gm.matrixSideLength = (int)sideLengthSlider.value;
        gm.winningLineLength = (int)winLengthSlider.value;

        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //We CANNOT have the winning length be greater than the actual matrix side length.
    //this code will help adjust the winning length maximum to be the matrix side length
    public void UpdateWinLengthSliderMax(float newMaxValue)
    {
        //Sliders do not allow min length & max length to be the same - 
        //"+ 0.001f" helps solve this issue without being especially noticable
        winLengthSlider.maxValue = newMaxValue + 0.001f;
    }

    public void UpdateSideLengthNum(float newNum)
    {
        sideLengthNum.text = newNum.ToString();
    }

    public void UpdateWinLengthNum(float newNum)
    {
        winLengthNum.text = newNum.ToString();
    }

    public void UpdateSlidersToGMValues()
    {
        sideLengthSlider.value = gm.matrixSideLength;
        winLengthSlider.value = gm.winningLineLength;
        UpdateWinLengthSliderMax(gm.matrixSideLength);
    }

}
