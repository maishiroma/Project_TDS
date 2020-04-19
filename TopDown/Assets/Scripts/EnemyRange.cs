using UnityEngine;
using System.Collections;

/*  Enemy Range
 *  Dictates whether anything of interest is in the enemy's range.
 *  If so, a specific value will be triggered in the respected components
 */

namespace Matt_Movement {

    [RequireComponent(typeof(CircleCollider2D))]
    public class EnemyRange : MonoBehaviour {

        [Header("General Refs")]
        [Tooltip("An array of all of the tags that the enemy can interact with")]
        public string[] interactableTags;

        [Header("Outside Refs")]
        [Tooltip("Ref to the enemy associated with this range")]
        public EnemyMovement enemyMovement;

        private void Start() {
            if (enemyMovement == null) {
                Debug.LogError("Need enemy movment component on!");
                Destroy(this.gameObject);
            }
        }

		// If a specific object enters the range of this, something happens
        private void OnTriggerStay2D(Collider2D collision) {
            if(checkIfTagIsInArray(collision.gameObject.tag)) {
                switch(collision.gameObject.tag) {
                    case "Player":
                        // If the player is in our range, we shoot at them.
                        StartCoroutine(enemyMovement.ShootProjectile());
                        break;
                }
            }
		}

        // Helper method to check if a tag is in the array
        private bool checkIfTagIsInArray(string tag) {
            for(int index = 0; index < interactableTags.Length; ++index) {
                if(interactableTags[index] == tag) {
                    return true;
                }
            }
            return false;
        }
	}

}