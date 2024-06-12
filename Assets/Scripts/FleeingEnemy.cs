using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeingEnemy : MonoBehaviour
{
    public Stats enemyStats;

    [Tooltip("The transform that will lock onto the player once the enemy has spotted them.")]
    public Transform sight;

    [Tooltip("Blue explosion particles")]
    public GameObject enemyExplosionParticles;

    private bool slipping = false;

    private float facing;

    private Rigidbody rb;

    private GameObject player;

    private Patrol patrolBehavior;

    public GameObject playerGameObject;

    /// <summary>
    /// Contains tunable parameters to tweak the enemy's movement and behavior.
    /// </summary>
    [System.Serializable]
    public struct Stats
    {
        [Header("Enemy Settings")]
        [Tooltip("How fast the enemy walks (only when idle is true).")]
        public float walkSpeed;

        [Tooltip("How fast the enemy turns in circles as they're walking (only when idle is true).")]
        public float rotateSpeed;

        [Tooltip("How fast the enemy runs after you (only when idle is false).")]
        public float chaseSpeed;

        [Tooltip("Whether the enemy is idle or not. Once the player is within distance, idle will turn false and the enemy will chase the player.")]
        public bool idle;

        [Tooltip("How close the enemy needs to be to explode")]
        public float explodeDist;

        [Tooltip("The health threshold above which the enemy will flee")]
        public float fleeHealthThreshold;
    }

    private enum EnemyState
    {
        Idle,
        Chasing,
        Fleeing
    }

    private EnemyState currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        patrolBehavior = GetComponent<Patrol>();
        player = playerGameObject.GetComponent<PlayerMovement>().gameObject;
        currentState = EnemyState.Idle;
    }

    private void Update()
    {
        float playerHealth = player.GetComponent<PlayerMovement>().playerStats.health;

        if (playerHealth >= enemyStats.fleeHealthThreshold)
        {
            currentState = EnemyState.Fleeing;
        }
        else if (!enemyStats.idle)
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            currentState = EnemyState.Idle;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                patrolBehavior.Move(enemyStats.walkSpeed);
                break;
            case EnemyState.Chasing:
                Chase();
                CheckExplode();
                break;
            case EnemyState.Fleeing:
                Flee();
                break;
        }

        CheckSlipping();
    }

    private void Flee()
    {
        Vector3 runDirection = transform.position - player.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + runDirection, enemyStats.chaseSpeed * Time.deltaTime);
    }

    private void Chase()
    {
        sight.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(sight);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * enemyStats.chaseSpeed);
    }

    private void CheckSlipping()
    {
        if (slipping == true)
        {
            transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
        }
    }

    private void CheckExplode()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < enemyStats.explodeDist)
        {
            StartCoroutine("Explode");
            enemyStats.idle = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 9)
        {
            slipping = true;
        }
        else
        {
            slipping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject;
            enemyStats.idle = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enemyStats.idle = true;
        }
    }

    private IEnumerator Explode()
    {
        GameObject particles = Instantiate(enemyExplosionParticles, transform.position, new Quaternion());
        yield return new WaitForSeconds(0.2f);
        Destroy(this.gameObject);
        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);
    }
}
