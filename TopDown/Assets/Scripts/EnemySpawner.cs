using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Matt_Gimmicks {
    public class EnemySpawner : MonoBehaviour {

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
        private bool disableSpawn;  // Disables spawning objects alltogether

        // Sets up the random seed
        private void Start() {
            Random.InitState(Random.Range(1, 255));

            // Due to the behavior of the spawner, the loop will be done via an InvokeRepeating
            InvokeRepeating("CheckIfSpawnable", spawnRate, spawnRate);
            InvokeRepeating("CleanSpawnList", spawnRate * 5f, spawnRate * 5f);
        }

        // Checks if the spawner can spawn something
        private void CheckIfSpawnable() {
            if(disableSpawn == false && spawnedObjs.Count < maxEnemySpawn)  {
                StartCoroutine(SpawnEnemy());
            }
        }

        // Spawns an enemy at a random position
        private IEnumerator SpawnEnemy() {
            float xRanPos = Random.Range(leftMostRange, rightMostRange);
            float yRanPos = Random.Range(bottomMostRange, topMostRange);

            GameObject enemyCloud = Instantiate(enemySpawn_Cloud, new Vector2(xRanPos, yRanPos), Quaternion.identity, null);
            yield return new WaitForSeconds(1f);

            Destroy(enemyCloud);
            GameObject newEnemy = Instantiate(enemySpawn, new Vector2(xRanPos, yRanPos), Quaternion.identity, null);

            spawnedObjs.Add(newEnemy);
        }

        // Removes all null enemies in list
        private void CleanSpawnList() {
            disableSpawn = true;
            for(int currCount = 0; currCount < spawnedObjs.Count; ++currCount) {
                if(spawnedObjs[currCount] == null) {
                    spawnedObjs.RemoveAt(currCount);
                    currCount--;
                }
            }
            disableSpawn = false;
        }
    }

}