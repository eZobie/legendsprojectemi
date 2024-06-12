using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetHit : MonoBehaviour
{
    [Tooltip("Determines when the player is taking damage.")]
    public bool hurt = false;

    private bool slipping = false;
    private PlayerMovement playerMovementScript;
    private Rigidbody rb;
    private Transform enemy;

    private void Start()
    {
        playerMovementScript = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // stops the player from running up the slopes and skipping platforms
        if (slipping == true)
        {
            transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
            playerMovementScript.playerStats.canMove = false;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!hurt)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                enemy = other.gameObject.transform;
                rb.AddForce(enemy.forward * 1000);
                rb.AddForce(transform.up * 500);
                TakeDamage(10); // Remove 10 points from health
            }
            if (other.gameObject.CompareTag("Trap"))
            {
                rb.AddForce(transform.forward * -1000);
                rb.AddForce(transform.up * 500);
                TakeDamage(10); // Remove 10 points from health
            }
        }

        if (other.gameObject.layer == 9)
        {
            slipping = true;
        }
        else if (slipping)
        {
            slipping = false;
            playerMovementScript.playerStats.canMove = true;
        }
    }

    private void TakeDamage(float amount)
    {
        hurt = true;
        playerMovementScript.playerStats.canMove = false;
        playerMovementScript.soundManager.PlayHitSound();
        playerMovementScript.TakeDamage(amount); // Use the TakeDamage method from PlayerMovement
        StartCoroutine(Recover());

        if (playerMovementScript.playerStats.health <= 0)
        {
            RestartGame(); // Call the method to restart the game
        }
    }

    private IEnumerator Recover()
    {
        yield return new WaitForSeconds(0.75f);
        hurt = false;
        playerMovementScript.playerStats.canMove = true;
    }

    private void RestartGame()
    {
        // Restart the game here
        // You can reload the current scene or reset player position, etc.
        // For example:
        Debug.Log("Dead");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
