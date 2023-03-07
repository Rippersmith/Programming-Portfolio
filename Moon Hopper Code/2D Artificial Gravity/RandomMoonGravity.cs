using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoonGravity : MoonGravity {

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Moon" || collision.gameObject.tag == "Gravity")
        {
            gameObject.SetActive(false);
        }
    }
}
