using UnityEngine;

public enum PetWellbeingState
{
    Bien,
    NecesitaAtencion,
    Descuidada,
    Recuperacion
}

[CreateAssetMenu(fileName = "NewPetState", menuName = "shelter/Pet State Data")]
public class PetStateData : ScriptableObject
{
    public string petName = "Mascota Prueba";
    public PetWellbeingState currentState = PetWellbeingState.Bien;

    // Variables base listas para futuras etapas
    [Range(0, 100)] public float hunger = 100f;
    [Range(0, 100)] public float hygiene = 100f;
    [Range(0, 100)] public float energy = 100f;
}
