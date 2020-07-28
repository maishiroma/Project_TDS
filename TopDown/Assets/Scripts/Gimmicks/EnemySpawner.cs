/*  Spawns enemies in a given radius of an area
 *  Inherits from Spawner, applying a few extra mmethods unique to it
 * 
 */

namespace Matt_Gimmicks
{
    using Matt_Generics;
    using Matt_System;
    using System.Collections;
    using UnityEngine;

    public class EnemySpawner : Spawner
    {
        [Header("Difficulty Vars")]
        [Tooltip("Additional sccre needed to reach next round")]
        public int baseScoreAdddition = 100;
        [Tooltip("How many rounds per game will it take to increase enemy spawn and count?")]
        public int perRoundCheck = 3;
        [Tooltip("Time between rounds")]
        public float roundIntervals = 3f;
        [Space]
        [Tooltip("The current amount of enemies that can spawn. Clamped to be less than or equal to maxObjectSpawned")]
        public int currSpawnLimit = 3;
        [Tooltip("The modifier on how many enemies that can spawn at the moment")]
        public int currSpawnModdifier = 1;
        [Tooltip("The modifier on how much faster enemies can spawn?")]
        public float spawnRateChange = 0.3f;
        [Space]
        [Tooltip("The potental max number of enemy spawns that can happen in one spawn.")]
        public int simuSpawnMax = 5;
        [Tooltip("The modifier used to increase the number of enemies that spawn at once")]
        public int simuSpawnModifier = 1;
        
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

        // Adds the additional behavior of spawwning in multiple enemies at once
        protected override void CheckIfSpawnable()
        {
            // If we reached the max amount of objs spawned, we clean up the array
            if(spawnedObjs.Count >= currSpawnLimit)
            {
                CleanSpawnList();
            }
            
            int randSpawnNo = Random.Range(1, simuSpawnMax + 1);
            for (int iterator = randSpawnNo; iterator > 0; iterator--)
            {
                if (spawnedObjs.Count < currSpawnLimit)
                {
                    StartCoroutine(SpawnObject());
                }
                else
                {
                    // If we hit the cap, we just break out of the loop completly
                    break;
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
                // There is a limit to how small/large these values can get
                // If those limits are reached, the changes will not be made
                spawnRate = Mathf.Clamp(spawnRate - spawnRateChange, 1f, spawnRate);
                currSpawnLimit = Mathf.Clamp(currSpawnLimit + currSpawnModdifier, 1, maxObjectSpawned);
                simuSpawnMax = Mathf.Clamp(simuSpawnMax + simuSpawnModifier, 1, simuSpawnMax);
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