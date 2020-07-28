/*  Enemy Movement
 *  Most basic: Enemies will hone in on Player, and upon hitting the player, the player will take damage
 *  and the enemy will dissapear
 * 
 */

namespace Matt_Movement
{
    using UnityEngine;
    using System.Collections;
    using Matt_Generics;
    using Matt_Gimmicks;
    using Matt_UI;

    public class EnemyMovement : Entity
    {
        [Header("Outside Refs")]
        [Tooltip("Reference to the enemy's gameobject coorelating to its range")]
        public GameObject enemyRange;
        [Tooltip("Reference to the enemy's collision hitbox")]
        public BoxCollider2D enemyCollision;

        [Header("Sub Variables")]
        [Range(0.1f, 1f)]
        [Tooltip("The amount of time it takes for this enemy to start shooting when it becomes aggresive")]
        public float timeToStartShoot = 1f; // Once the enemy gets agressive, how long it takes before it will start firing

        // Private Vars
        private Rigidbody2D playerRb;       // Caches the player's Rigidbody for future calculations
        private bool isAggresive;           // Is the enemy in an aggresive state
        private float currTime;             // Helper method to keep track of how 

        // Getter/Setter
        public bool IsAggresive
        {
            get { return isAggresive; }
            set { isAggresive = value; }
        }

        // Grabs the player's rigidbody component and stores it
        private void Start()
        {
            playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        }

        // When the enemy is aggresive, a inner timer will count down until the enemy can start attacking
        private void Update()
        {
            if (isAggresive == true && SlowMoEffect.Instance.IsInSlowMo == false)
            {
                currTime += Time.deltaTime;
            }
            else
            {
                currTime = 0;
            }
        }

        // Handles all movement logic
        private void FixedUpdate()
        {
            // If the player is in range of the enemy, they will give chase and attack
            if (isAggresive == true)
            {
                OrientateEntity(playerRb.position);
                MoveEntity();

                if (currTime >= timeToStartShoot)
                {
                    StartCoroutine(ShootProjectile(playerRb.position));
                    currTime = 0f;
                }
            }
        }

        // Overriden method from base class
        // Tells enemy to hone in on the player's position
        protected override void MoveEntity()
        {
            // If the game is in slow motion, enemy movement will be slowed
            Vector2 newPos = new Vector2();

            if (SlowMoEffect.Instance.IsInSlowMo)
            {
                float newMoveSpeed = moveSpeed * SlowMoEffect.Instance.GetSlowDownFactor;
                newPos = Vector2.MoveTowards(entityRb.position, playerRb.position, newMoveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                newPos = Vector2.MoveTowards(entityRb.position, playerRb.position, moveSpeed * Time.fixedDeltaTime);
            }
            entityRb.MovePosition(newPos);

            if (isAggresive == false)
            {
                entityGraphics.SetBool("is_moving", false);
            }
            else
            {
                entityGraphics.SetBool("is_moving", true);
            }
        }

        // Called when the enemy is considered defeated
        public IEnumerator InvokeDefeated()
        {
            // We change its graphics to show its defeat as well as stop all movement and coroutines
            entityGraphics.SetBool("is_defeated", true);
            yield return new WaitForEndOfFrame();

            StopCoroutine("ShootProjectile");
            StopMovement();
            entityRb.isKinematic = true;
            enemyRange.SetActive(false);
            enemyCollision.enabled = false;
            isAggresive = false;

            yield return new WaitForSeconds(0.5f);
            this.gameObject.SetActive(false);
        }
    
        public void ResetEnemy()
        {
            entityGraphics.SetBool("is_defeated", false);
            entityRb.isKinematic = false;
            enemyRange.SetActive(true);
            enemyCollision.enabled = true;
        }
    }
}