using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  Player Movement:
 *  The goal in this case is to have the player be moved WASD, and have their rotation follow the mouse
 *  Fairly standard top down movement.
 *  
 *  Future Ideas
 *  Jumping
 * 
 */

namespace Matt_Movement {
    public class PlayerMovement : MonoBehaviour {

        [Header("Movement Vars")]
        [Tooltip("The speed of the player")]
        [Range(1f, 20f)]
        public float moveSpeed;
        [Tooltip("The speed of which the player can shoot")]
        [Range(0.1f,3f)]
        public float fireRate;

        [Header("Outside Refs")]
        [Tooltip("The scene's camera")]
        public Camera mainCamera;
        [Tooltip("The prefab of the bullet to use")]
        public Projectile bullet;
        [Tooltip("Reference to the player's front")]
        public Transform frontOfPlayer;

        // Private vars
        private Vector2 moveInput;
        private Vector2 mousePos;
        private bool shootInput;
        private bool hasFired;          // Has the player fired a projectile?
        private Rigidbody2D rb;

        // Sets up all of the components
        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update() {
            // Get player Input for movement
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // Get player input for direction
            mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // Get player input for firing
            shootInput = Input.GetKey(KeyCode.Mouse0);
        }

        private void FixedUpdate() {
            // Moves player
            rb.MovePosition(rb.position + (moveInput * moveSpeed * Time.fixedDeltaTime) );

            // Rotates player
            Vector2 lookPos = mousePos - rb.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;

            // Player shoot input
            if(shootInput == true) {
                StartCoroutine(ShootProjectile());
            }
        }

        // If the player's cooldown has reset, allow the player to fire
        private IEnumerator ShootProjectile() {
            if (hasFired == false) {
                Quaternion shotRotation = Quaternion.FromToRotation(Vector3.up, mousePos - rb.position);
                Projectile bulletShot = Instantiate(bullet, frontOfPlayer.position, shotRotation, null);
                bulletShot.origShooterTag = gameObject.tag;

                hasFired = true;
                yield return new WaitForSeconds(fireRate);
                hasFired = false;
            }
            yield return null;
        }
    }

}

