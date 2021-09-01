using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Image level1;
    [SerializeField] private Image level2;
    [SerializeField] private Image level3;

    private int level = 1;

    public void Select(Image newImage)
    {
        if(newImage == level1)
        {
            level1.DOFade(1f, 0f);
            level2.DOFade(0f, 0f);
            level3.DOFade(0f, 0f);
            level = 1;
        }
        if(newImage == level2)
        {
            level1.DOFade(0f, 0f);
            level2.DOFade(1f, 0f);
            level3.DOFade(0f, 0f);
            level = 2;
        }
        if (newImage == level3) 
        {
            level1.DOFade(0f, 0f);
            level2.DOFade(0f, 0f);
            level3.DOFade(1f, 0f);
            level = 3;
        }
    }

    public void StartGame()
    {
        SceneSwitcher.instance.SwitchTo(level + 1);
    }
}
