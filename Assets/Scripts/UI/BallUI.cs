using TMPro;
using UnityEngine;

public class BallUI : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private TextMeshProUGUI velocityText;

    private void Awake()
    {
        ball.OnLinearVelocityChanged += OnBallVelocityChanged;
    }

    private void OnDestroy()
    {
        ball.OnLinearVelocityChanged -= OnBallVelocityChanged;
    }

    private void OnBallVelocityChanged(float newSpeed)
    {
        velocityText.text = $"{newSpeed:F2}";
    }
}