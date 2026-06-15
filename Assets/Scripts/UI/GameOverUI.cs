using System;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private TextMeshProUGUI winnerText;

    private void Awake()
    {
        gameManager.onFinishGame.AddListener(UpdateUI);
    }

    public void UpdateUI(string winner)
    {
        winnerText.text = winner + " WIN!";
    }
}
