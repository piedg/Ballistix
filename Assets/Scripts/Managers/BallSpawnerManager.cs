using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallSpawnerManager : MonoBehaviour
{
    [SerializeField] private BallSpawner[] ballSpawners;
    
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 5f;

    private void Start()
    {
        StartCoroutine(ActiveRandomSpawnerRoutine());
    }

    private IEnumerator ActiveRandomSpawnerRoutine()
    {
        while (true)
        {
            ActiveRandomSpawner();

            var spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);
        }
    }


    private void ActiveRandomSpawner()
    {
        int randomIndex = Random.Range(0, ballSpawners.Length);
        ballSpawners[randomIndex].SpawnBall();
    }
}