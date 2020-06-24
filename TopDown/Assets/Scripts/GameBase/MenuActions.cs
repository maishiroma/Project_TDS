/*  
 *  Dictates all menu controls when on a menu screen
 * 
 * 
 */

namespace Matt_UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Matt_System;
    using TMPro;

    public class MenuActions : MonoBehaviour
    {
        [Header("Menu Objects")]
        [Tooltip("Ref to the GameObject holding the Main Menu UI elements")]
        public GameObject MainMenu_Blob;
        [Tooltip("Ref to the GameObject holding the Game Over Menu UI elements")]
        public GameObject GameOver_Blob;
        [Tooltip("Ref to the GameObject holding the Controls UI elements")]
        public GameObject Controls_Blob;

        [Header("Game Over Specifics")]
        [Tooltip("Ref to the text that displays the player's score")]
        public TextMeshProUGUI finalScore_text;

        // When the menu appears, this changes whether the menu shows the game over screen or the normal screen
        private void Start()
        {
            if (GameManager.Instance.DidGameOver)
            {
                ToggleUIBlobs("GameOver");
            }
            else
            {
                // By default, we display the main menu
                ToggleUIBlobs("MainMenu");
            }
        }

        // Starts up the main game
        public void NavigateToGame(int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }

        // Quits the game
        public void QuitGame()
        {
            Application.Quit();
        }

        // Toggles the named ui element to be the only one on
        public void ToggleUIBlobs(string ui_kind)
        {
            MainMenu_Blob.SetActive(false);
            GameOver_Blob.SetActive(false);
            Controls_Blob.SetActive(false);

            switch (ui_kind)
            {
                case "MainMenu":
                    MainMenu_Blob.SetActive(true);
                    break;
                case "GameOver":
                    finalScore_text.text = "Total Score: " + GameManager.Instance.GetFinalScore.ToString();
                    GameOver_Blob.SetActive(true);
                    break;
                case "Controls":
                    Controls_Blob.SetActive(true);
                    break;
                default:
                    Debug.LogError("The specified name does not exist!");
                    break;
            }
        }
    }

}
