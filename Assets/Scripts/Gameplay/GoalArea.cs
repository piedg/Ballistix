using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class GoalArea : MonoBehaviour
    {
        [SerializeField] private GameObject kart;
        [SerializeField] private GameObject wall;

        private const float DisableBallDelay = 1f;

        public UnityEvent OnGoal;

        private void Start()
        {
            wall.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Ball ball))
            {
                if (kart.TryGetComponent(out IPlayer player))
                {
                    player.DecreaseLives(1);
                    OnGoal.Invoke();
                    
                    if (player.Lives <= 0)
                    {
                        ShowWall();
                    }
                }

                ball.DisableBallAfterDelay(DisableBallDelay);
            }
        }

        private void ShowWall()
        {
            wall.SetActive(true);
        }
    }
}