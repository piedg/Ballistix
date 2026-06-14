using System;
using Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [Header("Difficulty Settings")]
    [Tooltip("Quanto tempo (in secondi) impiega la IA per aggiornare la sua decisione.")]
    [SerializeField] private float reactionTime = 0.2f; 
    [Tooltip("Di quanto può sbagliare la mira la IA rispetto al centro della palla.")]
    [SerializeField] private float errorMargin = 1.5f; 
    [Tooltip("Tempo di ricarica tra un impulso e l'altro.")]
    [SerializeField] private float impulseCooldown = 1.0f;

    [Header("Ball Detection")] 
    [SerializeField] private float detectionRadius = 20f;
    [SerializeField] private LayerMask ballLayer;

    private Ball _targetBall;
    private Vector3 _startPosition;
    
    // Timer interni per simulare i riflessi 
    private float _nextThinkTime;
    private float _nextImpulseTime;
    private float _currentErrorOffset;

    public event Action<int> OnLivesChanged;

    private void Start()
    {
        SetInitLives(); 
        _startPosition = transform.position; 
    }

    private void Update()
    {
        if (Time.time >= _nextThinkTime)
        {
            _targetBall = FindBestBall();
            
            // Genera un nuovo errore di mira ogni volta che cambia idea
            if (_targetBall != null)
            {
                _currentErrorOffset = Random.Range(-errorMargin, errorMargin);
            }

            _nextThinkTime = Time.time + reactionTime;
        }

        if (_targetBall != null && _targetBall.gameObject.activeInHierarchy)
        {
            Move();
            TryImpulse();
        }
        else
        {
            ReturnToStartPosition();
        }
        
        Die();
    }

    private void ReturnToStartPosition()
    {
        Vector3 moveAxis = GetMoveAxis();
        float delta = Vector3.Dot(_startPosition - transform.position, moveAxis);
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
        
        // Somma l'errore calcolato alla posizione reale della palla
        Vector3 perceivedBallPosition = _targetBall.transform.position + (moveAxis * _currentErrorOffset);
        
        float delta = Vector3.Dot(perceivedBallPosition - transform.position, moveAxis);
        float inputX = Mathf.Abs(delta) > reactionDeadzone ? Mathf.Sign(delta) : 0f;

        kart.Move(new Vector2(inputX, 0f), moveAxis);
    }

    private void TryImpulse()
    {
        if (_targetBall == null) return;

        // Don't spam impulse
        if (Vector3.Distance(transform.position, _targetBall.transform.position) <= impulseActivationRadius)
        {
            if (Time.time >= _nextImpulseTime)
            {
                kart.Impulse();
                _nextImpulseTime = Time.time + impulseCooldown;
            }
        }
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
            
            if (Application.isPlaying)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(_targetBall.transform.position + (GetMoveAxis() * _currentErrorOffset), 0.5f);
            }
        }
    }
}