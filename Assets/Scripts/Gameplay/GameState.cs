using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState instance;

    public event Action YouWin;
    public event Action YouLose;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void RaiseYouWin()
    {
        YouWin?.Invoke();
        SceneSwitcher.instance.SwitchTo(11);
    }

    public void RaiseYouLose()
    {
        YouLose?.Invoke();
        SceneSwitcher.instance.SwitchTo(11);
    }
}
