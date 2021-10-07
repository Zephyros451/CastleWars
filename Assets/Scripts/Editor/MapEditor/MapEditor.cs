using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class MapEditor : EditorWindow
{
    private VisualTreeAsset visualTree;
    private VisualElement root;
    private Editor editor;
    private Tower[] towers;
    private ListView towerList;
    private ListView pathsList;
    private ToolbarButton deleteTowerButton;

    private TowersParentFlag towersParent;
    private PathsParentFlag pathsParent;

    private Tower currentTower;
    private Path currentPath;

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

    private void InitializeTowerList()
    {
        towers = FindObjectsOfType<Tower>();
        Array.Reverse(towers);

        if (towerList != null)
        {
            towerList.onSelectionChanged -= SelectTower;
            towerList.onSelectionChanged -= InitializePathsList;
            towerList.onSelectionChanged -= InitializePathsToolbar;
        }

        towerList = root.Q<ListView>("tower-list");
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = towers[i].name;
        int itemHeight = 15;

        towerList.itemsSource = towers;
        towerList.itemHeight = itemHeight;
        towerList.makeItem = makeItem;
        towerList.bindItem = bindItem;
        towerList.selectionType = SelectionType.Single;

        towerList.onSelectionChanged += SelectTower;
        towerList.onSelectionChanged += InitializePathsList;
        towerList.onSelectionChanged += InitializePathsToolbar;
    }

    private void InitializeTowerToolbar()
    {
        towersParent = FindObjectOfType<TowersParentFlag>();

        var swordsmanTowerPrefab = AssetDatabase.LoadAssetAtPath<Tower>("Assets/Prefabs/Towers/HumanTowerLI.prefab");
        var spearmanTowerPrefab = AssetDatabase.LoadAssetAtPath<Tower>("Assets/Prefabs/Towers/HumanTowerHI.prefab");
        var archerTowerPrefab = AssetDatabase.LoadAssetAtPath<Tower>("Assets/Prefabs/Towers/HumanTowerA.prefab");

        deleteTowerButton = root.Q<ToolbarButton>("delete-tower");
        deleteTowerButton.clicked += DeleteTower;
        var addTowerMenu = root.Q<ToolbarMenu>("add-tower");

        Func<DropdownMenuAction, DropdownMenuAction.Status> func = (a) => { return DropdownMenuAction.Status.Normal; };

        addTowerMenu.menu.AppendAction("Player/Swordsman", CreateTower, func,
                                       new TowerUserData(swordsmanTowerPrefab, Allegiance.Player, TowerType.Swordsman));

        addTowerMenu.menu.AppendAction("Player/Spearman", CreateTower, func,
                                       new TowerUserData(spearmanTowerPrefab, Allegiance.Player, TowerType.Spearman));

        addTowerMenu.menu.AppendAction("Player/Archer", CreateTower, func,
                                       new TowerUserData(archerTowerPrefab, Allegiance.Player, TowerType.Archer));

        addTowerMenu.menu.AppendAction("Neutral/Swordsman", CreateTower, func,
                                       new TowerUserData(swordsmanTowerPrefab, Allegiance.Neutral, TowerType.Swordsman));

        addTowerMenu.menu.AppendAction("Neutral/Spearman", CreateTower, func,
                                       new TowerUserData(spearmanTowerPrefab, Allegiance.Neutral, TowerType.Spearman));

        addTowerMenu.menu.AppendAction("Neutral/Archer", CreateTower, func,
                                       new TowerUserData(archerTowerPrefab, Allegiance.Neutral, TowerType.Archer));

        addTowerMenu.menu.AppendAction("Enemy/Swordsman", CreateTower, func,
                                       new TowerUserData(swordsmanTowerPrefab, Allegiance.Enemy, TowerType.Swordsman));

        addTowerMenu.menu.AppendAction("Enemy/Spearman", CreateTower, func,
                                       new TowerUserData(spearmanTowerPrefab, Allegiance.Enemy, TowerType.Spearman));

        addTowerMenu.menu.AppendAction("Enemy/Archer", CreateTower, func,
                                       new TowerUserData(archerTowerPrefab, Allegiance.Enemy, TowerType.Archer));
    }

    private void CreateTower(DropdownMenuAction obj)
    {
        var userData = obj.userData as TowerUserData;
        var instance = PrefabUtility.InstantiatePrefab(userData.TowerPrefab, towersParent.transform) as Tower;
        instance.Initialize(userData.TowerType, userData.Allegiance);
        instance.name = $"Tower {towers.Length + 1}";

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

    private void SelectTower(List<object> obj)
    {
        currentTower = (Tower)obj[0];
    }
    
    private void InitializePathsList(List<object> obj = null)
    {
        if (currentTower == null)
            return;

        if (pathsList != null)
        {
            pathsList.onSelectionChanged -= SelectPath;
        }

        pathsList = root.Q<ListView>("path-list");
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = currentTower.Mediator.Navigator.Paths[i].name;
        int itemHeight = 15;

        pathsList.itemsSource = currentTower.Mediator.Navigator.Paths;
        pathsList.itemHeight = itemHeight;
        pathsList.makeItem = makeItem;
        pathsList.bindItem = bindItem;
        pathsList.selectionType = SelectionType.Single;

        pathsList.onSelectionChanged += SelectPath;
    }

    private void InitializePathsToolbar(List<object> obj = null)
    {
        pathsParent = FindObjectOfType<PathsParentFlag>();

        var pathPrefab = AssetDatabase.LoadAssetAtPath<Path>("Assets/Prefabs/Path.prefab");

        var deletePathButton = root.Q<ToolbarButton>("delete-path");
        deletePathButton.clicked += DeletePath;
        var addPathMenu = root.Q<ToolbarMenu>("add-path");

        Func<DropdownMenuAction, DropdownMenuAction.Status> func = (a) => { return DropdownMenuAction.Status.Normal; };

        addPathMenu.menu.MenuItems().Clear();
        foreach (var tower in currentTower.Mediator.Navigator.NotConnectedTowers)
        {
            addPathMenu.menu.AppendAction($"{tower.name}", CreatePath, func, new PathUserData(pathPrefab, currentTower, tower));
        }
    }

    private void CreatePath(DropdownMenuAction obj)
    {
        var userData = obj.userData as PathUserData;
        var instance = PrefabUtility.InstantiatePrefab(userData.PathPrefab, pathsParent.transform) as Path;
        instance.transform.localPosition = new Vector3(0f, 0.01f, 0f);
        instance.Initialize(userData.Tower1, userData.Tower2);
        instance.name = $"{userData.Tower1.name} <-> {userData.Tower2.name}";

        InitializePathsList();
        InitializePathsToolbar();
        EditorUtility.SetDirty(userData.Tower1.Mediator.Navigator);
        EditorUtility.SetDirty(userData.Tower2.Mediator.Navigator);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private void DeletePath()
    {
        if (currentPath != null)
        {
            currentPath.Destroy();
            DestroyImmediate(currentPath.gameObject);
            InitializePathsList();
        }
    }

    private void SelectPath(List<object> obj)
    {
        currentPath = (Path)obj[0];
    }

    private class TowerUserData
    {
        public Tower TowerPrefab { get; }
        public Allegiance Allegiance { get; }
        public TowerType TowerType { get; }

        public TowerUserData(Tower tower, Allegiance allegiance, TowerType towerType)
        {
            TowerPrefab = tower;
            Allegiance = allegiance;
            TowerType = towerType;
        }
    }

    private class PathUserData
    {
        public Path PathPrefab { get; }
        public Tower Tower1 { get; }
        public Tower Tower2 { get; }

        public PathUserData(Path pathPrefab, Tower tower1, Tower tower2)
        {
            PathPrefab = pathPrefab;
            Tower1 = tower1;
            Tower2 = tower2;
        }
    }

    private void OnDisable()
    {
        towerList.onSelectionChanged -= SelectTower;
        towerList.onSelectionChanged -= InitializePathsList;
        towerList.onSelectionChanged -= InitializePathsToolbar;
        deleteTowerButton.clicked -= DeleteTower;
    }
}