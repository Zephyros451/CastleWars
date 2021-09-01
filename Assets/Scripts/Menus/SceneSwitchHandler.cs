using UnityEngine;

public class SceneSwitchHandler : MonoBehaviour
{
    [SerializeField] private int sceneIndex;

    public void Switch()
    {
        SceneSwitcher.instance.SwitchTo(sceneIndex);
    }
}
