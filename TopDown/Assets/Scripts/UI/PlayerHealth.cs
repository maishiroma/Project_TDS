/*  Handles all of the Player's Health Tracking and whatnot
 * 
 */

namespace Matt_UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Matt_System;

    public class PlayerHealth : MonoBehaviour
    {
        // Static variables
        public static PlayerHealth Instance;

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

        [Header("Player Graphics")]
        [SerializeField]
        [Tooltip("Ref to the player's graphics")]
        private SpriteRenderer playerGraphics;
        [SerializeField]
        [Tooltip("Ref to the player's graphics")]
        private Sprite playerInvincible;

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
                StartCoroutine(ToggleHealthVisual());
            }
        }

        // Returns the invincible state
        public bool GetInvincible
        {
            get { return isInvincible; }
        }

        // Makes it so that there's only one of these in the game
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

            if (currHealth <= 0)
            {
                GameManager.Instance.GoToGameOver();
            }
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
            Sprite orig = playerGraphics.sprite;

            playerGraphics.sprite = playerInvincible;
            isInvincible = true;
            yield return new WaitForSeconds(invinciTime);
            playerGraphics.sprite = orig;
            isInvincible = false;
        }
    }
}


