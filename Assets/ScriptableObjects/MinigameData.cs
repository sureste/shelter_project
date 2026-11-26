using UnityEngine;

[CreateAssetMenu(fileName = "NewMinigame", menuName = "shelter/Minigame Data")]
public class MinigameData : ScriptableObject
{
    public string minigameID;
    public string title;
    [TextArea] public string placeholderDescription;
    public Sprite icon;
}
