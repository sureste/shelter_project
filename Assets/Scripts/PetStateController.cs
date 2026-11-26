using UnityEngine;

public class PetStateController : MonoBehaviour
{
    [Header("Datos del ScriptableObject")]
    [SerializeField] private PetStateData petData;

    [Header("Visuales Demostrativos")]
    [SerializeField] private SpriteRenderer targetSprite;
    [SerializeField] private Color healthyColor = Color.white;
    [SerializeField] private Color needsAttentionColor = new Color(1f, 0.9f, 0.5f);
    [SerializeField] private Color neglectedColor = new Color(0.55f, 0.6f, 0.75f); // Tono apagado/triste
    [SerializeField] private Color recoveringColor = new Color(0.7f, 1f, 0.75f);

    private void Start()
    {
        RefreshVisuals();
    }

    // Conectar a botón UI "Cuidar / Alimentar"
    public void CareForPet()
    {
        if (petData == null) return;

        petData.hunger = Mathf.Clamp(petData.hunger + 25f, 0f, 100f);
        petData.hygiene = Mathf.Clamp(petData.hygiene + 25f, 0f, 100f);
        petData.energy = Mathf.Clamp(petData.energy + 25f, 0f, 100f);

        petData.currentState = (petData.hunger >= 80f)
            ? PetWellbeingState.Bien
            : PetWellbeingState.Recuperacion;

        RefreshVisuals();
        if (SaveManager.Instance != null) SaveManager.Instance.SavePetData();
    }

    // Conectar a botón UI de prueba "Simular Descuido"
    public void SimulateNeglect()
    {
        if (petData == null) return;

        petData.hunger = 20f;
        petData.hygiene = 20f;
        petData.currentState = PetWellbeingState.Descuidada;

        RefreshVisuals();
        if (SaveManager.Instance != null) SaveManager.Instance.SavePetData();
    }

    public void RefreshVisuals()
    {
        if (petData == null || targetSprite == null) return;

        targetSprite.color = petData.currentState switch
        {
            PetWellbeingState.Bien => healthyColor,
            PetWellbeingState.NecesitaAtencion => needsAttentionColor,
            PetWellbeingState.Descuidada => neglectedColor,
            PetWellbeingState.Recuperacion => recoveringColor,
            _ => healthyColor
        };
    }
}