/*  This is used to control game flow, as well as display certain information in different scenes
 * This is a static, persistent object, so any information that needs to persist between scenes can be saved in here
 */

namespace Matt_System
{
    using UnityEngine;
    using Matt_Gimmicks;
    using Matt_UI;

    public class GameManager : MonoBehaviour
    {
        // Static Vars
        public static GameManager Instance;

        // Private vars
        private bool didGameOver;           // Saves whether the game is in a game over state
        private int finalScore;             // Saves the final score amount the player has acheived.

        // Getter/Setters
        public bool DidGameOver
        {
            get { return didGameOver; }
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
        // Also saves the result score to a private variable
        public void GoToGameOver(int resultScore, SceneTransitioner currSceneTransistor)
        {
            if (didGameOver == false)
            {
                didGameOver = true;
                finalScore = resultScore;

                StartCoroutine(currSceneTransistor.TransitionToScene(0));
            }
        }
    }

}
