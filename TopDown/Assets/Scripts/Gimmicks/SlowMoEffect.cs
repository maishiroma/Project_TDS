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

        [Header("External Refs")]
        [Tooltip("The light the main game used")]
        public Light2D gameLighting;                    // Ref to the main lighting in the game
        [Tooltip("The light that the player has")]
        public Light2D playerLighting;                  // Ref to the light the player has
        [Tooltip("Ref to the UI gauge that acts as the timer")]
        public Slider slowMoGauge;                      // Ref to the UI slider that showcases the time

        [Header("Light Refs")]
        [Range(0.1f, 20f)]
        public float slowMoLightIntensity = 0.5f;       // How dim does the main light get when slow mo is active?

        // Private Vars that are exposed in editor only
        [Header("Slow Motion Vars")]
        [SerializeField]
        [Range(1f, 40f)]
        [Tooltip("How long are entities slowed down? Bigger Number = shorter timeframe")]
        private float slowDownLength = 2f;              // How long does the slow motion last?
        [SerializeField]
        [Range(0.01f, 0.99f)]
        [Tooltip("How potent is the slow down effect? Smaller number = higher effect")]
        private float slowDownFactor = 0.05f;           // How strong is the slow motion effect
        [SerializeField]
        [Range(1f, 40f)]
        [Tooltip("How long is the cooldown before using this again? Bigger Number = faster recoveryd")]
        private float slowDownCoolDown = 2f;            // How long is the cooldown from using slowdown?

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
                    //  When the game is set to slow mo, the player glows
                    playerLighting.enabled = true;
                    isInSlowMo = value;
                    currLightTime = 0f;
                    isReady = false;
                }
            }
        }

        // Retrieves the value for the slow down factor
        public float GetSlowDownFactor
        {
            get { return slowDownFactor; }
        }

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
                DontDestroyOnLoad(this.gameObject);
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
                    isInSlowMo = false;

                    // As well as revert the lighting back
                    playerLighting.enabled = false;
                    currLightTime = 0f;
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
                        isReady = true;
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
    }
}