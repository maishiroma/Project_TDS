/*  This outlines the basic requirements for a spawner, whicch randomley spawns objects into a given space
 * 
 */

namespace Matt_Generics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class Spawner : MonoBehaviour
    {
        [Header("General Vars")]
        [Tooltip("The prefab object to spawn")]
        public GameObject objectToSpawn;
        [Tooltip("How many objects can be spawned at a given time?")]
        public int maxObjectSpawned;
        [Tooltip("How often does this spawner spawn objects?")]
        [Range(0.1f, 99f)]
        public float spawnRate;
        [Tooltip("The layermask that holds all of the level terrain")]
        public LayerMask terrainLevel;

        [Header("Spawn Range Vars")]
        [Tooltip("The smallest x pos that enemies can spawn from")]
        public float leftMostRange;
        [Tooltip("The largest x pos that enemies can spawn from")]
        public float rightMostRange;
        [Tooltip("The smallest y pos that enemies can spawn from")]
        public float bottomMostRange;
        [Tooltip("The largest y pos that enemies can pawn from")]
        public float topMostRange;

        // Protected Vars
        protected List<GameObject> spawnedObjs = new List<GameObject>();      // A list of all of the spawned objects
        protected bool disableSpawn = false;                                  // Disables spawning objects alltogether

        // Inherited Methods that cannot be overriden

        // Removes all null objects in list
        // By default, this method will stop spawning from happening during the cleaning
        protected void CleanSpawnList(bool canDisableSpawning = true)
        {
            if(canDisableSpawning == true)
            {
                disableSpawn = true;
            }
            
            for (int currCount = 0; currCount < spawnedObjs.Count; ++currCount)
            {
                if (spawnedObjs[currCount] == null)
                {
                    spawnedObjs.RemoveAt(currCount);
                    currCount--;
                }
            }

            if(canDisableSpawning == true)
            {
                disableSpawn = false;
            }
        }

       

        // Inherited Methods that can be overriden

        // Getter/Setter for spawner
        public virtual bool IsSpawnDisabled
        {
            get { return disableSpawn; }
            set
            {
                // We only set/deset the spawning if the pased value is properly set
                if (disableSpawn == true && value == false)
                {
                    disableSpawn = value;
                }
                else if (disableSpawn == false && value == true)
                {
                    StopCoroutine(SpawnObject());
                    disableSpawn = value;
                }
            }
        }

        // Sets up the random seed
        // Can be overriden if needed
        protected virtual void Start()
        {
            Random.InitState(Random.Range(1, 255));
        }

        // Checks if the spawner can spawn something
        // If not, we will clean up the spawn list
        protected virtual void CheckIfSpawnable()
        {
            if (spawnedObjs.Count < maxObjectSpawned)
            {
                StartCoroutine(SpawnObject());
            }
            else
            {
                CleanSpawnList();
            }
        }

        // Spawns a given object
        protected virtual IEnumerator SpawnObject()
        {
            Collider2D[] collisions = new Collider2D[3];
            Vector2 spawnPos = new Vector2(Random.Range(leftMostRange, rightMostRange), Random.Range(bottomMostRange, topMostRange));

            // Makes sure that the spwned object does not spawn in a wall or anything solid
            int numb = Physics2D.OverlapPointNonAlloc(spawnPos, collisions, terrainLevel);
            while (numb > 0)
            {
                spawnPos = new Vector2(Random.Range(leftMostRange, rightMostRange), Random.Range(bottomMostRange, topMostRange));
                numb = Physics2D.OverlapPointNonAlloc(spawnPos, collisions, terrainLevel);
            }

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
            spawnedObjs.Add(newObject);
            yield return null;
        }

        // Methods that must be declared in all sub classes

        // Logic for spawning objects into the scene
        protected abstract void Update();
    }

}