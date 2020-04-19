using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  Enemy Movement
 *  Most basic: Enemies will hone in on Player, and upon hitting the player, the player will take damage
 *  and the enemy will dissapear
 * 
 */

public class EnemyMovement : MonoBehaviour {

    [Header("Movement Vars")]
    [Tooltip("The speed of the enemy moving.")]
    [Range(1f, 20f)]
    public float moveSpeed = 10;

    // Private Vars
    private Rigidbody2D playerRb;
    private Rigidbody2D rb;

    // Sets up all components
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    // First, caches the player
    private void Start() {
        playerRb = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
    }

    // Hones in on the player's position
    private void FixedUpdate() {
        Vector2 newPos = Vector2.MoveTowards(rb.position, playerRb.position, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // Rotates the enemy so that it is facing the player
        Vector2 lookPos = playerRb.position - rb.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    // If the enemy comes into contact with the player, they will be destroyed
	private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Player") {
            Destroy(this.gameObject);
        }
	}

}
