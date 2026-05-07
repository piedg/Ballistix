using System;
using System.Collections;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float friction = 0f;
    [SerializeField] private float minSpeed = 15f;
    [SerializeField] private float maxSpeed = 100f;

    private Vector3 _initialVelocity;

    [Header("Hit Settings")] [SerializeField]
    private float wallBounceDamping = 0.95f;

    [SerializeField] private float kartHitMultiplier = 1.4f;

    [Header("Feedback")] [SerializeField] private float hitCooldownDuration = 0.5f;
    private float _hitCooldown;
    private bool _canHit = true;
    public bool CanHit => _canHit;

    [SerializeField] private Material hitMaterial;
    [SerializeField] private Material defaultMaterial;

    private Vector3 _currentVelocity;
    private Rigidbody _rb;

    private float _disableTimer = -1f;

    public event Action<float> OnLinearVelocityChanged;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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
    }

    private void Update()
    {
        if (!_canHit)
        {
            _hitCooldown -= Time.deltaTime;
            if (_hitCooldown <= 0f)
            {
                SetCanHit(true);
                GetComponentInChildren<Renderer>().material = defaultMaterial;
            }
        }

        if (_disableTimer > 0f)
        {
            _disableTimer -= Time.deltaTime;
            if (_disableTimer <= 0f)
            {
                DisableBall();
            }
        }
    }

    private void FixedUpdate()
    {
        _currentVelocity *= 1f - friction * Time.fixedDeltaTime;

        float speed = _currentVelocity.magnitude;

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

        GetComponentInChildren<Renderer>().material = _canHit ? defaultMaterial : hitMaterial;
    }

    public void ApplyImpulse(Vector3 impulseForce)
    {
        _currentVelocity += impulseForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var normal = collision.contacts[0].normal;

        normal.y = 0f;
        normal.Normalize();

        if (collision.collider.TryGetComponent(out PlayerController kart))
        {
            var otherVelocity = kart.CurrentVelocity;
            otherVelocity.y = 0f;

            var relativeVelocity = _currentVelocity - otherVelocity;

            var reflected = Vector3.Reflect(relativeVelocity, normal);

            _currentVelocity = reflected + otherVelocity * kartHitMultiplier;

            kart.ToggleHitEffect();
        }
        else
        {
            var reflected = Vector3.Reflect(_currentVelocity, normal);

            var randomAngle = UnityEngine.Random.Range(-5f, 5f);
            reflected = Quaternion.Euler(0f, randomAngle, 0f) * reflected;

            _currentVelocity = reflected * wallBounceDamping;
        }

        SetCanHit(false);

        _currentVelocity.y = 0f;
    }

    private void ResetBall()
    {
        _currentVelocity = _initialVelocity;
        _rb.linearVelocity = _currentVelocity;

        _hitCooldown = hitCooldownDuration;
        _disableTimer = -1f;
        SetCanHit(false);
    }

    private void DisableBall()
    {
        ResetBall();
        gameObject.SetActive(false);
    }

    public void DisableBallAfterDelay(float delay)
    {
        _disableTimer = delay;
    }
}