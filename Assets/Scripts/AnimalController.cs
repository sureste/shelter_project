using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class AnimalController : MonoBehaviour, ITappable
{
    [Header("Componentes")]
    [SerializeField] private Animator animator;

    [Header("Reacción Provisional (si aún no hay Animator)")]
    [SerializeField] private float tapBounceHeight = 0.35f;
    [SerializeField] private float tapBounceDuration = 0.25f;

    [Header("Movimiento Horizontal Opcional")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float waitTimeAtEdges = 1.5f;

    private SpriteRenderer _sr;
    private bool _canRoam;
    private float _minX;
    private float _maxX;
    private int _moveDir = 1;
    private float _waitTimer;

    private Vector3 _basePosition;
    private float _bounceTimer;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void Setup(AnimalSpawnConfig config)
    {
        transform.localPosition = config.spawnPosition;
        _basePosition = transform.localPosition;

        _canRoam = config.canRoamHorizontally && config.roamRange > 0.1f;
        _minX = _basePosition.x - config.roamRange;
        _maxX = _basePosition.x + config.roamRange;
    }

    private void Update()
    {
        HandleRoam();
        HandleTapBounce();
    }

    private void HandleRoam()
    {
        if (!_canRoam) return;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
            return;
        }

        // Mover en X
        _basePosition.x += _moveDir * moveSpeed * Time.deltaTime;

        // Voltear sprite según dirección
        _sr.flipX = _moveDir < 0;

        // Checar límites de patrulla
        if (_basePosition.x >= _maxX)
        {
            _basePosition.x = _maxX;
            _moveDir = -1;
            _waitTimer = waitTimeAtEdges;
        }
        else if (_basePosition.x <= _minX)
        {
            _basePosition.x = _minX;
            _moveDir = 1;
            _waitTimer = waitTimeAtEdges;
        }
    }

    private void HandleTapBounce()
    {
        if (_bounceTimer > 0f)
        {
            _bounceTimer -= Time.deltaTime;
            float progress = 1f - (_bounceTimer / tapBounceDuration);
            // Parábola simple de salto
            float yOffset = Mathf.Sin(progress * Mathf.PI) * tapBounceHeight;
            transform.localPosition = new Vector3(_basePosition.x, _basePosition.y + yOffset, _basePosition.z);
        }
        else
        {
            transform.localPosition = _basePosition;
        }
    }

    public void OnTap()
    {
        // 1. Si tienes Animator configurado, dispara el trigger "React"
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetTrigger("React");
        }

        // 2. Feedback visual inmediato (funciona incluso con cuadros provisionales)
        _bounceTimer = tapBounceDuration;
    }
}