using System;
using UnityEngine;
using UnityEngine.Events;

public class Ball2 : MonoBehaviour
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

    private Rigidbody _rb;
    private Collider _col;
    private Renderer _renderer;

    private float _disableTimer = -1f;

    public event Action<float> OnLinearVelocityChanged;
    public UnityEvent onBallHit;

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
        _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        
        // Fondamentale per evitare che la palla passi attraverso i muri ad alta velocità
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous; 
    }

    private void OnEnable()
    {
        ResetBall();
    }

    private void Update()
    {
        HandleCooldowns();
        DisableTimer();
        IgnoreCollisionTimer();
    }

    private void HandleCooldowns()
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
    }

    private void FixedUpdate()
    {
        // 1. Leggiamo la velocità attuale gestita da Unity
        Vector3 velocity = _rb.linearVelocity;
        
        // Assicuriamoci che non ci siano micromovimenti sull'asse Y
        velocity.y = 0f;

        float speed = velocity.magnitude;

        // 2. Clamping della velocità
        if (speed > 0.01f && speed < minSpeed)
        {
            velocity = velocity.normalized * minSpeed;
        }
        else if (speed > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }

        // 3. Riapplichiamo la velocità corretta solo alla fine del calcolo
        _rb.linearVelocity = velocity;

        OnLinearVelocityChanged?.Invoke(velocity.magnitude);
    }

    public void SetInitialVelocity(Vector3 velocity)
    {
        _initialVelocity = velocity;
    }

    public void SetCanHit(bool canHit)
    {
        _canHit = canHit;

        if (!_canHit)
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
        _rb.AddForce(impulseForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // All'interno di OnCollisionEnter, Unity ha GIA' calcolato la direzione del rimbalzo.
        // Leggiamo la nuova velocità post-impatto e ne modifichiamo solo l'intensità/angolo.
        Vector3 newVelocity = _rb.linearVelocity;

        if (collision.collider.TryGetComponent(out Kart kart))
        {
            Vector3 kartVelocity = kart.CurrentVelocity;
            kartVelocity.y = 0f;

            // Sommiamo l'energia del kart
            newVelocity += kartVelocity * kartHitMultiplier;
            kart.ToggleHitEffect();
        }
        else
        {
            // Applichiamo la resistenza dei muri
            newVelocity *= wallBounceDamping;

            // Aggiungiamo la variazione di angolo casuale (effetto "spin")
            float randomAngle = UnityEngine.Random.Range(-5f, 5f);
            newVelocity = Quaternion.Euler(0f, randomAngle, 0f) * newVelocity;
        }

        newVelocity.y = 0f;
        _rb.linearVelocity = newVelocity;

        SetCanHit(false);
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
        _rb.linearVelocity = _initialVelocity;

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
    }

    public void DisableBallAfterDelay(float delay)
    {
        _disableTimer = delay;
    }
}