/*  Player Movement:
 *  The goal in this case is to have the player be moved WASD, and have their rotation follow the mouse
 *  Fairly standard top down movement.
 *  
 *  Future Ideas
 *  Jumping
 * 
 */

namespace Matt_Movement {
    using UnityEngine;
    using Matt_Generics;
    using System.Collections;

    public class PlayerMovement : Entity {

        [Header("Subclass Referencess")]
        [Tooltip("The scene's camera")]
        public Camera mainCamera;
      
        // Private vars
        private Vector2 mousePos;       // Stores the coords for the player's mouse
        private Vector2 moveInput;      // Stores the input for the player movement
        private bool shootInput;        // Checks if the player is firing
        private bool dodgeInput;        // Checks if the player is dodging
        private int currNumbOfDodges;

        // Handles getting all player input
        private void Update() {
            // Get player Input for movement
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // Get player input for direction
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Get player input for firing
            shootInput = Input.GetKey(KeyCode.Mouse0);

            // Gets player input for dodging
            if(Input.GetKey(KeyCode.Space) && isInDodgeCoolDown == false) {
                dodgeInput = true;
            }
            else {
                dodgeInput = false;
            }
        }

        // Handles movement and player actions
        private void FixedUpdate() {
            if (dodgeInput == true && moveInput != Vector2.zero) {
                // If we input dodge and a movement, we perform the dodge
                StartCoroutine(DodgeAction(moveInput));
            }
            else {
                // If we are not dodging, we do normal movement
                if (isDodging == false) {
                    // Moves player
                    MoveEntity();

                    // Rotates player
                    OrientateEntity(mousePos);

                    // Player shoot input
                    if (shootInput == true && isInDodgeCoolDown == false) {
                        StartCoroutine(ShootProjectile(mousePos));
                    }
                }
            }
        }

        // Moves the player based on their input
        protected override void MoveEntity() {
            entityRb.MovePosition(entityRb.position + (moveInput * moveSpeed * Time.fixedDeltaTime));
        }
    }

}

