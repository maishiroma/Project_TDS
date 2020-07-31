# Base Classes
This piece of documentation will go over all of the standard classes that make up each of the GameObjects that inherit from them.

## Table of Contents
- [Entity](#Enntity)
- [SfxWrapper](#SfxWrapper)
- [Spawner](#Spawner)

## Entity
This makes up the base for all moving characters in the game. All objects that inherit from this have access to the following attributes:
- Graphics (Animator and SpriteRenderer)
- Standard Movement
- Atttacking via Projectiles

## SfxWrapper
This is a wrapper (get it?) class that stores useful information regarding sound effects. This class can also handle playing these sound effects through a passed in `AudioSource`. These are best used when:
- Playing a sound effect on any GameObject, especially on those that have varying audio levels

## Spawner
This is the base class for anything that spawns a specific GameObject in a given area. This has a few interesting mechanics attached to it when it comes to spawning new objects.

#### Methods that are inheritable (but cannot be overriden)
- `CleanSpawnList`: Method that removes all objects in a given spawn pool. Can be used to clean up a spawn pool if it is not being used anymore
- `SmartSpawnObj`: Method that handles all logic for making sure an object can spawn in a given area. Depending if passed in `true` or `false`, this can do two different behaviors:
    - Either spawn a new GameObject in a given spot
    - Reactivate an existing GameObject, resetting it so that it can be utilized again
    - The design choice on this is too give flexibility to the SpawnPool being used; it allows for the reuse of an existing object (if it exists) or spawns a new GameObject. The main purpose of this is to optimize the performance of the game.
- `NooOfActiveSpawned`: Returns the number of entities that are currrently spawned in the pool

#### Methods that are inheritable and can be ovverriden
- `IsSpawnDisabled`: Getter/Setter that dictates if the spawner is active or not
- `Start`: Called at the start of the Spawner's life