/*  This outlines the basic requirements for a spawner, whicch randomley spawns objects into a given space
 * 
 */

namespace Matt_Generics
{
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
        [Tooltip("The pool used to spawn/pull objs from")]
        public Transform spawnPool;
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
            
            for (int currCount = 0; currCount < spawnPool.childCount; ++currCount)
            {
                GameObject currObj = spawnPool.GetChild(currCount).gameObject;
                if (currObj == null || currObj.activeInHierarchy == false)
                {
                    Destroy(currObj);
                    currCount--;
                }
            }

            if(canDisableSpawning == true)
            {
                disableSpawn = false;
            }
        }

        // Depending on the paramter, this object will either return a new spawned object, or an existing one from the pool
        protected GameObject SmartSpawnObj(bool spawnNew = true)
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

            if (spawnNew == true)
            {
                // Creates a new object and asspciates it with the spawn pool
                return Instantiate(objectToSpawn, spawnPos, Quaternion.identity, spawnPool);
            }
            else
            {
                // We look for an existing, deactive spawned object in the pool
                // and if we find one at random, we reset its pos and rotation to be like it was 
                int randObjInPool = Random.Range(0, spawnPool.childCount);
                while (spawnPool.GetChild(randObjInPool).gameObject.activeInHierarchy != false)
                {
                    randObjInPool = Random.Range(0, spawnPool.childCount);
                }
                GameObject revivedObj = spawnPool.GetChild(randObjInPool).gameObject;
                revivedObj.transform.position = spawnPos;
                revivedObj.transform.rotation = Quaternion.identity;
                revivedObj.SetActive(true);

                return revivedObj;
            }
        }

        // Returns the number of objs that are spawned (aka, being active)
        protected int NoOfActiveSpawned()
        {
            int total = 0;
            for (int currCount = 0; currCount < spawnPool.childCount; ++currCount)
            {
                GameObject currObj = spawnPool.GetChild(currCount).gameObject;
                if (currObj.activeInHierarchy == true)
                {
                    total++;
                }
            }
            return total;
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

        // Methods that must be declared in all sub classes

        // Logic for spawning objects into the scene
        protected abstract void Update();
    }

}