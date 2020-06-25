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
    using Matt_UI;
    using System.Collections;

    // This describes the movement state of the player at a given moment
    // Depending on what state the player is in, various actions are allowed/not allowed
    public enum MovementState
    {
        NORMAL,                     // Standard state
        DODGING,                    // State in which the player is considered dodging an attack
        DASHING,                    // State in which the player is moving fast for a brief moment
        COOLDOWN,                   // State where the player cannot move or perform any movement actions
        SUCCESSFUL_DODGE_NORMAL,    // State where it is identical to NORMAL, but the player cannot perform dodges
        STUNNED                     // State where the player cannot move, due to hitstun from an enemy
    }

    public class PlayerMovement : Entity
    {

        [Tooltip("The scene's camera")]
        public Camera mainCamera;
        [Tooltip("The trigger used to catch the player's dodge")]
        public DodgeTrigger playerDodge;

        [Header("Player Specific Movements")]
        [Tooltip("How fast does the player move when dashing")]
        [Range(30f, 60f)]
        public float dashSpeed = 30f;
        [Range(0.1f, 2f)]
        public float dashTime = 0.1f;
        [Range(0.1f, 2f)]
        public float dashCoolDown = 0.2f;
        [Tooltip("How long does the player stay stunned when damaged?")]
        [Range(0.1f, 1f)]
        public float stunTime = 0.5f;

        [Space]

        [Tooltip("How long does the dodge last")]
        [Range(0.1f, 2f)]
        public float dodgeTime = 0.1f;
        [Tooltip("How long does the dodge cooldown last")]
        [Range(0.1f, 2f)]
        public float dodgeCoolDown = 0.2f;

        // Private vars
        private MovementState playerMovementState = MovementState.NORMAL;       // Gets the player's movement state
        private Vector2 mousePos;                   // Stores the coords for the player's mouse
        private Vector2 moveInput;                  // Stores the input for the player movement
        private bool shootInput;                    // Checks if the player is firing
        private bool specialMovementInput;          // Checks if the player is performing a special movement input
        private float prevDis;                      // Stores a ref to the disitance that was between the player and the mouse

        // Getter/Setters
        public MovementState GetPlayerMovementState
        {
            get { return playerMovementState; }
        }

        // Handles getting all player input
        private void Update()
        {
            // If the player is stunned, they can't move
            if (playerMovementState != MovementState.STUNNED)
            {
                // Get player Input for movement
                moveInput.x = Input.GetAxisRaw("Horizontal");
                moveInput.y = Input.GetAxisRaw("Vertical");

                // Get player input for direction
                mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                // Get player input for firing
                shootInput = Input.GetKey(KeyCode.Mouse0);

                // Gets player input for special movements
                specialMovementInput = Input.GetKey(KeyCode.Space);
            }
        }

        // Handles movement and player actions
        private void FixedUpdate()
        {
            // If the player is stunned, they can't do anything
            if (playerMovementState != MovementState.STUNNED)
            {
                // Checks if the player can recover from a cooldown state after dodging
                RevertFromSuccessDodgeState();

                // Rotates player
                OrientateEntity(mousePos);

                // The player can do any movement option as long as they are not in cooldown
                if (playerMovementState == MovementState.NORMAL || playerMovementState == MovementState.SUCCESSFUL_DODGE_NORMAL)
                {
                    if (specialMovementInput == true)
                    {
                        // Dodging is pretty powerful, so this can only be done if they player is in a normal state and not
                        // invincible
                        if (moveInput.x == 0f && moveInput.y == 0f && playerMovementState == MovementState.NORMAL && PlayerHealth.Instance.GetInvincible == false)
                        {
                            // The player performs a dodge
                            StartCoroutine("PerformDodge");
                        }
                        else
                        {
                            // The player performs a dash
                            StartCoroutine(PerformDash(moveInput));
                        }
                    }
                    else
                    {
                        // Moves player
                        MoveEntity();

                        // Player shoot input
                        if (shootInput == true)
                        {
                            StartCoroutine(ShootProjectile(mousePos));
                        }
                    }
                }
            }
        }

        // Handles a slight graphical issue with dashing as the player
        private void LateUpdate()
        {
            if (playerMovementState == MovementState.DASHING)
            {
                // If the player is dashing away from the mouse, they are "reversed" so it looks like
                // They are running away properly
                if (Mathf.Sign(prevDis - Vector2.Distance(entityRb.position, mousePos)) < 0)
                {
                    entityRenderer.flipY = true;
                }
            }
            else
            {
                entityRenderer.flipY = false;
            }
        }

        // Moves the player based on their input
        protected override void MoveEntity()
        {
            prevDis = Vector2.Distance(entityRb.position, mousePos);
            entityRb.MovePosition(entityRb.position + (moveInput * moveSpeed * Time.fixedDeltaTime));

            if (moveInput == Vector2.zero)
            {
                entityGraphics.SetBool("is_moving", false);
            }
            else
            {
                entityGraphics.SetBool("is_moving", true);
            }
        }

        // Helper function that stops all player movement
        private void StopMovement()
        {
            entityRb.velocity = Vector2.zero;
            entityRb.angularVelocity = 0f;
        }

        // Reverts the player state from the state where they cannot do any more dodges
        private void RevertFromSuccessDodgeState()
        {
            if (playerMovementState == MovementState.SUCCESSFUL_DODGE_NORMAL)
            {
                if (SlowMoEffect.Instance.isReadyToBeUsed == true)
                {
                    playerMovementState = MovementState.NORMAL;
                }
            }
        }

        // Performs the dodge action when called on
        private IEnumerator PerformDodge()
        {
            // The player is in the state of dodge for a brief moment
            playerMovementState = MovementState.DODGING;
            entityGraphics.SetBool("is_dodging", true);
            StopMovement();
            playerDodge.gameObject.SetActive(true);
            yield return new WaitForSeconds(dodgeTime);

            // Then they are in a state where they cannot do any action
            playerMovementState = MovementState.COOLDOWN;
            playerDodge.gameObject.SetActive(false);
            yield return new WaitForSeconds(dodgeCoolDown);

            // And then they return to normal
            entityGraphics.SetBool("is_dodging", false);
            playerMovementState = MovementState.NORMAL;
            yield return null;
        }

        // Perform the dash movement when called on
        private IEnumerator PerformDash(Vector2 movementDir)
        {
            MovementState oldState = playerMovementState;

            // The player dashes a timed amount of disitance
            entityGraphics.SetBool("is_dashing", true);
            playerMovementState = MovementState.DASHING;
            entityRb.AddForce(movementDir * dashSpeed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(dashTime);

            // Then they go into a state where they cannot do anything
            entityGraphics.SetBool("is_dashing", false);
            playerMovementState = MovementState.COOLDOWN;
            StopMovement();
            yield return new WaitForSeconds(dashCoolDown);

            // We revert to the state the player was in when they came into this function
            playerMovementState = oldState;
            yield return null;
        }

        // When called on, the player will be able to move instantly after a dodge.
        public IEnumerator SlowMoBonus()
        {
            if (SlowMoEffect.Instance.IsInSlowMo == true)
            {
                // Note: StopCoroutine only works if the coroutine used to create is uses a string argument
                // This stops the delay that the player suffers from a dodge
                // However, this also puts the player in a state where they cannot do any more additional dodges
                StopCoroutine("PerformDodge");
                entityGraphics.SetBool("is_dodging", false);
                playerDodge.gameObject.SetActive(false);
                playerMovementState = MovementState.SUCCESSFUL_DODGE_NORMAL;
            }
            yield return null;
        }

        // When called, the player will experience hitstun, meaning they won't be able to move
        // This is called when the player takes damage
        public IEnumerator EnactHitStun()
        {
            if (playerMovementState != MovementState.STUNNED)
            {
                playerMovementState = MovementState.STUNNED;
                StopMovement();
                yield return new WaitForSeconds(stunTime);
                playerMovementState = MovementState.NORMAL;
            }
        }

    }
}

