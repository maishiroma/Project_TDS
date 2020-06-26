/*  The projectile travels in a straight line from what it was fired from.
 *  When in contact with anything, it will be destroyd.
 * 
 */

namespace Matt_Movement
{
    using UnityEngine;
    using Matt_Gimmicks;
    using Matt_UI;
    using Matt_System;

    public class Projectile : MonoBehaviour
    {
        // Private Static Vars
        private static ScoreSystem scoreSystem;

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
        private float timeToDestroy = 0f;

        // Sets up all of the components
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (scoreSystem == null)
            {
                scoreSystem = FindObjectOfType<ScoreSystem>();
            }
        }

        // Keeps track of how long it will take to remove this object
        private void Update()
        {
            // If slow mo is active, the bullet will be slowed down to destroy
            if (SlowMoEffect.Instance.IsInSlowMo == true)
            {
                timeToDestroy += (Time.deltaTime * SlowMoEffect.Instance.GetSlowDownFactor);
            }
            else
            {
                timeToDestroy += Time.deltaTime;
            }

            if (timeToDestroy >= 5f)
            {
                DestroyItself();
            }
        }

        // While active, it will move in a straight line
        private void FixedUpdate()
        {
            // If the game is slowed, enemy bullets will be slowed down
            if (SlowMoEffect.Instance.IsInSlowMo && origShooterTag != "Player")
            {
                float newSpeed = moveSpeed * SlowMoEffect.Instance.GetSlowDownFactor;
                rb.velocity = rb.transform.up * newSpeed * Time.deltaTime;
            }
            else
            {
                rb.velocity = rb.transform.up * moveSpeed * Time.deltaTime;
            }
        }

        // Interacts with other objects
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // If we hit something that shares the same tag as the orig shooter, we ignore them
            // Think of this as friendly fire
            if (collision.gameObject.tag != origShooterTag)
            {
                // In general, we only go in here if this projectile can interact with it aka, if it is in the array
                if (CheckIfTagIsInArray(collision.gameObject.tag))
                {
                    switch (collision.gameObject.tag)
                    {
                        case "Enemy":
                            // Kills Enemy
                            scoreSystem.IncrementScore(3);
                            Destroy(collision.gameObject);
                            break;
                        case "Player":
                            // If the game is in slow mo, the bullet does not do anything
                            if (SlowMoEffect.Instance.IsInSlowMo)
                            {
                                return;
                            }

                            // Else, we hurt player if they are not dodging
                            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
                            if (playerMovement.GetPlayerMovementState != MovementState.DODGING)
                            {
                                // If the player is not invincible or in slow mo, they take damage
                                if (PlayerHealth.Instance.IsInvincible == false)
                                {
                                    PlayerHealth.Instance.CurrentHealth -= 1;
                                }
                            }
                            break;
                        case "Projectile":
                            // If we shoot an enemy projectime, we add points as well as help refill the slowmo meter
                            if (origShooterTag == "Player" && collision.GetComponent<Projectile>().origShooterTag == "Enemy")
                            {
                                scoreSystem.IncrementScore(1);
                                SlowMoEffect.Instance.AddAdditionalTime(10f);
                            }
                            break;
                        case "Walls":
                            // All of these cases just cause the projectile to be destroyed (no special effects)
                            break;
                    }
                    DestroyItself();
                }
            }
        }

        // Destroys the projectile
        private void DestroyItself()
        {
            Destroy(this.gameObject);
        }

        // Helper method to check if a tag is in the array
        private bool CheckIfTagIsInArray(string checkedTag)
        {
            for (int index = 0; index < interactableTags.Length; ++index)
            {
                if (interactableTags[index] == checkedTag)
                {
                    return true;
                }
            }
            return false;
        }
    }

}