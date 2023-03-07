using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimerUI : MonoBehaviour
{
    public float timer = 20f;

    float currMaxTime = 0;
    public float playerMaxTime = 10f;
    public float enemyMaxTime = 5f;

    public Canvas timerCircleCanvas;

    CombatManager cm;

    Image playerRadial;
    Image enemyRadial;
    Image currRadial;
    Text timeLeft;

    //will always start on player's turn, +5 secs
     bool isPlayerTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        playerRadial = timerCircleCanvas.transform.GetChild(1).GetComponent<Image>();
        enemyRadial = timerCircleCanvas.transform.GetChild(0).GetComponent<Image>();
        timeLeft = GetComponentInChildren<Text>();

        currRadial = playerRadial;
        currMaxTime = timer;

        cm = CombatManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        currRadial.fillAmount = (timer / currMaxTime);
        timeLeft.text = timer.ToString("0.0");

        if (timer <= 0f)
        {
            currRadial.fillAmount = 1;
            if (isPlayerTurn) //End Player's Turn
            {
                enemyRadial.transform.SetSiblingIndex(1);
                currRadial = enemyRadial;
                currMaxTime = enemyMaxTime;
            }
            else if (!isPlayerTurn) //End Enemy's Turn
            {
                playerRadial.transform.SetSiblingIndex(1);
                currRadial = playerRadial;
                currMaxTime = playerMaxTime;
            }

            isPlayerTurn = !isPlayerTurn;
            cm.ChangeTurns();
            timer = currMaxTime;
        }

    }
}
