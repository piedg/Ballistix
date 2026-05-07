using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private ObjectPool ballPool;
    [SerializeField] private Vector2 spawnDirection;
    [SerializeField] private float spawnInterval = 2f;
    
    [Header("Ball Settings")]
    [SerializeField] private float ballInitialMinSpeed = 15f;
    [SerializeField] private float ballInitialMaxSpeed = 30f;

    public void SpawnBall()
    {
        var ballInstance = ballPool.GetObjectFromPool();
        ballInstance.transform.position = transform.position;
        
        ballInstance.SetActive(true);
        
        if (ballInstance.TryGetComponent(out Ball ball))
        {
            var initialSpeed = Random.Range(ballInitialMinSpeed, ballInitialMaxSpeed);

            var initialVelocity = new Vector3(spawnDirection.x, 0f, spawnDirection.y) * initialSpeed;
            ball.SetInitialVelocity(initialVelocity);
        }
    }
}