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
        public GameObject enemyRange;
        public BoxCollider2D enemyCollision;

        [Header("Sub Variables")]
        [Range(0.1f, 1f)]
        [Tooltip("The amount of time it takes for this enemy to start shooting when it becomes aggresive")]
        public float timeToStartShoot = 1f; // Once the enemy gets agressive, how long it takes before it will start firing

        // Private Vars
        private Rigidbody2D playerRb;       // Caches the player's Rigidbody for future calculations
        private bool isAggresive;           // Is the enemy in an aggresive state
        private float currTime;

        // Getter/Setter
        public bool IsAggresive
        {
            get { return isAggresive; }

            set
            {
                //// Depending on the enemy's fire rate, we slow down the enemy's attack animation to fit the need
                //if (value == true)
                //{
                //    entityGraphics.speed /= attackRate;
                //}
                //else
                //{
                //    entityGraphics.speed = 1f;
                //}

                isAggresive = value;
            }
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

        // If the enemy comes into contact with the player, they will be destroyed
        // But the player will also take damage.
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                // If the game is in slow mo, they do not get affected by enemies
                if (SlowMoEffect.Instance.IsInSlowMo)
                {
                    return;
                }

                // If the player is not invincible, they take damage
                if (PlayerHealth.Instance.IsInvincible == false)
                {
                    PlayerHealth.Instance.CurrentHealth -= 1;
                }
                StartCoroutine(InvokeDefeated());
            }
        }

        // Overriden method from base class
        // Tells enemy to hone in on the player's position
        protected override void MoveEntity()
        {
            // If the game is in slow motion, enemy movement will be slowed
            Vector2 newPos;
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

            // We then scale the size of the sprite, (since the sprite was too small)
            entityRenderer.transform.localScale = new Vector3(3f, 3f, 0);

            yield return new WaitForSeconds(0.5f);
            Destroy(this.gameObject);
        }
    }
}