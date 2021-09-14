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
        visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/MapEditor/MapEditor.uxml");

        root.Clear();
        visualTree.CloneTree(root);

        InitializeTowerList();
        InitializeTowerToolbar();
    }

    private void InitializeTowerToolbar()
    {
        towersParent = FindObjectOfType<TowersParentFlag>();
        pathsParent = FindObjectOfType<PathsParentFlag>();

        var swordsmanTower = AssetDatabase.LoadAssetAtPath<Tower>("Assets/Prefabs/Towers/HumanTowerLI.prefab");
        var spearmanTower = AssetDatabase.LoadAssetAtPath<Tower>("Assets/Prefabs/Towers/HumanTowerHI.prefab");
        var archerTower = AssetDatabase.LoadAssetAtPath<Tower>("Assets/Prefabs/Towers/HumanTowerA.prefab");

        var deleteTowerButton = root.Q<ToolbarButton>("delete-tower");
        deleteTowerButton.clicked += DeleteTower;
        var addTowerMenu = root.Q<ToolbarMenu>("add-tower");

        addTowerMenu.menu.AppendAction("Player/Swordsman", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(swordsmanTower, Allegiance.Player, TowerType.Swordsman));

        addTowerMenu.menu.AppendAction("Player/Spearman", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(spearmanTower, Allegiance.Player, TowerType.Spearman));

        addTowerMenu.menu.AppendAction("Player/Archer", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(archerTower, Allegiance.Player, TowerType.Archer));

        addTowerMenu.menu.AppendAction("Neutral/Swordsman", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(swordsmanTower, Allegiance.Neutral, TowerType.Swordsman));

        addTowerMenu.menu.AppendAction("Neutral/Spearman", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(spearmanTower, Allegiance.Neutral, TowerType.Spearman));

        addTowerMenu.menu.AppendAction("Neutral/Archer", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(archerTower, Allegiance.Neutral, TowerType.Archer));

        addTowerMenu.menu.AppendAction("Enemy/Swordsman", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(swordsmanTower, Allegiance.Enemy, TowerType.Swordsman));

        addTowerMenu.menu.AppendAction("Enemy/Spearman", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(spearmanTower, Allegiance.Enemy, TowerType.Spearman));

        addTowerMenu.menu.AppendAction("Enemy/Archer", CreateTower,
                                       (a) => { return DropdownMenuAction.Status.Normal; },
                                       new TowerUserData(archerTower, Allegiance.Enemy, TowerType.Archer));
    }

    private void CreateTower(DropdownMenuAction obj)
    {
        var userData = obj.userData as TowerUserData;
        var instance = PrefabUtility.InstantiatePrefab(userData.Tower, towersParent.transform) as Tower;
        instance.Initialize(userData.TowerType, userData.Allegiance);
        instance.name = $"{instance.Allegiance} {instance.TowerType} Tower";

        InitializeTowerList();
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
        currentTower = (Tower)obj;
    }

    private class TowerUserData
    {
        public Tower Tower;
        public Allegiance Allegiance;
        public TowerType TowerType;

        public TowerUserData(Tower tower, Allegiance allegiance, TowerType towerType)
        {
            Tower = tower;
            Allegiance = allegiance;
            TowerType = towerType;
        }
    }
}