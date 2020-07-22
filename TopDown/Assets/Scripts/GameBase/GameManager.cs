/*  This is used to control game flow, as well as display certain information in different scenes
 * This is a static, persistent object, so any information that needs to persist between scenes can be saved in here
 */

namespace Matt_System
{
    using UnityEngine;
    using Matt_Gimmicks;
    using Matt_UI;
    using Matt_Movement;
    using Matt_Generics;
    using UnityEngine.SceneManagement;

    public class GameManager : MonoBehaviour
    {
        // Static Vars
        public static GameManager Instance;

        // Private vars
        // The following refs are cached for future refs
        private ScoreSystem currentScoreSystem;         // Saves the object that has the score system
        private SceneTransitioner sceneTransitioner;    // Saves the object that has the scene transitioner

        private bool didGameOver = false;               // Saves whether the game is in a game over state
        private int finalScore = 0;                     // Saves the final score amount the player has acheived.

        // Allows others to reference the score system, but CANNOT replace it!
        public ScoreSystem GetScoreSystem
        {
            get { return currentScoreSystem; }
        }

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
                Destroy(this.gameObject);
            }
        }

        // Due to OnLevelLoaded being deprecated going to be adding a delgate to this method instead.
        private void Start()
        {
            currentScoreSystem = FindObjectOfType<ScoreSystem>();
            sceneTransitioner = FindObjectOfType<SceneTransitioner>();

            SceneManager.sceneLoaded += onLevelLoad;
        }

        // Because this is a singleton, we need to refresh the following private vars
        // Each time we load in a level
        private void onLevelLoad(Scene scene, LoadSceneMode mode)
        {
            currentScoreSystem = FindObjectOfType<ScoreSystem>();
            sceneTransitioner = FindObjectOfType<SceneTransitioner>();

            // If we are not in the main menu (aka index 0, we reset this back to false
            if (scene.buildIndex > 0)
            {
                didGameOver = false;
            }
        }

        // This method stops all entities from moving
        // Such as projectiles, enemies and players
        private void StopEverything()
        {
            foreach (Entity currEntity in FindObjectsOfType<Entity>())
            {
                currEntity.StopMovement();
            }
        }

        public void RemvoeAllEnemies()
        {
            foreach(EnemyMovement currEnemy in FindObjectsOfType<EnemyMovement>())
            {
                currEnemy.StartCoroutine(currEnemy.InvokeDefeated());
            }
        }

        // Moves the game logic to the game over screen.
        // Also saves the result score to a private variable
        public void GoToGameOver()
        {
            if (didGameOver == false && currentScoreSystem != null && sceneTransitioner != null)
            {
                didGameOver = true;
                finalScore = currentScoreSystem.CurrentScore;

                StopEverything();
                StartCoroutine(sceneTransitioner.TransitionToScene(0));
            }
        }
    }

}
