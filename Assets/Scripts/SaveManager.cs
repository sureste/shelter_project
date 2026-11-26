using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("Referencia al PetStateData de Casa")]
    [SerializeField] private PetStateData homePetData;

    private const string KEY_PET_STATE = "Refugio_PetState";
    private const string KEY_PET_HUNGER = "Refugio_PetHunger";
    private const string KEY_PET_HYGIENE = "Refugio_PetHygiene";
    private const string KEY_PET_ENERGY = "Refugio_PetEnergy";
    private const string KEY_VOLUME = "Refugio_MasterVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadData();
    }

    public void SavePetData()
    {
        if (homePetData == null) return;

        PlayerPrefs.SetInt(KEY_PET_STATE, (int)homePetData.currentState);
        PlayerPrefs.SetFloat(KEY_PET_HUNGER, homePetData.hunger);
        PlayerPrefs.SetFloat(KEY_PET_HYGIENE, homePetData.hygiene);
        PlayerPrefs.SetFloat(KEY_PET_ENERGY, homePetData.energy);
        PlayerPrefs.Save();
    }

    public void SaveVolume(float volume)
    {
        AudioListener.volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(KEY_VOLUME, AudioListener.volume);
        PlayerPrefs.Save();
    }

    public float GetSavedVolume()
    {
        return PlayerPrefs.GetFloat(KEY_VOLUME, 1f);
    }

    public void LoadData()
    {
        AudioListener.volume = GetSavedVolume();

        if (homePetData == null) return;

        if (PlayerPrefs.HasKey(KEY_PET_STATE))
        {
            homePetData.currentState = (PetWellbeingState)PlayerPrefs.GetInt(KEY_PET_STATE);
            homePetData.hunger = PlayerPrefs.GetFloat(KEY_PET_HUNGER, 100f);
            homePetData.hygiene = PlayerPrefs.GetFloat(KEY_PET_HYGIENE, 100f);
            homePetData.energy = PlayerPrefs.GetFloat(KEY_PET_ENERGY, 100f);
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // En Android guarda automáticamente si el usuario minimiza la app
        if (pauseStatus) SavePetData();
    }

    private void OnApplicationQuit()
    {
        SavePetData();
    }
}