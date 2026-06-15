using System;
using Gameplay;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Kart kart;
    
    public event Action<int> OnLivesChanged;

    private void Awake()
    {
        SetInitLives();
        
        inputManager.OnJump += kart.Impulse;
    }

    private void OnDestroy()
    {
        inputManager.OnJump -= kart.Impulse;
    }

    private void Start()
    {
        OnLivesChanged?.Invoke(Lives);
    }

    private void Update()
    {
        Move();
        Die();
    }

    private void Move()
    {
        Vector2 input = inputManager.GetMovementVectorNormalized();
        kart.Move(input, transform.right);
    }

    public int Lives { get; set; }

    private void SetInitLives()
    {
        Lives = IPlayer.InitialLives;
    }
    
    public void DecreaseLives(int amount)
    {
        Lives -= amount;
        OnLivesChanged?.Invoke(Lives);
    }

    public void Die()
    {
        if (Lives <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}