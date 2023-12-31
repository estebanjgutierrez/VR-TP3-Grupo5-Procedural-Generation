using UnityEngine;

public enum EnemyLevel
{
    Enemy = 0,
    Boss = 1,
    SuperBoss = 2
}

public class EnemyController : MonoBehaviour
{
    public bool collided;
    public EnemyLevel enemyLevel = EnemyLevel.Enemy;
    private EnemySpawner _enemySpawner;

    private void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        // Verifica si el objeto alcanzado por el raycast es otro enemigo.
        if (collider.gameObject.CompareTag("Enemy") && !collided)
        {
            var otherEnemy = collider.GetComponent<EnemyController>();
            if ((int)enemyLevel >= (int)EnemyLevel.SuperBoss ||
                (int)otherEnemy.enemyLevel >= (int)EnemyLevel.SuperBoss ||
                otherEnemy.collided) return;
            collided = true;
            otherEnemy.collided = true;
            _enemySpawner.CombineEnemies(gameObject, collider.gameObject);
        }
    }
}