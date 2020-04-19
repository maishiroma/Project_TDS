using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*  The bullet travels in a straight line from what it was fired from.
 *  When in contact with anything, it will be destroyd.
 * 
 */

public class Bullet : MonoBehaviour {

    [Header("General Vars")]
    [Tooltip("The travel speed of the bullet. Shpuld be a high value: i.e. 1000")]
    [Range(500, 99999)]
    public float moveSpeed = 1000f;

    // Private Vars
    private Rigidbody2D rb;

    // Sets up all of the components
	private void Awake() {
        rb = GetComponent<Rigidbody2D>();
	}

	private void Start() {
        // Upon creation, the bullet will remove itself
        Invoke("DestroyItself", 5f);
	}

	private void FixedUpdate() {
        // While active, it will move in a straight line
        rb.velocity = rb.transform.up * moveSpeed * Time.deltaTime;
	}

    // Interacts with other objects
	private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag != "Player") {

            // Damage Enemy
            if(collision.gameObject.tag == "Enemy") {
                Destroy(collision.gameObject);
            }

            // As long as the bullet hit anything, it will dissapear
            DestroyItself();
        }
	}

    // Destroys the bullet
	private void DestroyItself() {
        Destroy(this.gameObject);
    }
}
