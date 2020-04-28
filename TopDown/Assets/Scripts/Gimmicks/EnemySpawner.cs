﻿/*  Spawns enemies in a given radius of an area
 * 
 */

namespace Matt_Gimmicks
{
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
        [Tooltip("How fast does this spawner spawn enemies")]
        public float spawnRate;

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

        // Sets up the random seed
        private void Start()
        {
            Random.InitState(Random.Range(1, 255));
        }

        private void Update()
        {
            // If the game is in slow mo, no enemies will be spawned
            if (SlowMoEffect.Instance.IsInSlowMo)
            {
                disableSpawn = true;
                StopCoroutine(SpawnEnemy());
            }
            else
            {
                disableSpawn = false;
            }

            // When the spawner can spawn enemies, it spawns them on a time interval
            // Once that time interval is met, an enemy will be spawned (potentally)
            if (disableSpawn == false)
            {
                amountofTime += Time.deltaTime;
                if (amountofTime >= spawnRate)
                {
                    amountofTime = 0;
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
            float xRanPos = Random.Range(leftMostRange, rightMostRange);
            float yRanPos = Random.Range(bottomMostRange, topMostRange);

            GameObject enemyCloud = Instantiate(enemySpawn_Cloud, new Vector2(xRanPos, yRanPos), Quaternion.identity, null);
            yield return new WaitForSeconds(1f);

            Destroy(enemyCloud);
            GameObject newEnemy = Instantiate(enemySpawn, new Vector2(xRanPos, yRanPos), Quaternion.identity, null);

            spawnedObjs.Add(newEnemy);
        }

        // Removes all null enemies in list
        private void CleanSpawnList()
        {
            disableSpawn = true;
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
    }

}