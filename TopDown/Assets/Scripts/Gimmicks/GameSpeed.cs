namespace Matt_Gimmicks
{
    using Matt_Movement;
    using UnityEngine;

    public class GameSpeed : MonoBehaviour
    {
        [Range(0.01f, 0.1f)]
        public float slowDownFactor = 0.05f;
        [Range(1f, 3f)]
        public float slowDownLength = 2f;

        private EnemyMovement[] allEnemies;
        private Projectile[] allProjectiles;

        private bool isInSlowDown;
        private float timeSinceSlowDown;

        private void Update()
        {
            if (isInSlowDown)
            {
                timeSinceSlowDown += Time.deltaTime;
                if (timeSinceSlowDown >= slowDownLength)
                {
                    RevertBack();
                }
            }
        }

        public void SlowDown()
        {
            if (isInSlowDown == false)
            {
                allEnemies = GameObject.FindObjectsOfType<EnemyMovement>();
                allProjectiles = GameObject.FindObjectsOfType<Projectile>();

                foreach (EnemyMovement currEnemy in allEnemies)
                {
                    currEnemy.TriggerSlowDown(slowDownFactor);
                }

                foreach (Projectile currProjectile in allProjectiles)
                {
                    if (currProjectile.origShooterTag != "Player")
                    {
                        currProjectile.TriggerSlowDown(slowDownFactor);
                    }
                }

                isInSlowDown = true;
                timeSinceSlowDown = 0f;
            }
        }

        public void RevertBack()
        {
            if (isInSlowDown == true)
            {
                foreach (EnemyMovement currEnemy in allEnemies)
                {
                    currEnemy.RestoreToNormalSpeed();
                }

                foreach (Projectile currProjectile in allProjectiles)
                {
                    if (currProjectile.origShooterTag != "Player")
                    {
                        currProjectile.RestoreToNormalSpeed();
                    }
                }

                isInSlowDown = false;
                timeSinceSlowDown = 0f;
            }

        }

    }
}