using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private DifficultySettings difficultySettings;
    [SerializeField] private TMP_Dropdown dropdown;

    private bool isEnabled;

    public void TogglePanel()
    {
        if(isEnabled)
        {
            settingsPanel.SetActive(false);            
        }
        else
        {
            settingsPanel.SetActive(true);
        }

        isEnabled = !isEnabled;
    }

    public void UpdateDifficulty()
    {
        switch(dropdown.value)
        {
            case 0:
                difficultySettings.difficulty = AIBrain.GameDifficulty.Easy;
                break;
            case 1:
                difficultySettings.difficulty = AIBrain.GameDifficulty.Normal;
                break;
            case 2:
                difficultySettings.difficulty = AIBrain.GameDifficulty.Hard;
                break;
        }
    }
}
