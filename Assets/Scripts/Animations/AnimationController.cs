using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private Animator animator;
    private Kart _kart;
    public bool isSide = false;

    private void Awake()
    {
        _kart = GetComponentInParent<Kart>();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        Vector2 velocity = _kart.MoveDirection;
        Debug.Log(transform.parent.name + " velocity: " + velocity);
        float smoothedX = Mathf.Lerp(animator.GetFloat(MoveX), isSide ? velocity.y : velocity.x, Time.deltaTime * smoothSpeed);

        animator.SetFloat(MoveX, smoothedX);
    }
}