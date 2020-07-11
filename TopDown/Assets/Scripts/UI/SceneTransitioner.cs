/*  Handles all of the screen transitions (between main menu and game overs)
 * 
 */

namespace Matt_UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;

    public class SceneTransitioner : MonoBehaviour
    {
        [Header("General Vars")]
        [Tooltip("How fast does the transition go?")]
        [Range(0.01f, 5f)]
        public float transitionSpeed;

        [Header("Visuals Ref")]
        [Tooltip("Ref to the item that transitions between scenes")]
        public Image transitioner;

        // private vars
        private bool isDoingExit;           // Is the transition performing the exit variety?
        private AsyncOperation sceneLoader; //Holds a ref to the loaded scene operation

        // When the scene is first loaded, this makes sure whether the menu shows the game over screen or the normal screen
        private void Start()
        {
            // We set the fill to be full by default
            transitioner.fillAmount = 1f;
            StartBeginning();
        }

        // Handles the fadein/fadeout logic
        private void Update()
        {
            if (isDoingExit == false)
            {
                // Transition into scene
                if (transitioner.fillAmount <= 0.01f && transitioner.raycastTarget == true)
                {
                    transitioner.fillAmount = 0f;
                    transitioner.raycastTarget = false;
                }
                else if (transitioner.fillAmount >= 0f)
                {
                    transitioner.fillAmount -= transitionSpeed * Time.deltaTime;
                }
            }
            else
            {
                // Transition out of scene
                if (transitioner.fillAmount >= 0.99f)
                {
                    transitioner.fillAmount = 1f;
                }
                else if (transitioner.fillAmount <= 1f)
                {
                    transitioner.fillAmount += transitionSpeed * Time.deltaTime;
                }
            }
        }

        // Helper method that invokes the main transition outward.
        private void StartExit()
        {
            if (isDoingExit == false && transitioner.fillAmount == 0f)
            {
                isDoingExit = true;
                transitioner.raycastTarget = true;
            }
        }

        // Helper method that invokes the main transition inward
        private void StartBeginning()
        {
            if (transitioner.fillAmount == 1f)
            {
                isDoingExit = false;
                transitioner.raycastTarget = true;
            }
        }

        // Public method that handles moving from scene to scene
        public IEnumerator TransitionToScene(int sceneIndex)
        {
            StartExit();
            while (transitioner.fillAmount < 1f)
            {
                yield return null;
            }

            sceneLoader = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
            sceneLoader.allowSceneActivation = false;
            while (sceneLoader.progress < 0.9f)
            {
                yield return null;
            }
            sceneLoader.allowSceneActivation = true;
        }
    }
}