/*  The projectile travels in a straight line from what it was fired from.
 *  When in contact with anything, it will be destroyd.
 * 
 */

namespace Matt_Movement
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering.LWRP;
    using Matt_Gimmicks;
    using Matt_System;
    using Matt_Generics;

    public class Projectile : MonoBehaviour
    {
        [Header("General Vars")]
        [Tooltip("The travel speed of the projectile. Shpuld be a high value: i.e. 1000")]
        [Range(500, 99999)]
        public float moveSpeed = 1000f;
        [Tooltip("An array of all of the tags that this projectile can interact with")]
        public string[] interactableTags;

        [Header("Physics Refs")]
        [Tooltip("Ref to the hitbox of this projectile")]
        public BoxCollider2D projectileTrigger;
        [Tooltip("Ref to the rigidbody of this projectile")]
        public Rigidbody2D projectileRb;

        [Header("Visual Refs")]
        [Tooltip("Ref to the render of the projectile")]
        public SpriteRenderer projectileRender;
        [Tooltip("Ref to the animations of the projectile")]
        public Animator projectileAnims;
        [Tooltip("Reference to the light surrounding the projectile")]
        public Light2D projectileLight;

        [Header("Sounds Refs")]
        [Tooltip("Ref to the audio source, where all projectile sounds come from")]
        public AudioSource sfxSource;
        [Tooltip("Sound when this projectile hits another projectile")]
        public SfxWrapper hitProjectile;
        [Tooltip("Sound when this projectile hits a solid surface")]
        public SfxWrapper hitWall;
        [Tooltip("Sound when this projectile hits a damageable target")]
        public SfxWrapper hitTarget;
        [Tooltip("Sound when this projectile is fired")]
        public SfxWrapper fireStart;

        // [HideInInspector]
        public string origShooterTag;       // The gameobject's tag that shot this

        // Private Vars
        private float timeToDestroy = 0f;

        // Called when the object becomes active
        private void OnEnable()
        {
            timeToDestroy = 0f;
            projectileAnims.SetInteger("hit_type", 0);

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
                this.gameObject.SetActive(false);
            }
        }

        // While active, it will move in a straight line
        private void FixedUpdate()
        {
            // If the game is slowed, enemy bullets will be slowed down
            if (SlowMoEffect.Instance.IsInSlowMo && origShooterTag != "Player")
            {
                float newSpeed = moveSpeed * SlowMoEffect.Instance.GetSlowDownFactor;
                projectileRb.velocity = projectileRb.transform.up * newSpeed * Time.deltaTime;
            }
            else
            {
                projectileRb.velocity = projectileRb.transform.up * moveSpeed * Time.deltaTime;
            }
        }

        // Interacts with other objects
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // If we hit something that shares the same tag as the orig shooter, we ignore them
            // Think of this as friendly fire
            if (!collision.gameObject.CompareTag(origShooterTag))
            {
                // In general, we only go in here if this projectile can interact with it aka, if it is in the array
                if (CheckIfTagIsInArray(collision.gameObject.tag))
                {
                    switch (collision.gameObject.tag)
                    {
                        case "Enemy":
                            // Kills Enemy
                            projectileAnims.SetInteger("hit_type", 2);
                            GameManager.Instance.GetScoreSystem.IncrementScore(3);

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
                                if (playerMovement.playerHealth.IsInvincible == false)
                                {
                                    PlaySoundAtSource("Player");
                                    playerMovement.playerHealth.CurrentHealth -= 1;
                                }
                                else
                                {
                                    PlaySoundAtSource("Wall");
                                }
                            }
                            projectileAnims.SetInteger("hit_type", 2);
                            break;
                        case "Projectile":
                            // If the player shoot an enemy projectile, we add points as well as help refill the slowmo meter
                            if (origShooterTag == "Player" && collision.GetComponent<Projectile>().origShooterTag == "Enemy")
                            {
                                GameManager.Instance.GetScoreSystem.IncrementScore(1);
                                SlowMoEffect.Instance.AddAdditionalTime(10f);
                            }
                            PlaySoundAtSource("Projectile");
                            projectileAnims.SetInteger("hit_type", 2);
                            break;
                        case "Item":
                            // If the projectile hits an item, it will be destroyed
                            ItemBehavior currItem = collision.GetComponent<ItemBehavior>();

                            PlaySoundAtSource("Projectile");
                            projectileAnims.SetInteger("hit_type", 2);
                            currItem.StartCoroutine(currItem.InvokeDespawning(0f));
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
            projectileRb.constraints = RigidbodyConstraints2D.FreezePosition;
            yield return new WaitForFixedUpdate();

            // Plays out the animation for getting destroyed
            projectileLight.enabled = false;
            projectileRender.flipY = true;

            // Then removes the object after X seconds
            yield return new WaitForSeconds(0.3f);
            this.gameObject.SetActive(false);
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
            switch (soundType)
            {
                case "Start":
                    fireStart.PlaySoundClip(sfxSource);
                    break;
                case "Player":
                case "Enemy":
                    hitTarget.PlaySoundClip(sfxSource);
                    break;
                case "Projectile":
                    hitProjectile.PlaySoundClip(sfxSource);
                    break;
                case "Walls":
                    hitWall.PlaySoundClip(sfxSource);
                    break;
            }
        }

        // Configs the projectile to be used
        public void SetupProjectile(Vector2 newPos, Quaternion newRotation, string shooterTag)
        {
            if(this.gameObject.activeInHierarchy == false)
            {
                projectileTrigger.enabled = true;
                projectileRb.constraints = RigidbodyConstraints2D.None;

                projectileLight.enabled = true;
                projectileRender.flipY = false;

                origShooterTag = shooterTag;
                this.gameObject.transform.position = newPos;
                this.gameObject.transform.rotation = newRotation;

                this.gameObject.SetActive(true);
            }
        }
    }

}