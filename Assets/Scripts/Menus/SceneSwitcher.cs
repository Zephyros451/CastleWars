using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SwitchTo(int index)
    {
        SceneManager.LoadScene(index);
    }
}

public enum SceneIndex
{
    Menu, Levels, Gameplay, Result
}
