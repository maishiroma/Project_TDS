/*  Player Movement:
 *  The goal in this case is to have the player be moved WASD, and have their rotation follow the mouse
 *  Fairly standard top down movement.
 *  
 *  Future Ideas
 *  Jumping
 * 
 */

namespace Matt_Movement {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Matt_Generics;


    public class PlayerMovement : Entity {

        [Header("Subclass Referencess")]
        [Tooltip("The scene's camera")]
        public Camera mainCamera;

        // Private vars
        private Vector2 mousePos;       // Stores the coords for the player's mouse
        private Vector2 moveInput;      // Stores the input for the player movement
        private bool shootInput;        // Checks if the player is firing

        // Handles getting all player input
        private void Update() {
            // Get player Input for movement
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // Get player input for direction
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Get player input for firing
            shootInput = Input.GetKey(KeyCode.Mouse0);
        }

        // Handles movement and player actions
        private void FixedUpdate() {
            // Moves player
            MoveEntity();

            // Rotates player
            OrientateEntity(mousePos);

            // Player shoot input
            if(shootInput == true) {
                StartCoroutine(ShootProjectile(mousePos));
            }
        }

        // Moves the player based on their input
        protected override void MoveEntity() {
            entityRb.MovePosition(entityRb.position + (moveInput * moveSpeed * Time.fixedDeltaTime));
        }
    }

}

