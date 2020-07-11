/*  Defines a basic scoring system for the player
 * 
 */

namespace Matt_Gimmicks
{
    using TMPro;
    using UnityEngine;

    public class ScoreSystem : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Ref to the score UI text")]
        public TextMeshProUGUI scoreDisplay;
        [Tooltip("Ref to the short message that displays to keep track of the score status")]
        public TextMeshProUGUI statusMessage;

        // Private vars
        private int currentScore = 0;

        // Getter/Setter
        public int CurrentScore
        {
            get { return currentScore; }
        }

        // Keeps track of the UI changes, depending on the state of the SlowMotion
        private void Update()
        {
            if (SlowMoEffect.Instance.IsInSlowMo)
            {
                statusMessage.text = "Double Points!!!";
            }
            else
            {
                statusMessage.text = "";
            }
            scoreDisplay.text = "Score: " + currentScore.ToString();
        }

        // Increments the score count
        // If we are in slowmo, points are doubled
        public void IncrementScore(int amount)
        {
            if (SlowMoEffect.Instance.IsInSlowMo)
            {
                currentScore += amount * 2;
            }
            else
            {
                currentScore += amount;
            }
        }
    }

}