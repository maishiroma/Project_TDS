/*  Global Script
 *  This controls the slow motion that occues in the game
 */

namespace Matt_Gimmicks
{
    using UnityEngine;

    public class SlowMoEffect : MonoBehaviour
    {
        // Static Variables
        public static SlowMoEffect Instance;            // Only one of these cane be made at a time

        [SerializeField]
        [Range(1f, 3f)]
        private float slowDownLength = 2f;              // How long does the slow motion last?
        [SerializeField]
        [Range(0.01f, 0.1f)]
        private float slowDownFactor = 0.05f;           // How strong is the slow motion effect
        private bool isInSlowMo = false;                // Is the game in slow motion?
        private float timeSinceSlowDown = 0f;           // The amount of time that passed while the game is in slow motion

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
                    isInSlowMo = value;
                }
            }
        }

        // Retrieves the value for the slow down factor
        public float GetSlowDownFactor
        {
            get { return slowDownFactor; }
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

        // Handles the time duration of the slow down effect
        private void Update()
        {
            if (isInSlowMo)
            {
                timeSinceSlowDown += Time.deltaTime;
                if (timeSinceSlowDown >= slowDownLength)
                {
                    // Once we reach the duration length, we turn off slow motion
                    isInSlowMo = false;
                    timeSinceSlowDown = 0f;
                }
            }
        }
    }
}