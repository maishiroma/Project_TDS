/*  Enemy Movement
 *  Most basic: Enemies will hone in on Player, and upon hitting the player, the player will take damage
 *  and the enemy will dissapear
 * 
 */

namespace Matt_Movement
{
    using UnityEngine;
    using Matt_Generics;
    using Matt_Gimmicks;

    public class EnemyMovement : Entity
    {

        // Private Vars
        private Rigidbody2D playerRb;       // Caches the player's Rigidbody for future calculations
        private bool isAggresive;           // Is the enemy in an aggresive state

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

        // Handles all movement logic
        private void FixedUpdate()
        {
            // If the player is in range of the enemy, they will give chase and attack
            if (isAggresive == true)
            {
                OrientateEntity(playerRb.position);
                MoveEntity();

                // If the enemy is slowed down, they will not shoot
                if (SlowMoEffect.Instance.IsInSlowMo == false)
                {
                    StartCoroutine(ShootProjectile(playerRb.position));
                }
            }
        }

        // If the enemy comes into contact with the player, they will be destroyed
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                Destroy(this.gameObject);
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
        }
    }
}