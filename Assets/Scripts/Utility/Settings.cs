using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "New Settings Data Object", menuName = "Settings/Settings Data Object")]
public class Settings : ScriptableObject
{
    [SerializeField] public Color activeSelectionColour;
    [SerializeField] public Color inactiveSelectionColour;
    [SerializeField] public Color allySelectionColour;
    [SerializeField] public Color enemySelectionColour;
    [SerializeField] public Color aetherResourceColour;
    [SerializeField] public Color readinessResourceColour;
    [SerializeField] public Color healthResourceColour;
    [SerializeField] public Color manaResourceColour;
}
