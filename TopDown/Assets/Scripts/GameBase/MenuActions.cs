/*  Menu Actions
 *  Dictates all menu controls when on a menu screen
 * 
 */

namespace Matt_UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using Matt_System;
    using TMPro;

    public class MenuActions : MonoBehaviour
    {
        [Header("Menu Objects")]
        public GameObject MainMenu_Blob;
        public GameObject GameOver_Blob;
        public GameObject Controls_Blob;

        [Header("Game Over Specifics")]
        public TextMeshProUGUI finalScore_text;

        private void Start()
        {
            if (GameManager.Instance.DidGameOver)
            {
                NavigateToGameOver();
            }
        }

        public void NavigatetoControls()
        {
            MainMenu_Blob.SetActive(false);
            GameOver_Blob.SetActive(false);
            Controls_Blob.SetActive(true);
        }

        public void NavigateToGame(int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }

        public void NavigateToTitle()
        {
            GameOver_Blob.SetActive(false);
            Controls_Blob.SetActive(false);
            MainMenu_Blob.SetActive(true);
        }

        public void NavigateToGameOver()
        {
            Controls_Blob.SetActive(false);
            MainMenu_Blob.SetActive(false);
            GameOver_Blob.SetActive(true);

            finalScore_text.text = "Final Score: " + GameManager.Instance.GetFinalScore.ToString();
            GameManager.Instance.DidGameOver = false;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }

}
