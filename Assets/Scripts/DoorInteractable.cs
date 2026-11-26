using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class DoorInteractable : MonoBehaviour, ITappable
{
    private AreaData _targetArea;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(DoorSpawnConfig config)
    {
        _targetArea = config.targetArea;
        transform.localPosition = config.position;

        if (config.customSprite != null)
        {
            _spriteRenderer.sprite = config.customSprite;
        }
    }

    public void OnTap()
    {
        if (_targetArea != null)
        {
            NavigationManager.Instance.EnterArea(_targetArea);
        }
        else
        {
            Debug.LogWarning($"Puerta '{name}' no tiene un AreaData asignado.");
        }
    }
}