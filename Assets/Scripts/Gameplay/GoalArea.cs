using UnityEngine;

public class GoalArea : MonoBehaviour
{
    [SerializeField] private GameObject kart;
    [SerializeField] private GameObject wall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball ball))
        {
            if (kart.transform.TryGetComponent(out PlayerController player))
            {
                player.UpdateScore(1);
            }
            if (kart.transform.TryGetComponent(out DummyEnemy enemy))
            {
                enemy.UpdateScore(1);
                
                if(enemy.Score <= 0)
                {
                    ShowWall();
                }
            }
        }
    }
    
    private void ShowWall()
    {
        wall.SetActive(true);
    }
}