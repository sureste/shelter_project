using UnityEngine;

public class HomeCareUI : MonoBehaviour
{
    [Header("Contenedor de los 2 botones")]
    [SerializeField] private GameObject careButtonsContainer;

    [Header("ID exacto de tu Area_Casa")]
    [SerializeField] private string homeAreaID = "house";

    private void Start()
    {
        if (NavigationManager.Instance != null)
            NavigationManager.Instance.OnAreaLoaded += OnAreaChanged;
    }

    private void OnDestroy()
    {
        if (NavigationManager.Instance != null)
            NavigationManager.Instance.OnAreaLoaded -= OnAreaChanged;
    }

    private void OnAreaChanged(AreaData area)
    {
        if (careButtonsContainer == null) return;

        // Solo muestra los botones si entraste a Casa
        bool isHome = area != null && area.areaID.ToLower() == homeAreaID.ToLower();
        careButtonsContainer.SetActive(isHome);
    }

    // --- MÉTODOS PARA CONECTAR EN EL INSPECTOR DE LOS BOTONES ---

    public void OnCareButtonClicked()
    {
        PetStateController activePet = FindFirstObjectByType<PetStateController>();
        if (activePet != null) activePet.CareForPet();
    }

    public void OnNeglectButtonClicked()
    {
        PetStateController activePet = FindFirstObjectByType<PetStateController>();
        if (activePet != null) activePet.SimulateNeglect();
    }
}