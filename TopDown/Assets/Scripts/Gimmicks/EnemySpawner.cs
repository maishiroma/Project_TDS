/*  Spawns enemies in a given radius of an area
 * 
 */

namespace Matt_Gimmicks
{
    using Matt_System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EnemySpawner : MonoBehaviour
    {

        [Header("Outside Refs Vars")]
        [Tooltip("The prefab object to spawn")]
        public GameObject enemySpawn;
        [Tooltip("A preview of the enemy being spawned")]
        public GameObject enemySpawn_Cloud;

        [Header("General Vars")]
        [Tooltip("How many enemys can be spawned at a given time?")]
        public int maxEnemySpawn;
        [Tooltip("How fast does this spawner spawn enemies?")]
        [Range(0.1f, 10f)]
        public float spawnRate;
        [Tooltip("How fast does enemy cloud spawns last (must be smaller than spawn rate)?")]
        [Range(0.1f, 10f)]
        public float cloudDuration;
        [Tooltip("The layermask that holds all of the level terrain")]
        public LayerMask terrainLevel;

        [Header("Difficulty Vars")]
        [Tooltip("Additional sccre needed to reach next round")]
        public int baseScoreAdddition = 100;
        [Tooltip("How many rounds per game will it take to increase enemy spawn and count?")]
        public int perRoundCheck = 3;
        [Tooltip("How much faster do enemies spawn?")]
        public float spawnRateChange = 0.3f;
        [Tooltip("The rate on the max number of enemies that go up?")]
        public int maxSpawnModifier = 1;

        [Header("Random Range Vars")]
        [Tooltip("The smallest x pos that enemies can spawn from")]
        public float leftMostRange;
        [Tooltip("The largest x pos that enemies can spawn from")]
        public float rightMostRange;
        [Tooltip("The smallest y pos that enemies can spawn from")]
        public float bottomMostRange;
        [Tooltip("The largest y pos that enemies can pawn from")]
        public float topMostRange;

        // Private Vars
        private List<GameObject> spawnedObjs = new List<GameObject>();      // A list of all of the spawned objects
        private bool disableSpawn = false;                                  // Disables spawning objects alltogether
        private float amountofTime = 0f;                                    // How much time has passed from the last spawn
        private int toNextRoundScore;                                       // Score needed to get to next round

        public bool IsSpawnDisabled
        {
            get { return disableSpawn; }
            set
            {
                // We only set/deset the spawning if the pased value is properly set
                if (disableSpawn == true && value == false)
                {
                    disableSpawn = value;
                }
                else if(disableSpawn == false && value == true)
                {
                    StopCoroutine(SpawnEnemy());
                    disableSpawn = value;
                }
            }
        }

        // Sets up the random seed
        private void Start()
        {
            Random.InitState(Random.Range(1, 255));
            toNextRoundScore = baseScoreAdddition;
        }

        // Makes sure that the spawn rate and the enemyCloudDuration are not clashing
        private void OnValidate()
        {
            if (spawnRate <= cloudDuration)
            {
                Debug.LogError("Spawn Rate must be higher than enemyCloudDuration!");
            }
        }

        // Handles spawn logic
        private void Update()
        {
            if (GameManager.Instance.GetScoreSystem.CurrentScore >= toNextRoundScore && disableSpawn == false)
            {
                IsSpawnDisabled = true;
                StartCoroutine(IncreaseDifficulty());
            }

            // When the spawner can spawn enemies, it spawns them on a time interval
            // Once that time interval is met, an enemy will be spawned (potentally)
            if (disableSpawn == false)
            {
                amountofTime += Time.deltaTime;
                if (amountofTime >= spawnRate)
                {
                    amountofTime = 0f;
                    CheckIfSpawnable();
                }
            }
        }

        // Checks if the spawner can spawn something
        // If not, we will clean up the spawn list
        private void CheckIfSpawnable()
        {
            if (spawnedObjs.Count < maxEnemySpawn)
            {
                StartCoroutine(SpawnEnemy());
            }
            else
            {
                CleanSpawnList();
            }
        }

        // Spawns an enemy at a random position
        private IEnumerator SpawnEnemy()
        {
            Collider2D[] collisions = new Collider2D[3];
            Vector2 spawnPos = new Vector2(Random.Range(leftMostRange, rightMostRange), Random.Range(bottomMostRange, topMostRange));

            // Makes sure that the enemy does not spawn in a wall or anything solid
            int numb = Physics2D.OverlapPointNonAlloc(spawnPos, collisions, terrainLevel);
            while (numb > 0)
            {
                spawnPos = new Vector2(Random.Range(leftMostRange, rightMostRange), Random.Range(bottomMostRange, topMostRange));
                numb = Physics2D.OverlapPointNonAlloc(spawnPos, collisions, terrainLevel);
            }

            // If the size of the cloud is off, check the animation on the cloud
            GameObject enemyCloud = Instantiate(enemySpawn_Cloud, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(cloudDuration);
            Destroy(enemyCloud);

            GameObject newEnemy = Instantiate(enemySpawn, spawnPos, Quaternion.identity);
            spawnedObjs.Add(newEnemy);
            yield return null;
        }

        // Removes all null enemies in list
        private void CleanSpawnList()
        {
            IsSpawnDisabled = true;
            for (int currCount = 0; currCount < spawnedObjs.Count; ++currCount)
            {
                if (spawnedObjs[currCount] == null)
                {
                    spawnedObjs.RemoveAt(currCount);
                    currCount--;
                }
            }
            disableSpawn = false;
        }
    
        // This gets callled automatically when the player's score enters a specifc threshold.
        private IEnumerator IncreaseDifficulty()
        {
            // When a new round starts, all enemies are defeated
            toNextRoundScore += baseScoreAdddition;
            GameManager.Instance.StartCoroutine(GameManager.Instance.GetScoreSystem.UpdateRound());
            GameManager.Instance.RemvoeAllEnemies();
            yield return new WaitForSeconds(3f);

            // Depending on the per round check, the amount of enemies + spawn rate will gradually increase
            if(GameManager.Instance.GetScoreSystem.CurrentRound % perRoundCheck == 0)
            {
                if(spawnRate - spawnRateChange >= cloudDuration)
                {
                    spawnRate -= spawnRateChange;
                }
                maxEnemySpawn += maxSpawnModifier;
            }
            
            // We clean up the spawn list, which will then kick off the next round
            CleanSpawnList();
            yield return new WaitForFixedUpdate();
            
        }    
    }

}