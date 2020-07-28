/*  A class that interacts with the player when ran into
 * 
 */

namespace Matt_Gimmicks
{
    using Matt_Movement;
    using Matt_UI;
    using Matt_Generics;
    using UnityEngine;
    using System.Collections;

    // Determines what kind of item this is
    public enum ItemType
    {
        HEAL        // When the player runs into this, the player will recover health
    }

    public class ItemBehavior : MonoBehaviour
    {
        // Static vars
        private static AudioSource sfx;

        [Tooltip("The item Type. This will vary the behavior of this item")]
        public ItemType currentType;
        [Space]

        [Header("Graphics Var")]
        [Tooltip("Reference to the animations for the item")]
        public Animator itemAnims;

        [Header("Sound Vars")]
        [Tooltip("Source that plays this audio clip")]
        public SfxWrapper spawnItemSound;
        [Tooltip("Sound that plays when the iitem is gotten")]
        public SfxWrapper getItemSound;

        [Header("General Vars")]
        [Tooltip("The item's value, which is determined by the ItemType")]
        public int itemValue;
        [Tooltip("How long will it take before the item despawns")]
        public float itemDespawn;

        // Starts up the item's despawning behavior
        private void Start()
        {
            // Caches the sfx player for all items to use
            if(sfx == null)
            {
                sfx = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
            }

            StartCoroutine(InvokeDespawning(itemDespawn));
            spawnItemSound.PlaySoundClip(sfx);
        }

        // Depending on the item eeffect, this ccan do a multitue of things
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == "Player")
            {
                switch (currentType)
                {
                    case ItemType.HEAL:
                        // Heals the player by the item value
                        PlayerHealth playerHeath = collision.GetComponent<PlayerMovement>().playerHealth;
                        playerHeath.CurrentHealth += itemValue;
                        break;
                }
                getItemSound.PlaySoundClip(sfx);
                StartCoroutine(InvokeDespawning(0f));
            }
        }

        // Sets the animation for the item to despawning
        private void VisualDespawn()
        {
            itemAnims.SetBool("canDespawn", true);
        }

        // Removes the item from the game
        private void Despawn()
        {
            this.gameObject.SetActive(false);
        }

        // Public CoRoutine that allows for the itemm to despawn acccordingly
        public IEnumerator InvokeDespawning(float timeToDespawn)
        {
            yield return new WaitForSeconds(timeToDespawn);
            if (itemAnims.GetBool("canDespawn") == false)
            {
                VisualDespawn();
                yield return new WaitForSeconds(1f);
                Despawn();
            }
        }
    }

}