using System;
using System.Collections.Generic;
using System.Linq;
using AlanSartorio.GridPathGenerator;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    private List<Path<Vector2Int>> _paths;
    [SerializeField] private MapGenerator mapGenerator;
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private float spawnInterval = 5;
    private float _spawnTimer = 0;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int targetSpawnAmount = 5;
    private int _spawnCount = 0;
    [NonSerialized] public UnityEvent OnEnemyDeath = new();
    [NonSerialized] public UnityEvent OnEnemySpawn = new();

    public bool DidFinishSpawning => _spawnCount >= targetSpawnAmount;

    void Awake()
    {
        mapGenerator.OnMapChanged.AddListener(OnMapChanged);
    }

    private void OnDestroy()
    {
        mapGenerator.OnMapChanged.RemoveListener(OnMapChanged);
    }

    private void OnMapChanged(GridPathGenerator<Vector2Int> generator)
    {
        _paths = generator.GetPathsFromLeaves().ToList();
    }

    void Update()
    {
        _spawnTimer += Time.deltaTime;
        while (_spawnTimer > spawnInterval)
        {
            _spawnTimer -= spawnInterval;
            SpawnEnemies();
        }
    }

    private void OnEnable()
    {
        ResetTimer();
        ResetSpawnCount();
    }

    private void SpawnEnemies()
    {
        _spawnCount++;
        if (DidFinishSpawning)
            enabled = false;

        foreach (var path in _paths)
        {
            var pos = path.Nodes[0];
            var origin = mapGenerator.GetNodeOrigin(pos);
            var enemy = Instantiate(enemyPrefab);
            enemy.transform.position = origin;
            var enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
            OnEnemySpawn.Invoke();
            enemyBehaviour.OnDeath.AddListener(OnDeath);
            enemyBehaviour.Path = path;
            enemyBehaviour.gameStateManager = gameStateManager;
            enemyBehaviour.mapGenerator = mapGenerator;
        }
    }

    public void CombineEnemies(GameObject enemy1, GameObject enemy2)
    {
        // Destruye el enemigo actual.
        Destroy(enemy1);

        // Destruye al enemigo alcanzado por el raycast.
        Destroy(enemy2);

        // Genera un nuevo jefe.
        var bossObject = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        bossObject.name = "Boss";
        var scriptEnemy1 = enemy1.GetComponent<EnemyBehaviour>();
        var scriptEnemy2 = enemy2.GetComponent<EnemyBehaviour>();
        var boss = bossObject.GetComponent<EnemyBehaviour>();
        boss.Health = scriptEnemy1.Health + scriptEnemy2.Health;
        boss.Path = scriptEnemy1.Path;
        boss.NodeIndex = scriptEnemy1.NodeIndex;
        boss.Timer = scriptEnemy1.Timer;
        boss.mapGenerator = scriptEnemy1.mapGenerator;
        boss.gameStateManager = scriptEnemy1.gameStateManager;
        
        OnDeath();
    }

    private void OnDeath()
    {
        OnEnemyDeath.Invoke();
    }

    private void ResetTimer()
    {
        _spawnTimer = 0;
    }

    private void ResetSpawnCount()
    {
        _spawnCount = 0;
    }
}