using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private TextMeshProUGUI p1ScoreText;
    [SerializeField] private TextMeshProUGUI p2ScoreText;
    [SerializeField] private TextMeshProUGUI p3ScoreText;
    [SerializeField] private TextMeshProUGUI p4ScoreText;

    private void Awake()
    {
        gameManager.Player.OnLivesChanged += UpdatePlayerScore;
        gameManager.Enemy1.OnLivesChanged += UpdateEnemy1Score;
        gameManager.Enemy2.OnLivesChanged += UpdateEnemy2Score;
        gameManager.Enemy3.OnLivesChanged += UpdateEnemy3Score;
    }

    private void OnDestroy()
    {
        gameManager.Player.OnLivesChanged -= UpdatePlayerScore;
        gameManager.Enemy1.OnLivesChanged -= UpdateEnemy1Score;
        gameManager.Enemy2.OnLivesChanged -= UpdateEnemy2Score;
        gameManager.Enemy3.OnLivesChanged -= UpdateEnemy3Score;
    }

    private void Start()
    {
        UpdatePlayerScore(gameManager.Player.Lives);
        UpdateEnemy1Score(gameManager.Enemy1.Lives);
        UpdateEnemy2Score(gameManager.Enemy2.Lives);
        UpdateEnemy3Score(gameManager.Enemy3.Lives);
    }

    private void UpdatePlayerScore(int score)
    {
        p1ScoreText.text = $"Player: \n {math.max(score, 0)}";
    }

    private void UpdateEnemy1Score(int score)
    {
        p2ScoreText.text = $"Enemy 1 \n {math.max(score, 0)}";
    }

    private void UpdateEnemy2Score(int score)
    {
        p3ScoreText.text = $"Enemy 2 \n {math.max(score, 0)}";
    }

    private void UpdateEnemy3Score(int score)
    {
        p4ScoreText.text = $"Enemy 3: \n {math.max(score, 0)}";
    }
}