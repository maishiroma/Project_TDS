/*  When the specified gameObjects collide into this, they will take damage
 */

namespace Matt_Gimmicks
{
    using Matt_Movement;
    using Matt_Generics;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DamageOnContact : MonoBehaviour
    {
        // Static vars
        private static AudioSource sfx;

        [Header("General Vars")]
        [Tooltip("Whoever matches with this tag will interact with this script.")]
        public string interactWithTag;
        [Tooltip("Sound to play when colliding with said object")]
        public SfxWrapper damageSound;

        // Caches the sfx source for all users of this class
        private void Start()
        {
            if (sfx == null)
            {
                sfx = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
            }
        }

        // Depending on the target tag, this script will interact with them differently
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(interactWithTag))
            {
                switch (collision.gameObject.tag)
                {
                    case "Player":
                        // If the player is not invincible or in slow mo, they will take damage
                        PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
                        if (playerMovement.playerHealth.IsInvincible == false && !SlowMoEffect.Instance.IsInSlowMo)
                        {
                            damageSound.PlaySoundClip(sfx);
                            playerMovement.playerHealth.CurrentHealth -= 1;
                        }
                        break;
                }
            }
        }
    }

}