/*  This class will be the baseclass for all entities in the game, which includes the player and enemies
 *  This class will hold a lot of the basic movement for them, variables and helper methods.
 *  
 *  As such, this class cannot be used on its own: however, deriving from it is fairly easy.
 */

namespace Matt_Generics
{
    using UnityEngine;
    using System.Collections;
    using Matt_Movement;

    public abstract class Entity : MonoBehaviour
    {
        [Header("Graphical Variables")]
        [Tooltip("Reference to the entity's animatior")]
        public Animator entityGraphics;             // The animatior that is on the gameObject controlling the sprite
        [Tooltip("Reference to the entity's sprite renderer")]
        public SpriteRenderer entityRenderer;       // The render that will be displaying the sprites

        [Header("Movement Variables")]
        [Tooltip("The speed of the character")]
        [Range(1f, 30f)]
        public float moveSpeed;
        [Tooltip("The speed of which the character can attack")]
        [Range(0.1f, 5f)]
        public float attackRate;

        [Header("Outside References")]
        [Tooltip("The projectile the character will use when attacking")]
        public Projectile entityProjectile;
        [Tooltip("Reference to the entity's front. Used to calculate where the firing will shoot from.")]
        public Transform frontOfEntity;

        // Protected variables
        protected Rigidbody2D entityRb;         // The rigidbody of the entity
        protected bool hasFired = false;        // Has the entity fired a projectile?

        // Makes sure that all public variables are properly set
        private void OnValidate()
        {
            if (moveSpeed < 1f)
            {
                moveSpeed = 1f;
            }
            else if (moveSpeed > 30f)
            {
                moveSpeed = 30f;
            }

            if (attackRate < 0.1f)
            {
                attackRate = 0.1f;
            }
            else if (attackRate > 5f)
            {
                attackRate = 5f;
            }
        }

        // Sets up all of the components
        private void Awake()
        {
            entityRb = GetComponent<Rigidbody2D>();
        }

        // Methods that cannot be overriden, but are shared in all subclasses
        // Shoots a projectile that the entity has forward from the given forward position
        protected IEnumerator ShootProjectile(Vector2 posToShootAt)
        {
            if (hasFired == false)
            {
                entityGraphics.SetBool("is_attacking", true);
                Quaternion shotRotation = Quaternion.FromToRotation(Vector2.up, posToShootAt - entityRb.position);
                Projectile bulletShot = Instantiate(entityProjectile, frontOfEntity.position, shotRotation, null);
                bulletShot.origShooterTag = gameObject.tag;

                hasFired = true;
                yield return new WaitForSeconds(attackRate);
                hasFired = false;
                entityGraphics.SetBool("is_attacking", false);
            }
            yield return null;
        }

        // Methods that have overridenable defaults
        // Rotates the entity to face the specific point of reference
        protected virtual void OrientateEntity(Vector2 objToLookAt)
        {
            Vector2 lookPos = objToLookAt - entityRb.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg - 90f;
            entityRb.rotation = angle;
        }

        // Methods that need to be defined in subclasses
        // Defines how the entity moves
        protected abstract void MoveEntity();


        // Methods that all classes can utilize that are public
        // Helper function that stops all player movement
        // Can be overriden if needed
        public virtual void StopMovement()
        {
            entityGraphics.SetBool("is_moving", false);
            entityRb.velocity = Vector2.zero;
            entityRb.angularVelocity = 0f;
        }

    }

}

