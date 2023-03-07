using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public Text gloCountText;

    public int gloCount = 5;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        gloCountText.text = "Glowsticks: " + gloCount;
    }

    public void UseGlowstick()
    {
        gloCount--;
    }
}
