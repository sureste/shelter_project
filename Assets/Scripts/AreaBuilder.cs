using System.Collections.Generic;
using UnityEngine;

public class AreaBuilder : MonoBehaviour
{
    [Header("Prefabs Base")]
    [SerializeField] private DoorInteractable doorPrefab;

    private readonly List<GameObject> _spawnedElements = new List<GameObject>();

    private void Start()
    {
        if (NavigationManager.Instance != null)
            NavigationManager.Instance.OnAreaLoaded += BuildAreaElements;
    }

    private void OnDestroy()
    {
        if (NavigationManager.Instance != null)
            NavigationManager.Instance.OnAreaLoaded -= BuildAreaElements;
    }

    private void BuildAreaElements(AreaData area)
    {
        // Limpiar puertas/animales del cuarto anterior
        foreach (var item in _spawnedElements)
        {
            if (item != null) Destroy(item);
        }
        _spawnedElements.Clear();

        if (area == null) return;

        // Instanciar todas las puertas configuradas en el ScriptableObject
        if (area.doors != null && doorPrefab != null)
        {
            foreach (var doorConfig in area.doors)
            {
                DoorInteractable door = Instantiate(doorPrefab, transform);
                door.Setup(doorConfig);
                _spawnedElements.Add(door.gameObject);
            }
        }

        if (area.animals != null)
        {
            foreach (var animalConfig in area.animals)
            {
                if (animalConfig.animalPrefab == null) continue;

                GameObject animalObj = Instantiate(animalConfig.animalPrefab, transform);
                if (animalObj.TryGetComponent<AnimalController>(out var controller))
                {
                    controller.Setup(animalConfig);
                }
                _spawnedElements.Add(animalObj);
            }
        }
    }
}