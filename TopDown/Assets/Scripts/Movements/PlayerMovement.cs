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

    // This describes the movement state of the player at a given moment
    // Depending on what state the player is in, various actions are allowed/not allowed
    public enum MovementState
    {
        NORMAL,                     // Standard state
        DODGING,                    // State in which the player is considered dodging an attack
        DASHING,                    // State in which the player is moving fast for a brief moment
        COOLDOWN,                   // State where the player cannot move or perform any movement actions
        SUCCESSFUL_DODGE_NORMAL     // State where it is identical to NORMAL, but the player cannot perform dodges
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

        // Getter/Setters
        public MovementState GetPlayerMovementState
        {
            get { return playerMovementState; }
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

            // Gets player input for special movements
            specialMovementInput = Input.GetKey(KeyCode.Space);
        }

        // Handles movement and player actions
        private void FixedUpdate()
        {
            // Checks if the player can recover from a cooldown state after dodging
            RevertFromSuccessDodgeState();

            // Rotates player
            OrientateEntity(mousePos);

            // If the player is pressing on the special movement input, they do different actions than normal
            if (specialMovementInput == true)
            {
                if (playerMovementState == MovementState.SUCCESSFUL_DODGE_NORMAL)
                {
                    // If the player is in a state where they dodged an attack, they can only do standard movement
                    // Moves player
                    MoveEntity();

                    // Player shoot input
                    if (shootInput == true)
                    {
                        StartCoroutine(ShootProjectile(mousePos));
                    }
                }
                else if (playerMovementState == MovementState.NORMAL)
                {
                    // If they are in a normal state, they can only do dashes and dodges
                    if (moveInput.x == 0f && moveInput.y == 0f)
                    {
                        // The player performs a dodge ONLY if they are NOT in a special state
                        StartCoroutine("PerformDodge");
                    }
                    else
                    {
                        // The player performs a dash
                        StartCoroutine(PerformDash(moveInput));
                    }
                }
            }
            else if (playerMovementState == MovementState.NORMAL || playerMovementState == MovementState.SUCCESSFUL_DODGE_NORMAL)
            {
                // As long as the player is in a normal state, they can always do these actions

                // Moves player
                MoveEntity();

                // Player shoot input
                if (shootInput == true)
                {
                    StartCoroutine(ShootProjectile(mousePos));
                }
            }
        }

        // Moves the player based on their input
        protected override void MoveEntity()
        {
            entityRb.MovePosition(entityRb.position + (moveInput * moveSpeed * Time.fixedDeltaTime));
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
            entityRb.velocity = Vector2.zero;
            playerDodge.gameObject.SetActive(true);
            yield return new WaitForSeconds(dodgeTime);

            // Then they are in a state where they cannot do any action
            playerMovementState = MovementState.COOLDOWN;
            playerDodge.gameObject.SetActive(false);
            yield return new WaitForSeconds(dodgeCoolDown);

            // And then they return to normal
            playerMovementState = MovementState.NORMAL;
            yield return null;
        }

        // Perform the dash movement when called on
        private IEnumerator PerformDash(Vector2 movementDir)
        {
            // The player dashes a timed amount of disitance
            playerMovementState = MovementState.DASHING;
            entityRb.AddForce(movementDir * dashSpeed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(dashTime);

            // Then they go into a state where they cannot do anything
            playerMovementState = MovementState.COOLDOWN;
            entityRb.velocity = Vector2.zero;
            yield return new WaitForSeconds(dashCoolDown);

            playerMovementState = MovementState.NORMAL;
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
                playerDodge.gameObject.SetActive(false);
                playerMovementState = MovementState.SUCCESSFUL_DODGE_NORMAL;
            }
            yield return null;
        }

    }
}

