using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCamera : MonoBehaviour {
    public float speed = 3f;
    public bool permissionToMove = true, playerDead = false;
    public Text score;

    float distanceTillNextBoost = 15f;
    float lastDistance = 15f;   
    float roundScore;

    StoreScore storeScore;

    private void Awake()
    {
        storeScore = GameObject.Find("StoreScore").GetComponent<StoreScore>();
    }

    // Update is called once per frame
    void Update () {
        if (permissionToMove == true)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;

            if (speed < 10)
            {
                distanceTillNextBoost -= Time.deltaTime;
                if (distanceTillNextBoost <= 0)
                {
                    distanceTillNextBoost = lastDistance + 10f;
                    lastDistance = distanceTillNextBoost;
                    speed++;
                }
            }
        }

        if (playerDead == false)
        {
            roundScore = Mathf.Floor(transform.position.x * 100f) / 100f;
            score.text = "Score: " + roundScore.ToString();
        }
        else
            storeScore.storedScore = roundScore;
    }
}
