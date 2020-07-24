/*  Spawns enemies in a given radius of an area
 *  Inherits from Spawner, applying a few extra mmethods unique to it
 * 
 */

namespace Matt_Gimmicks
{
    using Matt_Generics;
    using Matt_System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemySpawner : Spawner
    {
        [Header("Difficulty Vars")]
        [Tooltip("Additional sccre needed to reach next round")]
        public int baseScoreAdddition = 100;
        [Tooltip("How many rounds per game will it take to increase enemy spawn and count?")]
        public int perRoundCheck = 3;
        [Tooltip("How much faster do enemies spawn?")]
        public float spawnRateChange = 0.3f;
        [Tooltip("The rate on the max number of enemies that go up?")]
        public int maxSpawnModifier = 1;
        [Tooltip("Time between rounds")]
        public float roundIntervals = 3f;

        // Private Vars
        private float amountofTime = 0f;               // How much time has passed from the last spawn
        private int toNextRoundScore;                  // Score needed to get to next round

        // Adds the additional functionality to set the next round requirement
        protected override void Start()
        {
            base.Start();
            toNextRoundScore = baseScoreAdddition;
        }

        // Handles spawn logic for enemies
        protected override void Update()
        {
            // If the player reached the sccore threshold, spawning is limited and the difficulty of the
            // spawner is increased
            if (GameManager.Instance.GetScoreSystem.CurrentScore >= toNextRoundScore && disableSpawn == false)
            {
                IsSpawnDisabled = true;
                StartCoroutine(IncreaseDifficulty());
            }
            // When the spawner can spawn enemies, it spawns them on a time interval
            // Once that time interval is met, an enemy will be spawned (potentally)
            else if (disableSpawn == false)
            {
                amountofTime += Time.deltaTime;
                if (amountofTime >= spawnRate)
                {
                    amountofTime = 0f;
                    CheckIfSpawnable();
                }
            }
        }

        // This gets callled automatically when the player's score enters a specifc threshold.
        private IEnumerator IncreaseDifficulty()
        {
            // We first incrememt up the spawner statuses
            // Depending on the per round check, the amount of enemies + spawn rate will gradually increase
            GameManager.Instance.StartCoroutine(GameManager.Instance.GetScoreSystem.UpdateRound());
            if (GameManager.Instance.GetScoreSystem.CurrentRound % perRoundCheck == 0)
            {
                spawnRate -= spawnRateChange;
                maxObjectSpawned += maxSpawnModifier;
            }

            // When a new round starts, all enemies are defeated automatically
            // Once we clean up the spawn list, the next round will kick off after X seconds
            toNextRoundScore += baseScoreAdddition;
            GameManager.Instance.RemvoeAllEnemies();
            CleanSpawnList(false);
            yield return new WaitForSeconds(roundIntervals);

            disableSpawn = false;
        }    
    }

}