using UnityEngine;
using UnityEngine.Events;

public class Kart : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 80f;
    [SerializeField] private float acceleration = 400f;
    [SerializeField] private float deceleration = 200f;

    [Header("Obstacle Detection")] [SerializeField]
    private LayerMask obstacleLayer;

    [SerializeField] private float skinWidth = 0.05f;

    [SerializeField] private float hitEffectDuration = 0.2f;

    [SerializeField] private float impulseRadius = 10f;
    [SerializeField] private float impulsePower = 50f;
    [SerializeField] private float impulseCooldownMiss = 2f;
    [SerializeField] private float impulseCooldownHit = 0.5f;

    private float _impulseCooldown;
    private float _impulseCooldownTimer;
    private bool _canUseImpulse = true;

    private float _hitEffectTimer;

    private Vector3 _currentVelocity;
    public Vector3 CurrentVelocity => _currentVelocity;

    public Vector2 MoveDirection => new Vector2(_currentVelocity.x, _currentVelocity.z).normalized;
    
    private CapsuleCollider _capsuleCollider;
    
    public UnityEvent onImpulse;

    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        if (!_canUseImpulse)
        {
            _impulseCooldownTimer -= Time.deltaTime;
            if (_impulseCooldownTimer <= 0f)
            {
                _canUseImpulse = true;
            }
        }
    }

    public void Move(Vector2 input, Vector3 movementAxis)
    {
        Vector3 inputDirection = movementAxis * input.x;

        if (inputDirection != Vector3.zero)
        {
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, inputDirection * moveSpeed,
                acceleration * Time.deltaTime);
        }
        else
        {
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        Vector3 movement = _currentVelocity * Time.deltaTime;
        movement = ClampAxisMovement(movement, movementAxis);

        transform.Translate(movement, Space.World);
    }

    private Vector3 ClampAxisMovement(Vector3 movement, Vector3 axis)
    {
        float delta = Vector3.Dot(movement, axis);
        if (delta == 0f) return movement;

        float direction = Mathf.Sign(delta);
        float rayDistance = Mathf.Abs(delta) + skinWidth;

        Vector3 center = transform.position + _capsuleCollider.center;
        float halfHeight = Mathf.Max(0f, _capsuleCollider.height / 2f - _capsuleCollider.radius);
        Vector3 point1 = center + Vector3.up * halfHeight;
        Vector3 point2 = center - Vector3.up * halfHeight;

        bool hit = Physics.CapsuleCast(
            point1,
            point2,
            _capsuleCollider.radius,
            axis * direction,
            out RaycastHit hitInfo,
            rayDistance,
            obstacleLayer
        );

        if (hit)
        {
            float allowedDistance = Mathf.Max(0f, hitInfo.distance - skinWidth);
            _currentVelocity -= Vector3.Dot(_currentVelocity, axis) * axis;
            return movement - axis * delta + axis * (direction * allowedDistance);
        }

        return movement;
    }

    public void Impulse()
    {
        if (!_canUseImpulse) return;

        _canUseImpulse = false;
        bool hitAnyBall = false;

        Collider[] ballsInRadius = Physics.OverlapSphere(transform.position, impulseRadius);

        foreach (Collider col in ballsInRadius)
        {
            if (col.TryGetComponent(out Ball ball))
            {
                Vector3 directionFromKart = (col.transform.position - transform.position).normalized;
                ball.ApplyImpulse(directionFromKart * impulsePower);
                ball.SetCanHit(false);
                
                hitAnyBall = true;
            }
        }

        _hitEffectTimer = hitEffectDuration;
        _impulseCooldownTimer = hitAnyBall ? impulseCooldownHit : impulseCooldownMiss;
        
        
        onImpulse?.Invoke();
    }

    public void ToggleHitEffect()
    {
        _hitEffectTimer = hitEffectDuration;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, impulseRadius);
    }
}