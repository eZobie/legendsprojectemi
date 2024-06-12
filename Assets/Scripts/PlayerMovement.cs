using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    private void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    public struct Stats
    {
        [Tooltip("Player Health Meter")]
        public float health;
        
        [Tooltip("Player Max Health")]
        public float maxHealth;

        [Tooltip("How fast the player runs.")]
        public float speed;

        [Tooltip("How high the player jumps.")]
        public float jumpForce;

        [Tooltip("Whether the player is allowed to move or not.")]
        public bool canMove;

        [Tooltip("When the player is allowed to jump or not.")]
        public bool canJump;

        public void changeHealth(float change)
        {
            health += change;
            health = Mathf.Clamp(health, 0, maxHealth);
        }
    }

    public Stats playerStats;
    [Tooltip("Health")]
    public Slider healthBar;

    [Tooltip("The script that will play the player's sound effects.")]
    public SoundManager soundManager;

    [Tooltip("Which layer allows the player to jump.")]
    public LayerMask groundLayer;

    [Tooltip("The transform that detects what layer the player is on.")]
    public Transform groundCheckL, groundCheckR;

    [Tooltip("The transform that the player's directional movement will be based upon.")]
    public Transform mainCamera;

    private float moveX, moveY;
    private float facing;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerStats.maxHealth = playerStats.health; // Initialize max health
        UpdateHealthUI(); // Initialize the health bar
    }

    private void Update()
    {
        // Update health bar slider value
        healthBar.value = playerStats.health; // health represents the actual health value (0-100)

        if (playerStats.canMove == true)
        {
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");

            bool hitL = Physics.Linecast(new Vector3(groundCheckL.position.x, transform.position.y + 1, transform.position.z), groundCheckL.position, groundLayer);
            bool hitR = Physics.Linecast(new Vector3(groundCheckR.position.x, transform.position.y + 1, transform.position.z), groundCheckR.position, groundLayer);
            Debug.DrawLine(new Vector3(groundCheckL.position.x, transform.position.y + 1, transform.position.z), groundCheckL.position, Color.red);
            Debug.DrawLine(new Vector3(groundCheckR.position.x, transform.position.y + 1, transform.position.z), groundCheckR.position, Color.red);

            if (hitL || hitR)
            {
                playerStats.canJump = true;
            }
            else
            {
                playerStats.canJump = false;
            }

            if (playerStats.canJump)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (playerStats.canMove == true)
        {
            Vector3 movement = ((mainCamera.right * moveX) * playerStats.speed) + ((mainCamera.forward * moveY) * playerStats.speed);
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

            if (movement.x != 0 && movement.z != 0)
            {
                facing = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            }
            rb.rotation = Quaternion.Euler(0, facing, 0);
        }
    }

    private void Jump()
    {
        playerStats.canJump = false;
        soundManager.PlayJumpSound();
        rb.AddForce(Vector3.up * playerStats.jumpForce);
    }

    public void TakeDamage(float amount)
    {
        playerStats.changeHealth(-amount);
        UpdateHealthUI();
    }

    public void RestoreHealth(float amount)
    {
        playerStats.changeHealth(amount);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthBar.value = playerStats.health; // health bar value directly corresponds to player's health
    }
}
