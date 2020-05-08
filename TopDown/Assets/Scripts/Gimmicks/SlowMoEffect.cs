/*  Global Script
 *  This controls the slow motion that occues in the game
 */

namespace Matt_Gimmicks
{
    using UnityEngine;
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

        [Header("Light Refs")]
        [Range(0.1f, 20f)]
        public float slowMoLightIntensity = 0.5f;       // How dim does the main light get when slow mo is active?

        [Header("Slow Motion Vars")]
        [SerializeField]
        [Range(1f, 20f)]
        [Tooltip("How long are entities slowed down?")]
        private float slowDownLength = 2f;              // How long does the slow motion last?
        [SerializeField]
        [Range(0.01f, 0.99f)]
        [Tooltip("How potent is the slow down effect? Smaller number = higher effect")]
        private float slowDownFactor = 0.05f;           // How strong is the slow motion effect

        private bool isInSlowMo = false;                // Is the game in slow motion?
        private float internalTimer = 0f;               // Used to keep track of slow mo duration and lighting cues
        private float origLightLevel;                   // Stores the original light level of the game

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
                    internalTimer = 0f;

                    isInSlowMo = value;
                }
            }
        }

        // Retrieves the value for the slow down factor
        public float GetSlowDownFactor
        {
            get { return slowDownFactor; }
        }

        // Retrieves the value for the duration of the slow down
        public float GetSlowDownLength
        {
            get { return slowDownLength; }
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
                gameLighting.intensity = Mathf.Lerp(origLightLevel, slowMoLightIntensity, Mathf.Clamp(internalTimer, 0f, 1f));

                internalTimer += Time.deltaTime;
                if (internalTimer >= slowDownLength)
                {
                    // Once we reach the duration length, we turn off slow motion
                    playerLighting.enabled = false;

                    isInSlowMo = false;
                    internalTimer = 0f;
                }
            }
            else
            {
                // When the game is returning to normal lighting, this gradually sets it back
                if (gameLighting.intensity < origLightLevel)
                {
                    internalTimer += Time.deltaTime;
                    gameLighting.intensity = Mathf.Lerp(slowMoLightIntensity, origLightLevel, Mathf.Clamp(internalTimer, 0f, 1f));
                }
            }
        }
    }
}