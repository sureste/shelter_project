using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum ScreenType
{
    MainMenu,
    DestinationSelect, // Elegir entre Casa o Refugio
    ExploreArea,       // Dentro de Casa, Pasillo, Gatos o Perros
    MinigameSelect,
    MinigamePlay
}

// Guarda exactamente dónde estaba el usuario para poder volver
public struct NavigationNode
{
    public ScreenType screen;
    public AreaData area;         // Solo aplica si screen == ExploreArea
    public MinigameData minigame; // Solo aplica si screen == MinigamePlay
}

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance { get; private set; }

    [Header("Paneles UI")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject destinationSelectPanel;
    [SerializeField] private GameObject exploreHudPanel; // Contiene el botón "Volver" y acceso a minijuegos
    [SerializeField] private GameObject minigameSelectPanel;
    [SerializeField] private GameObject minigameDummyPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Contenedor del Mundo 2D")]
    [SerializeField] private Transform worldContainer;

    // Historial de navegación
    private Stack<NavigationNode> _history = new Stack<NavigationNode>();
    private GameObject _currentWorldInstance;

    // Evento para avisar a la cámara y otros sistemas cuando cambia el área
    public event Action<AreaData> OnAreaLoaded;
    public event Action<MinigameData> OnMinigameLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Arrancar siempre en el menú principal limpiando historial
        NavigateTo(ScreenType.MainMenu, clearHistory: true);
    }

    // --- MÉTODOS PÚBLICOS DE NAVEGACIÓN ---

    public void GoToDestinationSelect() => NavigateTo(ScreenType.DestinationSelect);

    public void EnterArea(AreaData area)
    {
        NavigateTo(ScreenType.ExploreArea, area: area);
    }

    public void GoToMinigameSelect() => NavigateTo(ScreenType.MinigameSelect);

    public void StartMinigame(MinigameData minigame)
    {
        NavigateTo(ScreenType.MinigamePlay, minigame: minigame);
    }

    public void ToggleSettings(bool show)
    {
        settingsPanel.SetActive(show);
    }

    // Botón "Regresar" universal
    public void GoBack()
    {
        // Necesitamos al menos 2 nodos (el actual y el anterior) para retroceder
        if (_history.Count <= 1) return;

        // Sacamos la pantalla actual
        _history.Pop();

        // Leemos cuál era la anterior sin sacarla todavía
        NavigationNode previous = _history.Peek();
        ApplyNodeState(previous);
    }

    // --- LÓGICA INTERNA ---

    private void NavigateTo(ScreenType targetScreen, AreaData area = null, MinigameData minigame = null, bool clearHistory = false)
    {
        if (clearHistory) _history.Clear();

        NavigationNode newNode = new NavigationNode
        {
            screen = targetScreen,
            area = area,
            minigame = minigame
        };

        _history.Push(newNode);
        ApplyNodeState(newNode);
    }

    private void ApplyNodeState(NavigationNode node)
    {
        // 1. Apagar todos los paneles primero
        mainMenuPanel.SetActive(false);
        destinationSelectPanel.SetActive(false);
        exploreHudPanel.SetActive(false);
        minigameSelectPanel.SetActive(false);
        minigameDummyPanel.SetActive(false);

        // 2. Limpiar el escenario 2D anterior si existía
        if (_currentWorldInstance != null)
        {
            Destroy(_currentWorldInstance);
            _currentWorldInstance = null;
        }

        // 3. Encender lo que corresponde al nodo actual
        switch (node.screen)
        {
            case ScreenType.MainMenu:
                mainMenuPanel.SetActive(true);
                break;

            case ScreenType.DestinationSelect:
                destinationSelectPanel.SetActive(true);
                break;

            case ScreenType.ExploreArea:
                exploreHudPanel.SetActive(true);
                LoadWorldArea(node.area);
                break;

            case ScreenType.MinigameSelect:
                minigameSelectPanel.SetActive(true);
                break;

            case ScreenType.MinigamePlay:
                minigameDummyPanel.SetActive(true);
                OnMinigameLoaded?.Invoke(node.minigame);
                break;
        }
    }

    private void LoadWorldArea(AreaData data)
    {
        if (data == null)
        {
            Debug.LogError("NavigationManager: Intentando cargar un AreaData nulo.");
            return;
        }

        // Instanciar el fondo del escenario dentro del contenedor limpio
        if (data.backgroundPrefab != null)
        {
            _currentWorldInstance = Instantiate(data.backgroundPrefab, worldContainer);
        }

        // Notificar a la cámara (para actualizar minX/maxX) y al spawner de puertas/animales
        OnAreaLoaded?.Invoke(data);
    }
}