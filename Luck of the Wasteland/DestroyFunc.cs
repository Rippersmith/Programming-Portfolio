using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyFunc : MonoBehaviour
{
    public void DestroyThis()
    {
        if (gameObject.transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject);

        Destroy(gameObject);
    }
}
