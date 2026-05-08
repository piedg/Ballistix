using Gameplay;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Kart kart;


    private void Awake()
    {
        inputManager.OnJump += kart.Impulse;
    }

    private void OnDestroy()
    {
        inputManager.OnJump -= kart.Impulse;
    }

    private void Start()
    {
        Lives = IPlayer.InitialLives;
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

    public void DecreaseLives(int amount)
    {
        Lives -= amount;
    }

    public void Die()
    {
        if (Lives <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}