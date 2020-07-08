/*  
 *  Dictates all menu controls when on a menu screen
 */

namespace Matt_UI
{
    using UnityEngine;
    using Matt_System;
    using TMPro;

    public class MenuActions : MonoBehaviour
    {
        [Header("Music Objects")]
        [Tooltip("The source of BGM music playing")]
        public AudioSource bgm;
        [Tooltip("Plays during the normal menus")]
        public AudioClip mainmenuMusic;
        [Tooltip("Plays when the player gets a Game Over")]
        public AudioClip gameoverMusic;

        [Header("Menu Objects")]
        [Tooltip("Ref to the GameObject holding the Main Menu UI elements")]
        public GameObject MainMenu_Blob;
        [Tooltip("Ref to the GameObject holding the Game Over Menu UI elements")]
        public GameObject GameOver_Blob;
        [Tooltip("Ref to the GameObject holding the Controls UI elements")]
        public GameObject Controls_Blob;
        [Tooltip("Ref to the screen transitioner")]
        public SceneTransitioner sceneTransitioner;

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

            // When we load up the screen, we have the music ready to play
            bgm.Play();
        }

        // Starts up the main game
        public void NavigateToGame(int levelIndex)
        {
            StartCoroutine(sceneTransitioner.TransitionToScene(levelIndex));
        }

        // Quits the game
        public void QuitGame()
        {
            Application.Quit();
        }

        // Toggles the named ui element to be the only one on
        // We also specifify specific music to play depending on where we are
        public void ToggleUIBlobs(string ui_kind)
        {
            MainMenu_Blob.SetActive(false);
            GameOver_Blob.SetActive(false);
            Controls_Blob.SetActive(false);

            switch (ui_kind)
            {
                case "MainMenu":
                    if (bgm.clip != mainmenuMusic)
                    {
                        bgm.clip = mainmenuMusic;
                    }
                    MainMenu_Blob.SetActive(true);
                    break;
                case "GameOver":
                    if (bgm.clip != gameoverMusic)
                    {
                        bgm.clip = gameoverMusic;
                    }
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
