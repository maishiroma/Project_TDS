/*  UI script that performs the UI display for the slo motion effect
 *
 *  When the player is in slow mo, all enemies will have a clock effect on them. When slo motion is done, the clock will fade
 *  away
 *
 */

namespace Matt_UI
{
    using UnityEngine;
    using Matt_Gimmicks;
    using UnityEngine.Experimental.Rendering.LWRP;

    // This script should be on the gameobject that also has the animator and sprite render of the timer ui
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class TimerIcon : MonoBehaviour
    {
        [Header("External Refs")]
        [Tooltip("The light emitting from the timer")]
        public Light2D timerLight;

        // Private vars
        private Animator slowMoAnimatior;        // The animatior that is on the gameObject controlling the sprite
        private SpriteRenderer slowMoSprite;     // The render that will be displaying the sprites
        private bool hasBeenActivated;          // Is the script active?

        // Sets up all of the internal components
        private void Awake()
        {
            slowMoAnimatior = gameObject.GetComponent<Animator>();
            slowMoSprite = gameObject.GetComponent<SpriteRenderer>();
        }

        // At the start of the game, hides the ui
        private void Start()
        {
            hasBeenActivated = false;
            slowMoAnimatior.SetBool("IsInSlowMo", false);
            slowMoSprite.enabled = false;
        }

        // Handles the logic for showcasing the slow mo on the enemy
        private void Update()
        {
            if (SlowMoEffect.Instance.IsInSlowMo)
            {
                gameObject.transform.rotation = Quaternion.identity;
                ShowSlowMoEffect();
            }
            else
            {
                HideSlowMoEffect();
            }
        }

        // Shows the Timer sprite on the enemy
        private void ShowSlowMoEffect()
        {
            if (hasBeenActivated == false)
            {
                hasBeenActivated = true;
                timerLight.enabled = true;
                slowMoSprite.enabled = true;
                slowMoAnimatior.SetBool("IsInSlowMo", true);
            }
        }

        // Hides the timer sprite on the enemy
        private void HideSlowMoEffect()
        {
            if (hasBeenActivated == true)
            {
                hasBeenActivated = false;
                timerLight.enabled = false;
                slowMoAnimatior.SetBool("IsInSlowMo", false);
                slowMoSprite.enabled = false;
            }
        }
    }

}