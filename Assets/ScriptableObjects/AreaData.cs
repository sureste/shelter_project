using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewArea", menuName = "shelter/Area Data")]
public class AreaData : ScriptableObject
{
    [Header("Identificación")]
    public string areaID;
    public string displayName;

    [Header("Escenario y Límites")]
    public GameObject backgroundPrefab; // El fondo panorámico 2D
    public float minX = -5f;            // Límite izquierdo para la cámara
    public float maxX = 5f;             // Límite derecho para la cámara

    [Header("Elementos del Escenario")]
    public List<DoorSpawnConfig> doors;
    public List<AnimalSpawnConfig> animals;
}

[Serializable]
public struct DoorSpawnConfig
{
    public Vector2 position;
    public AreaData targetArea; // A dónde lleva esta puerta
    public Sprite customSprite; // Opcional por si cada puerta luce distinta
}

[Serializable]
public struct AnimalSpawnConfig
{
    public GameObject animalPrefab;
    public Vector2 spawnPosition;
    public bool canRoamHorizontally;
    public float roamRange;
}