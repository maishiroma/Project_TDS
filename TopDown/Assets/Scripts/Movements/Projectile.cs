/*  The projectile travels in a straight line from what it was fired from.
 *  When in contact with anything, it will be destroyd.
 * 
 */

namespace Matt_Movement {
    using UnityEngine;

    public class Projectile : MonoBehaviour {

        [Header("General Vars")]
        [Tooltip("The travel speed of the projectile. Shpuld be a high value: i.e. 1000")]
        [Range(500, 99999)]
        public float moveSpeed = 1000f;
        [Tooltip("An array of all of the tags that this projectile can interact with")]
        public string[] interactableTags;

        [HideInInspector]
        public string origShooterTag;       // The gameobject's tag that shot this

        // Private Vars
        private Rigidbody2D rb;

        // Sets up all of the components
        private void Awake() {
            rb = GetComponent<Rigidbody2D>();
        }

        // Upon creation, the projectile will remove itself
        private void Start() {
            Invoke("DestroyItself", 5f);
        }

        // While active, it will move in a straight line
        private void FixedUpdate() {
            rb.velocity = rb.transform.up * moveSpeed * Time.deltaTime;
        }

		// Interacts with other objects
		private void OnTriggerEnter2D(Collider2D collision) {
            // If we hit something that shares the same tag as the orig shooter, we ignore them
            // Think of this as friendly fire
            if(collision.gameObject.tag != origShooterTag) {

                // In general, we only go in here if this projectile can interact with it aka, if it is in the array
                if(CheckIfTagIsInArray(collision.gameObject.tag)) {
                    switch(collision.gameObject.tag) {
                        case "Enemy":
                            // Damage Enemy
                            Destroy(collision.gameObject);
                            break;
                        case "Player":
                            // Hurt player
                            print("Ouch!");
                            break;
                        case "Projectile":
                        case "Walls":
                            // All of these cases just cause the projectile to be destroyed
                            DestroyItself();
                            break;
                    }
                    DestroyItself();
                }
            }
		}

        // Destroys the projectile
        private void DestroyItself() {
            Destroy(this.gameObject);
        }

        // Helper method to check if a tag is in the array
        private bool CheckIfTagIsInArray(string checkedTag) {
            for(int index = 0; index < interactableTags.Length; ++index) {
                if(interactableTags[index] == checkedTag) {
                    return true;
                }
            }
            return false;
        }
    }

}