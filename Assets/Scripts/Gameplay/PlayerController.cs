using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    private int _score = 15;
    public int Score => _score;

    [Header("Obstacle Detection")] [SerializeField]
    private LayerMask obstacleLayer;

    [SerializeField] private float colliderHalfWidth = 0.5f;
    [SerializeField] private float skinWidth = 0.05f;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material hitMaterial;
    [SerializeField] private Material impulseMaterial;
    
    [SerializeField] private float hitEffectDuration = 0.2f;
    [SerializeField] private float impulseEffectDuration = 0.1f;

    [SerializeField] private float impulseRadius = 10f;
    [SerializeField] private float impulsePower = 50f;
    [SerializeField] private float impulseCooldown = 2f;
    private float _impulseCooldownTimer;
    private bool _canUseImpulse = true;

    private float _hitEffectTimer;

    private Vector3 _currentVelocity;
    public Vector3 CurrentVelocity => _currentVelocity;

    private void Awake()
    {
        inputManager.OnJump += Impulse;
    }

    private void OnDestroy()
    {
        inputManager.OnJump -= Impulse;
    }

    private void Update()
    {
        Move();

        if (_hitEffectTimer > 0f)
        {
            _hitEffectTimer -= Time.deltaTime;
            if (_hitEffectTimer <= 0f)
            {
                meshRenderer.material = defaultMaterial;
            }
        }

        if (!_canUseImpulse)
        {
            _impulseCooldownTimer -= Time.deltaTime;
            if (_impulseCooldownTimer <= 0f)
            {
                _canUseImpulse = true;
                meshRenderer.material = defaultMaterial;
            }
        }
    }

    private void Move()
    {
        Vector2 input = inputManager.GetMovementVectorNormalized();
        
        Vector3 inputDirection = new Vector3(input.x, 0f, 0f);

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

        movement.x = ClampAxisMovement(movement.x, Vector3.right);

        transform.Translate(movement, Space.World);
    }

    private float ClampAxisMovement(float delta, Vector3 axis)
    {
        if (delta == 0f) return 0f;

        float direction = Mathf.Sign(delta);
        Vector3 rayOrigin = transform.position + axis * (direction * colliderHalfWidth);
        float rayDistance = Mathf.Abs(delta) + skinWidth;

        bool hit = Physics.Raycast(rayOrigin, axis * direction, out RaycastHit hitInfo, rayDistance, obstacleLayer);

        Debug.DrawRay(rayOrigin, axis * (direction * rayDistance), Color.red);

        if (hit)
        {
            float allowedDistance = Mathf.Max(0f, hitInfo.distance - skinWidth);

            _currentVelocity -= Vector3.Dot(_currentVelocity, axis) * axis;

            return direction * allowedDistance;
        }

        return delta;
    }

    public void Impulse()
    {
        if (!_canUseImpulse) return;

        _canUseImpulse = false;
        _impulseCooldownTimer = impulseCooldown;

        Collider[] ballsInRadius = Physics.OverlapSphere(transform.position, impulseRadius);

        foreach (Collider col in ballsInRadius)
        {
            if (col.TryGetComponent(out Ball ball))
            {
                Vector3 directionFromKart = (col.transform.position - transform.position).normalized;
                ball.ApplyImpulse(directionFromKart * impulsePower);
                ball.SetCanHit(false);
                _canUseImpulse = true;
                _impulseCooldownTimer = 0f;
            }
        }

        meshRenderer.material = impulseMaterial;
        _hitEffectTimer = hitEffectDuration;
    }

    public void UpdateScore(int amount)
    {
        _score -= amount;
    }

    public void ToggleHitEffect()
    {
        meshRenderer.material = hitMaterial;
        _hitEffectTimer = hitEffectDuration;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, impulseRadius);
    }
}