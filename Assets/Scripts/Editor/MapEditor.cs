using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;

public class MapEditor : EditorWindow
{
    private VisualTreeAsset visualTree;
    private VisualElement root;
    private Editor editor;
    private Tower[] towers;
    private Dictionary<string, Tower> references = new Dictionary<string, Tower>();

    private TowersParentFlag towersParent;
    private PathsParentFlag pathsParent;

    private Tower currentTower;

    [MenuItem("Tools/MapEditor")]
    public static void OpenMapEditorWindow()
    {
        MapEditor window = GetWindow<MapEditor>();
        window.titleContent = new GUIContent("MapEditor");
    }

    public void OnEnable()
    {
        root = rootVisualElement;
        visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/MapEditor.uxml");

        root.Clear();
        visualTree.CloneTree(root);

        InitializeTowerList();

        towersParent = FindObjectOfType<TowersParentFlag>();
        pathsParent = FindObjectOfType<PathsParentFlag>();

        var deleteTowerButton = root.Q<ToolbarButton>("delete-tower");
        deleteTowerButton.clicked += DeleteTower;
    }

    private void DeleteTower()
    {
        if (currentTower != null)
        {
            DestroyImmediate(currentTower.gameObject);
            InitializeTowerList();
        }
    }

    private void InitializeTowerList()
    {
        towers = FindObjectsOfType<Tower>();
        foreach(var tower in towers)
        {
            references.Add(tower.name, tower);
        }

        var towerList = root.Q<ListView>("tower-list");
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = towers[i].name;
        int itemHeight = 15;
        towerList.itemsSource = towers;
        towerList.itemHeight = itemHeight;
        towerList.makeItem = makeItem;
        towerList.bindItem = bindItem;
        towerList.selectionType = SelectionType.Single;

        towerList.onItemChosen += SelectTower;
    }

    private void SelectTower(object obj)
    {
        //currentTower = references[(Label)obj];
    }
}