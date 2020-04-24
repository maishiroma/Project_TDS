namespace Matt_Gimmicks
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Matt_Movement;

    public class PlayerDodge : MonoBehaviour
    {
        public GameSpeed gameSpeed;
        public BoxCollider2D hitTrigger;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Projectile"))
            {
                Projectile currProjectile = collision.gameObject.GetComponent<Projectile>();

                if (currProjectile.origShooterTag == "Enemy")
                {
                    gameSpeed.SlowDown();
                }
            }
        }

    }
}