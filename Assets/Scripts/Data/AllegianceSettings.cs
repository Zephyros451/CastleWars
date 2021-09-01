using UnityEngine;

[CreateAssetMenu(menuName = "Data/AllegianceSettings")]
public class AllegianceSettings : ScriptableObject
{
    public TowerData Player;
    public TowerData Neutral;
    public TowerData Enemy;
}
