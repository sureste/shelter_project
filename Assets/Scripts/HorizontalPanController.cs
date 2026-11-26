using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class HorizontalPanController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float panSpeed = 0.015f;
    [SerializeField] private float smoothTime = 0.08f;

    [Header("Filtro Tap vs Drag")]
    [Tooltip("Distancia mínima en píxeles para considerar el toque como arrastre y no como clic")]
    [SerializeField] private float dragThresholdPixels = 20f;

    private Camera _cam;
    private float _minX;
    private float _maxX;
    private float _targetX;
    private float _velocityX;

    private Vector2 _touchStartPos;
    private Vector2 _lastPointerPos;
    private bool _isTouching;
    private bool _isDragging;
    private bool _canPan;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (NavigationManager.Instance != null)
            NavigationManager.Instance.OnAreaLoaded += ConfigureBoundsForArea;
    }

    private void OnDestroy()
    {
        if (NavigationManager.Instance != null)
            NavigationManager.Instance.OnAreaLoaded -= ConfigureBoundsForArea;
    }

    private void ConfigureBoundsForArea(AreaData area)
    {
        Debug.Log("Área recibida: " + area.displayName);
        _canPan = area != null;
        if (!_canPan) return;

        // Calculamos la mitad del ancho visible de la cámara ortográfica
        // para que los bordes del escenario coincidan con los bordes de la pantalla
        float halfCamWidth = _cam.orthographicSize * _cam.aspect;

        _minX = area.minX + halfCamWidth;
        _maxX = area.maxX - halfCamWidth;

        // Si el escenario es más angosto que la pantalla, lo centramos
        if (_minX > _maxX)
        {
            float center = (area.minX + area.maxX) * 0.5f;
            _minX = _maxX = center;
        }

        // Reiniciar cámara al centro del nuevo cuarto
        _targetX = Mathf.Clamp(0f, _minX, _maxX);
        transform.position = new Vector3(_targetX, transform.position.y, transform.position.z);
        _velocityX = 0f;
    }

    private void Update()
    {
        if (!_canPan) return;

        HandlePointerInput();

        // Movimiento suavizado solo en X, bloqueando Y y Z
        float newX = Mathf.SmoothDamp(transform.position.x, _targetX, ref _velocityX, smoothTime);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    private void HandlePointerInput()
    {
        // Pointer.current unifica el toque táctil en Android y el clic izquierdo en el Editor
        var pointer = Pointer.current;
        if (pointer == null) return;

        // 1. Al presionar el dedo/mouse
        if (pointer.press.wasPressedThisFrame)
        {
            // Ignorar si tocó un botón de la UI (ej. GlobalBackBtn o Ajustes)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            _touchStartPos = pointer.position.ReadValue();
            _lastPointerPos = _touchStartPos;
            _isTouching = true;
            _isDragging = false;
        }

        // 2. Mientras mantiene presionado
        if (_isTouching && pointer.press.isPressed)
        {
            Vector2 currentPos = pointer.position.ReadValue();

            if (!_isDragging && Vector2.Distance(_touchStartPos, currentPos) > dragThresholdPixels)
            {
                _isDragging = true;
            }

            if (_isDragging)
            {
                float deltaX = currentPos.x - _lastPointerPos.x;
                // Restamos deltaX para sensación natural de arrastrar el escenario
                _targetX -= deltaX * panSpeed;
                _targetX = Mathf.Clamp(_targetX, _minX, _maxX);
            }

            _lastPointerPos = currentPos;
        }

        // 3. Al soltar el dedo/mouse
        if (_isTouching && pointer.press.wasReleasedThisFrame)
        {
            // Si NO superó el umbral de arrastre, fue un TAP limpio
            if (!_isDragging)
            {
                TryInteractAtPosition(pointer.position.ReadValue());
            }

            _isTouching = false;
            _isDragging = false;
        }
    }

    private void TryInteractAtPosition(Vector2 screenPos)
    {
        Vector2 worldPoint = _cam.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && hit.collider.TryGetComponent<ITappable>(out var tappable))
        {
            tappable.OnTap();
        }
    }
}