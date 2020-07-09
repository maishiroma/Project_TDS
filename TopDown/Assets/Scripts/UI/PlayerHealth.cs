/*  Handles all of the Player's Health Tracking and whatnot
 * 
 */

namespace Matt_UI
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using Matt_Movement;

    public class PlayerHealth : MonoBehaviour
    {
        [Header("Player Refs")]
        [Tooltip("Ref to the player movement component")]
        [SerializeField]
        private PlayerMovement playerMovement;

        // Private vars that are exposed in editor
        [Header("Positioning Vars")]
        [SerializeField]
        [Tooltip("Ref to the player")]
        private GameObject player;             // A ref to the player
        [SerializeField]
        [Tooltip("Ref to the main camera")]
        private Camera mainCamera;               // Reference to the main camera
        [SerializeField]
        [Tooltip("The offset that is applied to the UI when displayed")]
        private Vector2 offset;                  // Offset to position the UI

        [Header("UI Elements")]
        [SerializeField]
        [Tooltip("The Image that refers to the health's border")]
        private Image healthBorder;              // Ref to the border element of the health
        [SerializeField]
        [Tooltip("The Image that refers to the health's fill")]
        private Image healthFill;                // Ref to the fill element

        [Header("Health Values")]
        [Range(1, 4)]
        [SerializeField]
        [Tooltip("How much health does the player start out with?")]
        private int maxHealth = 4;              // How much health the player has
        [Range(1f, 5f)]
        [SerializeField]
        [Tooltip("How long does the player have after they take damage to be invulnerable?")]
        private float invinciTime = 2f;         // How long is the invincibility of the player?
        [Range(1f, 5f)]
        [SerializeField]
        [Tooltip("How long does the health UI show when it does show?")]
        private float showHealthTime = 2f;      // How long does the health show to the player?

        // Private vars that are NOT exposed
        private int currHealth;
        private bool isInvincible = false;

        // Getter/Setter
        public int CurrentHealth
        {
            get { return currHealth; }
            set
            {
                // Subtracting health
                if (currHealth > value)
                {
                    if (value < 0)
                    {
                        currHealth = 0;
                    }

                    // The player will be in hitstun for a little while
                    playerMovement.StartCoroutine(playerMovement.EnactHitStun());

                    // When the player takes damage, they get some invincibility frames
                    StartCoroutine(ToggleInvincibility());
                }
                else
                {
                    // Gaining Health
                    if (value > maxHealth)
                    {
                        currHealth = maxHealth;
                    }
                }
                currHealth = value;

                // We also briefy display the health meter when the player takes damage
                StartCoroutine(ToggleHealthVisual());
            }
        }

        // Returns the invincible state
        public bool IsInvincible
        {
            get { return isInvincible; }
        }

        // Sets the current health to be the max health
        private void Start()
        {
            currHealth = maxHealth;
            SetVisibiltyOfHealth(false);
        }

        // Keeps track of UI changes
        private void Update()
        {
            UpdateHealthVisual();
        }

        // Moves the UI to where the player is
        private void LateUpdate()
        {
            Vector2 uiToWorldSpace = mainCamera.WorldToScreenPoint(player.transform.position);
            gameObject.transform.position = uiToWorldSpace + offset;
        }

        // Updates the health visual to match the current health
        private void UpdateHealthVisual()
        {
            switch (currHealth)
            {
                case 4:
                    healthFill.fillAmount = 1f;
                    break;
                case 3:
                    healthFill.fillAmount = 0.75f;
                    break;
                case 2:
                    healthFill.fillAmount = 0.5f;
                    break;
                case 1:
                    healthFill.fillAmount = 0.25f;
                    break;
                case 0:
                    healthFill.fillAmount = 0f;
                    break;
            }
        }

        // Quickly toggles hiding/showing of health icon
        private void SetVisibiltyOfHealth(bool isShowing)
        {
            healthBorder.enabled = isShowing;
            healthFill.enabled = isShowing;
        }

        // Toggles the display of the health icon
        private IEnumerator ToggleHealthVisual()
        {
            SetVisibiltyOfHealth(true);
            yield return new WaitForSeconds(showHealthTime);
            SetVisibiltyOfHealth(false);
        }

        // Keeps track of the invincible duration
        private IEnumerator ToggleInvincibility()
        {
            playerMovement.entityGraphics.SetBool("is_damaged", true);
            isInvincible = true;
            yield return new WaitForSeconds(invinciTime);
            playerMovement.entityGraphics.SetBool("is_damaged", false);
            isInvincible = false;
        }
    }
}


