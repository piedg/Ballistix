using System;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float minSpeed = 15f;
    [SerializeField] private float maxSpeed = 100f;

    private Vector3 _initialVelocity;

    [Header("Hit Settings")] 
    [SerializeField] private float wallBounceDamping = 0.95f;
    [SerializeField] private float kartHitMultiplier = 1.2f;

    [Header("Feedback")] 
    [SerializeField] private float hitCooldownDuration = 0.5f;
    private float _hitCooldown;
    private bool _canHit = true;
    public bool CanHit => _canHit;

    [SerializeField] private Material hitMaterial;
    [SerializeField] private Material defaultMaterial;

    [Header("Spawn Settings")]
    [SerializeField] private float ignoreCollisionDuration = 2f;
    private float _ignoreCollisionTimer;

    private Vector3 _currentVelocity;
    private Rigidbody _rb;
    private Collider _col;
    private Renderer _renderer;

    private float _disableTimer = -1f;
    public Vector3 CurrentVelocity { get => _currentVelocity; set => _currentVelocity = value; }
    
    public event Action<float> OnLinearVelocityChanged;
    public UnityEvent onBallHit;
    
    public UnityEvent onBallDisable;
    public UnityEvent onBallEnable;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();
        _renderer = GetComponentInChildren<Renderer>();
    }

    private void Start()
    {
        _rb.useGravity = false;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation
                          | RigidbodyConstraints.FreezePositionY;
    }

    private void OnEnable()
    {
        ResetBall();
        
        onBallEnable?.Invoke();
    }

    private void Update()
    {
        if (!_canHit)
        {
            _hitCooldown -= Time.deltaTime;

            if (_hitCooldown <= 0f)
            {
                SetCanHit(true);
                if (_renderer != null) _renderer.material = defaultMaterial;
            }
        }

        DisableTimer();
        IgnoreCollisionTimer();
    }

    private void FixedUpdate()
    {
        var speed = _currentVelocity.magnitude;

        if (speed > 0.01f && speed < minSpeed)
            _currentVelocity = _currentVelocity.normalized * minSpeed;
        else if (speed > maxSpeed)
            _currentVelocity = _currentVelocity.normalized * maxSpeed;

        _rb.linearVelocity = _currentVelocity;

        OnLinearVelocityChanged?.Invoke(_rb.linearVelocity.magnitude);
    }

    public void SetInitialVelocity(Vector3 velocity)
    {
        _initialVelocity = velocity;
    }

    public void SetCanHit(bool canHit)
    {
        _canHit = canHit;

        if (_canHit == false)
        {
            _hitCooldown = hitCooldownDuration;
        }

        if (_renderer != null) 
        {
            _renderer.material = _canHit ? defaultMaterial : hitMaterial;
        }
    }

    public void ApplyImpulse(Vector3 impulseForce)
    {
        _currentVelocity += impulseForce;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        var normal = collision.contacts[0].normal;
        normal.y = 0f;
    
        if (normal.sqrMagnitude > 0.001f)
        {
            normal.Normalize();
        }
        else
        {
            normal = Vector3.forward; 
        }

        if (collision.collider.TryGetComponent(out Kart kart))
        {
            var otherVelocity = kart.CurrentVelocity;
            otherVelocity.y = 0f;

            var relativeVelocity = _currentVelocity - otherVelocity;
            var reflected = Vector3.Reflect(relativeVelocity, normal);

            _currentVelocity = reflected + otherVelocity * kartHitMultiplier;

            kart.ToggleHitEffect();
        }
        else if (collision.collider.TryGetComponent(out Ball otherBall))
        {
            // this check prevents the two balls from calculating and overriding the bounce twice
            if (this.gameObject.GetInstanceID() > otherBall.gameObject.GetInstanceID())
            {
                Vector3 v1 = this._currentVelocity;
                Vector3 v2 = otherBall.CurrentVelocity;

                float momentum1 = Vector3.Dot(v1, normal);
                float momentum2 = Vector3.Dot(v2, normal);

                // Swap the velocities along the collision normal
                this._currentVelocity = v1 - (momentum1 * normal) + (momentum2 * normal);
                otherBall.CurrentVelocity = v2 - (momentum2 * normal) + (momentum1 * normal);
            }
            // If this ball's ID is lower, it skips the physics calculation entirely
        }
        else
        {
            var reflected = Vector3.Reflect(_currentVelocity, normal);
            //var randomAngle = UnityEngine.Random.Range(-5f, 5f);
            //reflected = Quaternion.Euler(0f, randomAngle, 0f) * reflected;
            _currentVelocity = reflected * wallBounceDamping;
        }

        SetCanHit(false);
        _currentVelocity.y = 0f;

        onBallHit?.Invoke();
    }

    private void DisableTimer()
    {
        if (_disableTimer > 0f)
        {
            _disableTimer -= Time.deltaTime;

            if (_disableTimer <= 0f)
            {
                DisableBall();
            }
        }
    }

    private void IgnoreCollisionTimer()
    {
        if (_ignoreCollisionTimer > 0f)
        {
            _ignoreCollisionTimer -= Time.deltaTime;
            
            if (_ignoreCollisionTimer <= 0f)
            {
                if (_col != null) _col.enabled = true;
            }
        }
    }

    private void ResetBall()
    {
        _currentVelocity = _initialVelocity;
        _rb.linearVelocity = _currentVelocity;

        _hitCooldown = hitCooldownDuration;
        _disableTimer = -1f;
        SetCanHit(false);

        if (_col != null)
        {
            _col.enabled = false;
            _ignoreCollisionTimer = ignoreCollisionDuration;
        }
    }

    private void DisableBall()
    {
        ResetBall();
        gameObject.SetActive(false);
        
        onBallDisable?.Invoke();
    }

    public void DisableBallAfterDelay(float delay)
    {
        _disableTimer = delay;
    }
}