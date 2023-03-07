using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public MoonGravity moonGravity;
    public BoxCollider2D body;
    public BoxCollider2D sensor;
    public bool grounded = false;
    public float moveSpeed = 10f;

    public Rigidbody2D rb;
    public GameObject blackHole;
    public MoveCamera mainCam;
    public ExplosionEffect moonExplosion;

    public SoundManager soundManager;
    public AudioClip jumpSfx, speedUpSfx, slowDownSfx, dieSfx;

    bool inNewGravity = false;
    bool startJump = false;
    bool caughtInBlackHole = false;
    float moveSpeedReset;

    Vector3 moveDir;
    SpriteRenderer spriteRend;
    Animator anim;
    Coroutine moveSpeedCoroutine = null;

    // Use this for initialization
    void Awake()
    {
        moveSpeedReset = moveSpeed;
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        soundManager = GameObject.Find("Sound Manager").GetComponent<SoundManager>();
    }

    private void Update()
    {
        if (caughtInBlackHole == false)
        {
            if (moveDir.x < 0 && spriteRend.flipX == false)
                spriteRend.flipX = true;
            else if (moveDir.x > 0 && spriteRend.flipX == true)
                spriteRend.flipX = false;

            if (moonGravity != null)
                moonGravity.Rotation(transform);

            //lower the player gravity when jumping, so jump is way more "uppy" and not so heavy
            if (grounded == true) rb.mass = 1;
            else rb.mass = .15f;

            //if (Input.GetKeyDown(KeyCode.Return))
            //{ startJump = true; }

            //rarely, the player will flip on the y-axis for some unknown reason. This code is to prevent that from happening
            if (transform.rotation.y != 0f)
                transform.eulerAngles = new Vector3(0f, 0f, transform.rotation.z);
        }

    }

    void FixedUpdate()
    {
        //move the character based on the button pressed (on the UI)
        if (caughtInBlackHole == false)
            rb.MovePosition(rb.position + (Vector2)transform.TransformDirection(moveDir * moveSpeed * Time.deltaTime));

        if (moonGravity != null)
            moonGravity.Attract(transform);

        if (startJump == true)
        {
            startJump = false;
            if (grounded == true)
            {
                anim.SetTrigger("Jump");
                rb.mass = .1f;
                StartCoroutine(Jump());
            }
        }
    }

    //These scripts are used to change booleans right away
    public void JumpFunc()
    {
        startJump = true;
    }

    public void MoveLeft()
    {
        moveDir = Vector3.right;
    }

    public void MoveRight()
    {
        moveDir = Vector3.left;
    }

    public void StopMoving()
    {
        moveDir = Vector3.zero;
    }

    //using an IEnumerator as the Jump function makes the rise & fall of the jump feel more natural
    IEnumerator Jump()
    {
        float force = 230f;

        soundManager.PlaySinglePlayer(jumpSfx);
        transform.parent = null;
        for (int i = 0; i < 15; i++)
        {
            if (inNewGravity != true)
                rb.AddForce(transform.up * force);
            yield return new WaitForFixedUpdate();
            force -= 15f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if you jump on a moon, make the player's character a child of the moon & have it "pulled" by the moon's gravity, if not already
        if (collision.gameObject.tag == "Moon")
        {
            grounded = true;
            StopCoroutine(Jump());
            if (moonGravity != collision.gameObject.GetComponentInParent<MoonGravity>() && caughtInBlackHole == false)
            {
                moonGravity = collision.gameObject.GetComponentInParent<MoonGravity>();
                rb.velocity = Vector2.zero;
                transform.parent = collision.gameObject.transform;
            }

            rb.AddForce(-transform.up * 100f);
            inNewGravity = false;
        }
        //if the player enters a moon's gravity field, have the new moon "pull" the player in
        else if (collision.gameObject.tag == "Gravity" && inNewGravity == false && grounded == false && caughtInBlackHole == false)
        {
            StopCoroutine(Jump());
            inNewGravity = true;
            moonGravity = collision.gameObject.GetComponentInParent<MoonGravity>();
            rb.velocity = Vector2.zero;
            transform.parent = collision.gameObject.transform;
        }
        //this script kills the player & blows up the moon if they run into a "bomb" power-up
        else if (collision.gameObject.tag == "SelfDestruct")
        {
            GameObject baseMoon = moonGravity.gameObject;
            moonExplosion.GetColorsAndPlayExplosion(baseMoon.transform.position,
                   baseMoon.GetComponent<SpriteRenderer>().color, baseMoon.transform.Find("MoonOverlay").GetComponent<SpriteRenderer>().color);

            collision.transform.parent.gameObject.SetActive(false);
            transform.parent = null;

            Destroyed();
        }
        else if (collision.gameObject.tag == "BlackHole")
        {
            GetCaughtInBlackHole();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Moon")
        {
            grounded = false;
        }
    }

    //This script is run if the player "dies", and starts loading the GameOver scene
    void Destroyed()
    {
        soundManager.PlaySinglePlayer(dieSfx);
        soundManager.StopBackgroundMusic();
        caughtInBlackHole = true;
        anim.SetBool("Dead", true);
        inNewGravity = true;
        body.enabled = false;
        sensor.enabled = false;
        mainCam.playerDead = true;
        StartCoroutine(LoadGameOverScene());
    }

    //kill the player if they get caught in the black hole
    void GetCaughtInBlackHole()
    {
        moonGravity = blackHole.GetComponent<MoonGravity>();
        transform.parent = blackHole.transform;
        Destroyed();
    }

    IEnumerator LoadGameOverScene()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    //these next 4 scripts are run if the player picks up a power-up to either speed up/slow down the player
    public void StartSpeedUp()
    {
        moveSpeed = moveSpeedReset;
        soundManager.PlaySingleItem(speedUpSfx);
        if (moveSpeedCoroutine != null) StopCoroutine(moveSpeedCoroutine);
        moveSpeedCoroutine = StartCoroutine(TempSpeedUp());
    }

    public IEnumerator TempSpeedUp()
    {
        moveSpeed *= 1.5f;
        yield return new WaitForSeconds(7.5f);
        moveSpeed = moveSpeedReset;
    }

    public void StartSlowDown()
    {
        moveSpeed = moveSpeedReset;
        soundManager.PlaySingleItem(slowDownSfx);
        if (moveSpeedCoroutine != null) StopCoroutine(moveSpeedCoroutine);
        moveSpeedCoroutine = StartCoroutine(TempSlowDown());
    }

    public IEnumerator TempSlowDown()
    {
        moveSpeed /= 2f;
        yield return new WaitForSeconds(7.5f);
        moveSpeed = moveSpeedReset;
    }

}
