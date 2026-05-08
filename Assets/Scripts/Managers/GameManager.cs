using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private AIController enemy1;
        [SerializeField] private AIController enemy2;
        [SerializeField] private AIController enemy3;
        
        [SerializeField] TextMeshProUGUI p1scoreText;
        [SerializeField] TextMeshProUGUI p2scoreText;
        [SerializeField] TextMeshProUGUI p3scoreText;
        [SerializeField] TextMeshProUGUI p4scoreText;

        private void Update()
        {
                p1scoreText.text = $"Player : \n {playerController.Lives}";
                p2scoreText.text = $"Enemy 1: \n {enemy1.Lives}";
                p3scoreText.text = $"Enemy 2: \n {enemy2.Lives}";
                p4scoreText.text = $"Enemy 3: \n {enemy3.Lives}";
        }
    }
}