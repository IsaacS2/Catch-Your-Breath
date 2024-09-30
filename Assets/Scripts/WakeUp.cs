using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WakeUp : MonoBehaviour
{
    [SerializeField] private float restartTime = 1.3f;

    private Animator anim;
    private bool wokenUp;
    private float restartTimer;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (wokenUp)
        {
            restartTimer += Time.deltaTime;

            if (restartTimer >= restartTime)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // player woke up, reload the scene
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            WakeUpPlayer();
        }
    }

    public void WakeUpPlayer()
    {
        GetComponent<Collider>().enabled = false;

        // stop player from moving manually
        Movement playerMovement = GetComponent<Movement>();
        if (playerMovement != null)
        {
            Destroy(playerMovement);
        }

        // make player fall
        Rigidbody playerRb = GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.velocity = -1 * transform.up;
            playerRb.useGravity = true;
        }

        // set falling animation
        if (anim)
        {
            anim.SetBool("WokenUp", true);
        }

        wokenUp = true;
    }
}
