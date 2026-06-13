using System;
using Gameplay;
using UnityEngine;

public enum eKartPosition
{
    Up,
    Down,
    Left,
    Right
}

public class AIController : MonoBehaviour, IPlayer
{
    [SerializeField] private eKartPosition kartPosition;
    [SerializeField] private Kart kart;

    [Header("AI Settings")] 
    [SerializeField] private float reactionDeadzone = 0.1f;
    [SerializeField] private float impulseActivationRadius = 5f;

    [Header("Ball Detection")] 
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private LayerMask ballLayer;

    private Ball _targetBall;
    private Vector3 _startPosition;
    
    public event Action<int> OnLivesChanged;

    private void Start()
    {
        SetInitLives(); 
        
        _startPosition = transform.position; 
    }

    private void Update()
    {
        _targetBall = FindBestBall();

        if (_targetBall != null && _targetBall.gameObject.activeInHierarchy)
        {
            Move();
            TryImpulse();
        }
        else
        {
            ReturnToStartPosition(); // Sostituito il movimento casuale
        }
        
        Die();
    }

    private void ReturnToStartPosition()
    {
        Vector3 moveAxis = GetMoveAxis();
        
        // Calcola la distanza tra la posizione attuale e quella iniziale lungo l'asse di movimento
        float delta = Vector3.Dot(_startPosition - transform.position, moveAxis);
        
        // Usa la deadzone per evitare che il kart tremi quando arriva al centro
        float inputX = Mathf.Abs(delta) > reactionDeadzone ? Mathf.Sign(delta) : 0f;

        kart.Move(new Vector2(inputX, 0f), moveAxis);
    }

    private Ball FindBestBall()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, ballLayer);

        if (hits.Length == 0) return null;

        Ball bestBall = null;
        float bestScore = float.MinValue;

        foreach (Collider col in hits)
        {
            if (!col.gameObject.activeInHierarchy) continue;
            if (!col.TryGetComponent(out Ball ball)) continue;

            var directionToBall = (col.transform.position - transform.position).normalized;

            if (Vector3.Dot(-GetMoveAxis(), directionToBall) < 0f) continue;

            if (!col.TryGetComponent(out Rigidbody rb)) continue;

            var distance = Vector3.Distance(transform.position, col.transform.position);

            var approachScore = rb != null
                ? Vector3.Dot(rb.linearVelocity.normalized, -directionToBall)
                : 0f;

            var score = approachScore - (distance / detectionRadius);

            if (score > bestScore)
            {
                bestScore = score;
                bestBall = ball;
            }
        }

        return bestBall;
    }

    private void Move()
    {
        if (_targetBall == null) return;

        Vector3 moveAxis = GetMoveAxis();
        
        float delta = Vector3.Dot(_targetBall.transform.position - transform.position, moveAxis);
        float inputX = Mathf.Abs(delta) > reactionDeadzone ? Mathf.Sign(delta) : 0f;

        kart.Move(new Vector2(inputX, 0f), moveAxis);
    }

    private void TryImpulse()
    {
        if (_targetBall == null) return;

        if (Vector3.Distance(transform.position, _targetBall.transform.position) <= impulseActivationRadius)
            kart.Impulse();
    }

    private Vector3 GetMoveAxis()
    {
        return kartPosition switch
        {
            eKartPosition.Up or eKartPosition.Down => Vector3.right,
            eKartPosition.Left or eKartPosition.Right => Vector3.forward,
            _ => Vector3.right
        };
    }

    public int Lives { get; set; }

    private void SetInitLives()
    {
        Lives = IPlayer.InitialLives;
        OnLivesChanged?.Invoke(Lives);
    }
    
    public void DecreaseLives(int amount)
    {
        Lives -= amount;
        OnLivesChanged?.Invoke(Lives);
    }

    public void Die()
    {
        if (Lives <= 0)
            gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (_targetBall != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _targetBall.transform.position);
        }
    }
}