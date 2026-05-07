using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private DummyEnemy enemy1;
        [SerializeField] private DummyEnemy enemy2;
        [SerializeField] private DummyEnemy enemy3;
        
        [SerializeField] TextMeshProUGUI p1scoreText;
        [SerializeField] TextMeshProUGUI p2scoreText;
        [SerializeField] TextMeshProUGUI p3scoreText;
        [SerializeField] TextMeshProUGUI p4scoreText;

        private void Update()
        {
                p1scoreText.text = $"Player : \n {playerController.Score}";
                p2scoreText.text = $"Enemy 1: \n {enemy1.Score}";
                p3scoreText.text = $"Enemy 2: \n {enemy2.Score}";
                p4scoreText.text = $"Enemy 3: \n {enemy3.Score}";
        }
    }
}