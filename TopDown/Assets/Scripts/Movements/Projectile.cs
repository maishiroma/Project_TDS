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
    using System.Collections;
    using UnityEngine.Experimental.Rendering.LWRP;

    public class Projectile : MonoBehaviour
    {
        // Private Static Vars
        private static ScoreSystem scoreSystem;
        private static AudioSource sfx_player;
        private static AudioSource sfx_enemy;

        [Header("Sounds Refs")]
        [Tooltip("Sound when this projectile hits another projectile")]
        public AudioClip hitProjectile;
        [Tooltip("Sound when this projectile hits a solid surface")]
        public AudioClip hitWall;
        [Tooltip("Sound when this projectile hits a damageable target")]
        public AudioClip hitTarget;
        [Tooltip("Sound when this projectile is fired")]
        public AudioClip fireStart;

        [Header("Visual Refs")]
        [Tooltip("Ref to the render of the projectile")]
        public SpriteRenderer projectileRender;
        [Tooltip("Ref to the animations of the projectile")]
        public Animator projectileAnims;
        [Tooltip("Reference to the light surrounding the projectile")]
        public Light2D projectileLight;

        [Header("Physics Refs")]
        [Tooltip("Ref to the hitbox of this projectile")]
        public BoxCollider2D projectileTrigger;

        [Header("General Vars")]
        [Tooltip("The travel speed of the projectile. Shpuld be a high value: i.e. 1000")]
        [Range(500, 99999)]
        public float moveSpeed = 1000f;
        [Tooltip("An array of all of the tags that this projectile can interact with")]
        public string[] interactableTags;

        // [HideInInspector]
        public string origShooterTag;       // The gameobject's tag that shot this

        // Private Vars
        private Rigidbody2D rb;
        private float timeToDestroy = 0f;

        // Sets up all of the components
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Efficiently stores data on constantly referenced things that the class uses
        private void Start()
        {
            if (scoreSystem == null)
            {
                scoreSystem = FindObjectOfType<ScoreSystem>();
            }

            if (origShooterTag == "Player")
            {
                if (sfx_player == null)
                {
                    sfx_player = GameObject.FindGameObjectWithTag("SFX_Player").GetComponent<AudioSource>();
                }
            }
            else
            {
                if (sfx_enemy == null)
                {
                    sfx_enemy = GameObject.FindGameObjectWithTag("SFX_Env").GetComponent<AudioSource>();
                }

            }

            // When the projectile is made, it fires a sound efect
            PlaySoundAtSource("Start");
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
                Destroy(this.gameObject);
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
                            projectileAnims.SetInteger("hit_type", 2);
                            scoreSystem.IncrementScore(3);

                            EnemyMovement currEnemy = collision.gameObject.GetComponent<EnemyMovement>();
                            currEnemy.StartCoroutine(currEnemy.InvokeDefeated());
                            PlaySoundAtSource("Enemy");
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
                                    PlaySoundAtSource("Player");
                                    PlayerHealth.Instance.CurrentHealth -= 1;
                                }
                                else
                                {
                                    PlaySoundAtSource("Wall");
                                }
                            }
                            projectileAnims.SetInteger("hit_type", 2);
                            break;
                        case "Projectile":
                            // If we shoot an enemy projectime, we add points as well as help refill the slowmo meter
                            if (origShooterTag == "Player" && collision.GetComponent<Projectile>().origShooterTag == "Enemy")
                            {
                                PlaySoundAtSource("Projectile");
                                projectileAnims.SetInteger("hit_type", 2);
                                scoreSystem.IncrementScore(1);
                                SlowMoEffect.Instance.AddAdditionalTime(10f);
                            }
                            break;
                        case "Walls":
                            // All of these cases just cause the projectile to be destroyed (no special effects)
                            PlaySoundAtSource("Walls");
                            projectileAnims.SetInteger("hit_type", 1);
                            break;
                    }

                    // If for some reason it skipped the case, the projectile will default change to 2
                    if (projectileAnims.GetInteger("hit_type") == 0)
                    {
                        PlaySoundAtSource("Wall");
                        projectileAnims.SetInteger("hit_type", 2);
                    }
                    StartCoroutine(DestroyItself());
                }
            }
        }

        // Destroys the projectile while
        private IEnumerator DestroyItself()
        {
            // Gives time for the physics to kick in
            projectileTrigger.enabled = false;
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
            yield return new WaitForFixedUpdate();

            // Plays out the animation for getting destroyed
            projectileLight.enabled = false;
            projectileRender.flipY = true;
            projectileRender.transform.localScale = new Vector3(8, 8, 0);

            // Then removes the object after X seconds
            yield return new WaitForSeconds(0.3f);
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

        // Helper method that properly plays our sound from the correct source
        private void PlaySoundAtSource(string soundType)
        {
            if (origShooterTag == "Player")
            {
                switch (soundType)
                {
                    case "Start":
                        sfx_player.PlayOneShot(fireStart);
                        break;
                    case "Enemy":
                        sfx_player.PlayOneShot(hitTarget);
                        break;
                    case "Projectile":
                        sfx_player.PlayOneShot(hitProjectile);
                        break;
                    case "Walls":
                        sfx_player.PlayOneShot(hitWall);
                        break;
                }
            }
            else
            {
                switch (soundType)
                {
                    case "Start":
                        sfx_enemy.PlayOneShot(fireStart);
                        break;
                    case "Player":
                        sfx_enemy.PlayOneShot(hitTarget);
                        break;
                    case "Projectile":
                        sfx_enemy.PlayOneShot(hitProjectile);
                        break;
                    case "Walls":
                        sfx_enemy.PlayOneShot(hitWall);
                        break;
                }
            }
        }
    }

}