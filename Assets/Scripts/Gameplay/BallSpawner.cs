using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private ObjectPool ballPool;
    [SerializeField] private Vector2 spawnDirection;
    [SerializeField] private Transform spawnPoint;
    
    [Header("Ball Settings")]
    [SerializeField] private float ballInitialMinSpeed = 15f;
    [SerializeField] private float ballInitialMaxSpeed = 30f;

    private void Start()
    {
        if(spawnPoint == null)
        {
            spawnPoint = transform;
        }
    }

    public void SpawnBall()
    {
        var ballInstance = ballPool.GetObjectFromPool();
        ballInstance.transform.position = spawnPoint.transform.position;
        
        if (ballInstance.TryGetComponent(out Ball ball))
        {
            var initialSpeed = Random.Range(ballInitialMinSpeed, ballInitialMaxSpeed);

            var initialVelocity = new Vector3(spawnDirection.x, 0f, spawnDirection.y) * initialSpeed;
            ball.SetInitialVelocity(initialVelocity);
        }
        
        ballInstance.SetActive(true);
    }
}