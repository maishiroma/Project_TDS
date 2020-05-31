/*  This is used to control game flow
 * 
 */

namespace Matt_System
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Matt_Gimmicks;

    public class GameManager : MonoBehaviour
    {
        // Static Vars
        public static GameManager Instance;

        // Private vars
        private bool didGameOver;
        private int finalScore;

        public bool DidGameOver
        {
            get { return didGameOver; }
            set
            {
                didGameOver = value;
            }
        }

        public int GetFinalScore
        {
            get { return finalScore; }
        }

        // Singleton Statement
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        // Moves the game logic to the game over screen.
        public void GoToGameOver()
        {
            didGameOver = true;
            finalScore = FindObjectOfType<ScoreSystem>().CurrentScore;
            SceneManager.LoadScene(0);
        }

    }

}
