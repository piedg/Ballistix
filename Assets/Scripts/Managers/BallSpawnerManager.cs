using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallSpawnerManager : MonoBehaviour
{
    [SerializeField] private BallSpawner[] ballSpawners;
    
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 5f;
    [SerializeField] private float startSpawnTime = 1.5f; 

    private float _startSpawnTimer;
    private float _spawnTimer;
    private float _currentSpawnInterval;

    private bool _isSpawning;
    private bool _isFirstSpawnStarted;

    private void Update()
    {
        if (_isFirstSpawnStarted && !_isSpawning)
        {
            _startSpawnTimer += Time.deltaTime;
            
            if (_startSpawnTimer >= startSpawnTime)
            {
                _isSpawning = true;
                SetNewSpawnInterval(); 
            }
        }

        if (_isSpawning)
        {
            _spawnTimer += Time.deltaTime;
            
            if (_spawnTimer >= _currentSpawnInterval)
            {
                ActiveRandomSpawner();
                
                _spawnTimer = 0f; 
                SetNewSpawnInterval(); 
            }
        }
    }

    public void StopSpawning()
    {
        _isSpawning = false;
        _isFirstSpawnStarted = false;
    }

    public void StartSpawning()
    {
        _startSpawnTimer = 0f;
        _isFirstSpawnStarted = true;
    }

    private void SetNewSpawnInterval()
    {
        _currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void ActiveRandomSpawner()
    {
        if (ballSpawners == null || ballSpawners.Length == 0) return;

        int randomIndex = Random.Range(0, ballSpawners.Length);
        ballSpawners[randomIndex].SpawnBall();
    }
}