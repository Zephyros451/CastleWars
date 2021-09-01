using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Difficulty Settings")]
public class DifficultySettings : ScriptableObject
{
    public AIBrain.GameDifficulty difficulty;
}
