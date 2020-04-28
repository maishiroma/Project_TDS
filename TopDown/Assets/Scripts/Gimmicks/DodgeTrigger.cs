/*  Dodge Mechanic
 *  This controls the dodge mechanic that the game will revolve around
 *  Essentially, if the entity does the dodge (which indicated by the hitbox shown) and an attack hits the dodge
 *  SlowMo will be activated
 *
 */

namespace Matt_Gimmicks
{
    using UnityEngine;
    using Matt_Movement;

    public class DodgeTrigger : MonoBehaviour
    {
        [Header("Outside Refs")]
        [Tooltip("The hitbox used to determine the sweet spot of the dodge")]
        public BoxCollider2D hitTrigger;

        // When the dodge is activated, this hitboc will be looking for any attacks coming its way
        // When an attack does come through, slowmo will be acheived.
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Projectile"))
            {
                Projectile currProjectile = collision.gameObject.GetComponent<Projectile>();

                if (currProjectile.origShooterTag == "Enemy" && SlowMoEffect.Instance.IsInSlowMo == false)
                {
                    SlowMoEffect.Instance.IsInSlowMo = true;
                }
            }
        }

    }
}