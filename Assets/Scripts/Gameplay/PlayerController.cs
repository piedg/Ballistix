using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Kart kart;

    private int _score = 15;

    public int Score => _score;
    
    private void Awake()
    {
        inputManager.OnJump += kart.Impulse;
    }

    private void OnDestroy()
    {
        inputManager.OnJump -= kart.Impulse;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 input = inputManager.GetMovementVectorNormalized();
        kart.Move(input);
    }

    public void UpdateScore(int amount)
    {
        _score -= amount;
    }
}