/*  Global Script
 *  This controls the slow motion that occues in the game
 */

namespace Matt_Gimmicks
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Experimental.Rendering.LWRP;

    public class SlowMoEffect : MonoBehaviour
    {
        // Static Variables
        public static SlowMoEffect Instance;            // Only one of these cane be made at a time

        [Header("Sound Refs")]
        [Tooltip("Reference to the Background SFX that plays the game sounds")]
        public AudioSource sfx;
        [Tooltip("Plays when the player gets into slow motion")]
        public AudioClip slowMoStart;
        [Tooltip("Plays when slowm motion ends")]
        public AudioClip slowmoEnd;
        [Tooltip("Plays when the player's slow motion meter is full")]
        public AudioClip gaugeFull;

        // Private Vars that are exposed in editor only
        [Header("Gauge Refs")]
        [Tooltip("Ref to the UI gauge that acts as the timer")]
        public Slider slowMoGauge;                      // Ref to the UI slider that showcases the time
        [Space]
        [Tooltip("Bellow items are associated with the BG of the gauge, showcasing its various states")]
        public Image gaugeBG;                           // Ref to the BG of the gauge
        public Sprite fullGauge;                        // What the gauge will look like when ready
        public Sprite usingGauge;                       // What the gauge will look like during usages
        public Sprite refillGauge;                      // What the gauge will look like when refilling

        [Header("Light Refs")]
        [Range(0.1f, 20f)]
        [Tooltip("How dim is the main game light when slow motion is happening?")]
        public float slowMoLightIntensity = 0.5f;       // How dim does the main light get when slow mo is active?
        [Tooltip("The light the main game used")]
        public Light2D gameLighting;                    // Ref to the main lighting in the game
        [Tooltip("The light that the player has")]
        public Light2D playerLighting;                  // Ref to the light the player has

        [Header("Slow Motion Vars")]
        [Range(1f, 40f)]
        [Tooltip("How long are entities slowed down? Bigger Number = shorter timeframe")]
        public float slowDownLength = 2f;              // How long does the slow motion last?
        [Range(0.01f, 0.99f)]
        [Tooltip("How potent is the slow down effect? Smaller number = higher effect")]
        public float slowDownFactor = 0.05f;           // How strong is the slow motion effect
        [Range(1f, 40f)]
        [Tooltip("How long is the cooldown before using this again? Bigger Number = faster recovery")]
        public float slowDownCoolDown = 2f;            // How long is the cooldown from using slowdown?

        // Private vars that are hidden
        private bool isReady = true;                    // Indicates if the slowdown ability is ready
        private bool isInSlowMo = false;                // Is the game in slow motion?
        private float origLightLevel;                   // Stores the original light level of the game
        private float currLightTime;                    // The internal time used for transitions

        // Getters/Setters

        // Allows to get the value of whether the game is in slow motion
        // ONLY allows for the setting of this value to be true IFF the game is not in slow motion
        public bool IsInSlowMo
        {
            get { return isInSlowMo; }
            set
            {
                if (isInSlowMo == false && value == true)
                {
                    // When slow motion is active, we enable various flags to showcase the slow motion
                    sfx.PlayOneShot(slowMoStart);
                    gaugeBG.sprite = usingGauge;
                    currLightTime = 0f;
                    playerLighting.enabled = true;
                    isReady = false;
                    isInSlowMo = value;
                }
            }
        }

        // Retrieves the value for the slow down factor
        public float GetSlowDownFactor
        {
            get { return slowDownFactor; }
        }

        // Returns if the gauge is full and slow mo is not currrently active
        public bool isReadyToBeUsed
        {
            get { return isReady; }
        }

        // Prepares the Instanciation of this object to be a singleton
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        // Stores the original light in the private vars
        private void Start()
        {
            origLightLevel = gameLighting.intensity;
        }

        // Handles the time duration of the slow down effect
        private void Update()
        {
            if (isInSlowMo)
            {
                // The game light dims to emphasize the slow down
                currLightTime += Time.deltaTime;
                gameLighting.intensity = Mathf.Lerp(origLightLevel, slowMoLightIntensity, Mathf.Clamp(currLightTime, 0f, 1f));

                slowMoGauge.value -= (slowDownLength * Time.deltaTime);
                if (slowMoGauge.value <= 0)
                {
                    // Once we reach the duration length, we turn off slow motion
                    sfx.PlayOneShot(slowmoEnd);
                    isInSlowMo = false;

                    // As well as revert the lighting back
                    playerLighting.enabled = false;
                    currLightTime = 0f;

                    // And change the gauge to show that it is recharging
                    gaugeBG.sprite = refillGauge;
                }
            }
            else
            {
                // We refill on the gauge, which indicates how long the cooldown will be
                if (isReady == false)
                {
                    slowMoGauge.value += (slowDownCoolDown * Time.deltaTime);
                    if (slowMoGauge.value >= slowMoGauge.maxValue)
                    {
                        // Once the gauge is filled and no slowdown is active, this is ready to be used
                        sfx.PlayOneShot(gaugeFull);
                        isReady = true;
                        gaugeBG.sprite = fullGauge;
                    }
                }
                // When the game is returning to normal lighting, this gradually sets it back
                if (gameLighting.intensity < origLightLevel)
                {
                    currLightTime += Time.deltaTime;
                    gameLighting.intensity = Mathf.Lerp(slowMoLightIntensity, origLightLevel, Mathf.Clamp(currLightTime, 0f, 1f));
                }
            }
        }

        // Adds on additional time to slow motion, if able
        public void AddAdditionalTime(float amount)
        {
            slowMoGauge.value = Mathf.Clamp(slowMoGauge.value + amount, 0, slowMoGauge.maxValue);
        }
    }
}