/*  Enemy Movement
 *  Most basic: Enemies will hone in on Player, and upon hitting the player, the player will take damage
 *  and the enemy will dissapear
 * 
 */

namespace Matt_Movement {
    using UnityEngine;
    using Matt_Generics;

    public class EnemyMovement : Entity {
    
        // Private Vars
        private Rigidbody2D playerRb;       // Caches the player's Rigidbody for future calculations
        private bool isAttacking;           // Is the enemy attacking?

        // Getter/Setter for the isAttacking boolean
        public bool IsAttacking {
            get { return isAttacking; }
            set { isAttacking = value; }
        }

        // Grabs the player's rigidbody component and stores it
        private void Start() {
            playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
        }

        // Handles all movement logic
        private void FixedUpdate() {
            // Moves the enemy
            MoveEntity();

            // Rotates the enemy so that it is facing the player
            OrientateEntity(playerRb.position);

            // Handles attacking logic
            if(isAttacking == true) {
                StartCoroutine(ShootProjectile(playerRb.position));
            }
        }

        // If the enemy comes into contact with the player, they will be destroyed
        private void OnCollisionEnter2D(Collision2D collision) {
            if(collision.gameObject.tag == "Player") {
                Destroy(this.gameObject);
            }
        }

        // Overriden method from base class
        // Tells enemy to hone in on the player's position
        protected override void MoveEntity() {
            Vector2 newPos = Vector2.MoveTowards(entityRb.position, playerRb.position, moveSpeed * Time.fixedDeltaTime);
            entityRb.MovePosition(newPos);
        }

    }
}