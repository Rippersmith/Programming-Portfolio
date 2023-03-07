using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonGravity : MonoBehaviour
{
    public float gravity = 100f;
    public SpriteRenderer moonMain, moonOverlay;
    protected float rotationAmount;

    // Use this for initialization
    void Start()
    {
        gravity = -gravity;
        rotationAmount = Random.Range(-100f, 100f);
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    private void Update()
    {
        if (gameObject.tag != "BlackHole")
            transform.Rotate(Vector3.forward * rotationAmount * Time.deltaTime);
    }

    public void Rotation(Transform playerTransform)
    {
        Vector3 gravityUp = (playerTransform.position - transform.position).normalized;
        Vector3 bodyUp = playerTransform.up;

        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * playerTransform.rotation;
        playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, 100 * Time.deltaTime);
    }

    //this code simulates the gravity each planet adds. It applies a force on the player focused on the center of the moon
    public void Attract(Transform playerTransform)
    {
        Vector3 gravityUp = (playerTransform.position - transform.position).normalized;
        playerTransform.GetComponent<Rigidbody2D>().AddForce(gravityUp * gravity);
    }
}