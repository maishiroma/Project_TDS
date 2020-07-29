/*  Similar to how enemy spawning works, but this one spawns in items instead
 *  Inherits from Spawner
 * 
 */

namespace Matt_Gimmicks
{
    using Matt_Generics;
    using UnityEngine;

    public class ItemSpawner : Spawner
    {
        [Header("Spawn Time Vars")]
        [Tooltip("The minimun time it takes for an item to spawn")]
        public float minTimePossible;
        [Tooltip("The maximumm time it takes for an item to spawn")]
        public float maxTimePossible;

        // Private vars
        private float amountofTime = 0f;               // How much time has passed from the last spawn
        private float randomTimeSlot;                  // The time duration that it will take for an item to spawn 

        protected override void Start()
        {
            base.Start();
            randomTimeSlot = Random.Range(minTimePossible, maxTimePossible);
        }

        // Spawns healing items through RNG
        protected override void Update()
        {
            if (disableSpawn == false)
            {
                amountofTime += Time.deltaTime;
                if (amountofTime >= randomTimeSlot)
                {
                    // If the second random check doesn't pass, we reset the time to spawn another item and try again
                    if(Random.Range(1, 101) <= spawnRate)
                    {
                        disableSpawn = true;
                        if(NoOfActiveSpawned() < maxObjectSpawned)
                        {
                            if (spawnPool.childCount < maxObjectSpawned)
                            {
                                SmartSpawnObj(true);
                            }
                            else
                            {
                                ItemBehavior newItem = SmartSpawnObj(false).GetComponent<ItemBehavior>();
                                newItem.ResetItem();
                            }
                        }
                        Invoke("ReenableSpawning", 1f);
                    }
                    amountofTime = 0f;
                    randomTimeSlot = Random.Range(minTimePossible, maxTimePossible);
                }
            }
        }

        // Called in an invoke to reenable spawning of items
        private void ReenableSpawning()
        {
            disableSpawn = false;
        }
    }

}