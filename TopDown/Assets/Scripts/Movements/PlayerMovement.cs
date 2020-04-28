/*  Player Movement:
 *  The goal in this case is to have the player be moved WASD, and have their rotation follow the mouse
 *  Fairly standard top down movement.
 *  
 *  Future Ideas
 *  Jumping
 * 
 */

namespace Matt_Movement
{
    using UnityEngine;
    using Matt_Generics;
    using Matt_Gimmicks;
    using System.Collections;

    public class PlayerMovement : Entity
    {

        [Header("Subclass Referencess")]
        [Tooltip("The scene's camera")]
        public Camera mainCamera;

        [Tooltip("How fast does the entity move when dodging")]
        [Range(30f, 60f)]
        public float dodgeSpeed = 30f;
        [Tooltip("How long does the dodge last")]
        [Range(0.1f, 2f)]
        public float dodgeTime = 0.1f;
        [Tooltip("How long does the dodge cooldown last")]
        [Range(0.1f, 2f)]
        public float dodgeCoolDownTime = 0.2f;
        public DodgeTrigger playerDodge;

        // Private vars
        private Vector2 mousePos;       // Stores the coords for the player's mouse
        private Vector2 moveInput;      // Stores the input for the player movement
        private bool shootInput;        // Checks if the player is firing
        private bool dodgeInput;        // Checks if the player is dodging

        protected bool isDodging;               // Is the entity currently dodging?
        protected bool isInDodgeCoolDown;       // Is the entity in a dodge cool down?

        // Getter/Setters
        public bool IsDodging
        {
            get { return isDodging; }
        }

        // Handles getting all player input
        private void Update()
        {
            // Get player Input for movement
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // Get player input for direction
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Get player input for firing
            shootInput = Input.GetKey(KeyCode.Mouse0);

            // Gets player input for dodging
            if (Input.GetKey(KeyCode.Space) && isInDodgeCoolDown == false)
            {
                dodgeInput = true;
            }
            else
            {
                dodgeInput = false;
            }
        }

        // Handles movement and player actions
        private void FixedUpdate()
        {
            if (dodgeInput == true && moveInput != Vector2.zero)
            {
                // If we input dodge and a movement, we perform the dodge
                StartCoroutine(DodgeAction(moveInput));
            }
            else
            {
                // If we are not dodging, we do normal movement
                if (isDodging == false)
                {
                    // Moves player
                    MoveEntity();

                    // Rotates player
                    OrientateEntity(mousePos);

                    // Player shoot input
                    if (shootInput == true && isInDodgeCoolDown == false)
                    {
                        StartCoroutine(ShootProjectile(mousePos));
                    }
                }
            }
        }

        // Moves the player based on their input
        protected override void MoveEntity()
        {
            entityRb.MovePosition(entityRb.position + (moveInput * moveSpeed * Time.fixedDeltaTime));
        }

        // Sets the entity in a dodge state, allowing them to move quickly in a given direction
        protected IEnumerator DodgeAction(Vector2 dodgeDir)
        {
            if (isDodging == false && isInDodgeCoolDown == false)
            {
                // We apply a quick force on the entity given the direction passed in
                isDodging = true;
                entityRb.AddForce(dodgeDir * dodgeSpeed, ForceMode2D.Impulse);
                playerDodge.gameObject.SetActive(true);
                yield return new WaitForSeconds(dodgeTime);

                // When this is reached, the player should not be able to move
                playerDodge.gameObject.SetActive(false);
                entityRb.velocity = Vector2.zero;
                isInDodgeCoolDown = true;
                isDodging = false;

                // Time spent waiting for the player to be able to dodge again
                yield return new WaitForSeconds(dodgeCoolDownTime);
                isInDodgeCoolDown = false;
            }
        }
    }

}

