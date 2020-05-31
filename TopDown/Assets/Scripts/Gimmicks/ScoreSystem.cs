namespace Matt_Gimmicks
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class ScoreSystem : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI scoreDisplay;
        public TextMeshProUGUI statusMessage;

        // Private vars
        private int currentScore;

        public int CurrentScore
        {
            get { return currentScore; }
        }


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

        public void IncrementScore(int amount)
        {
            // If we are in slowmo, points are doubled
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