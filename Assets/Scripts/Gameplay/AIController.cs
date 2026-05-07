using UnityEngine;

public enum eKartPosition
{
    Up,
    Down,
    Left,
    Right
}

public class AIController : MonoBehaviour
{
    [SerializeField] private eKartPosition kartPosition;
    [SerializeField] private Kart kart;

    [Header("AI Settings")] [SerializeField]
    private float reactionDeadzone = 0.1f;

    [SerializeField] private float impulseActivationRadius = 5f;

    [Header("Ball Detection")] [SerializeField]
    private float detectionRadius = 20f;

    [SerializeField] private LayerMask ballLayer;

    private Ball _targetBall;
    private int _score = 15;
    public int Score => _score;


    [Header("Random Movement")] [SerializeField]
    private float randomMoveMinDuration = 0.5f;

    [SerializeField] private float randomMoveMaxDuration = 2f;

    private float _randomMoveTimer;
    private float _randomMoveDirection;

    private void Update()
    {
        _targetBall = FindBestBall();

        if (_targetBall != null)
        {
            Move();
            TryImpulse();
        }
        else
        {
            MoveRandomly();
        }
    }

    private void MoveRandomly()
    {
        _randomMoveTimer -= Time.deltaTime;

        if (_randomMoveTimer <= 0f)
        {
            _randomMoveDirection = Random.value > 0.5f ? 1f : -1f;
            _randomMoveTimer = Random.Range(randomMoveMinDuration, randomMoveMaxDuration);
        }

        kart.Move(new Vector2(_randomMoveDirection, 0f), GetKartDirection());
    }

    private Ball FindBestBall()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, ballLayer);

        if (hits.Length == 0) return null;

        Ball bestBall = null;
        float bestScore = float.MinValue;

        foreach (Collider col in hits)
        {
            if (!col.TryGetComponent(out Ball ball)) continue;

            var directionToBall = (col.transform.position - transform.position).normalized;

            if (Vector3.Dot(-GetKartDirection(), directionToBall) < 0f) continue;

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

        float delta = _targetBall.transform.position.x - transform.position.x;
        float inputX = Mathf.Abs(delta) > reactionDeadzone ? Mathf.Sign(delta) : 0f;

        kart.Move(new Vector2(inputX, 0f), GetKartDirection());
    }

    private void TryImpulse()
    {
        if (_targetBall == null) return;

        if (Vector3.Distance(transform.position, _targetBall.transform.position) <= impulseActivationRadius)
            kart.Impulse();
    }

    public void UpdateScore(int amount)
    {
        _score -= amount;
    }

    private Vector3 GetKartDirection()
    {
        return kartPosition switch
        {
            eKartPosition.Up => Vector3.forward,
            eKartPosition.Down => Vector3.back,
            eKartPosition.Left => Vector3.left,
            eKartPosition.Right => Vector3.right,
            _ => Vector3.zero
        };
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