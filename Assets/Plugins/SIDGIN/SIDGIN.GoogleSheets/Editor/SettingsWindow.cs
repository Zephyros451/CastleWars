using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace SIDGIN.GoogleSheets.Editors
{
    using Internal;
    using Common.Editors;
    using Common.Editors.Controls;
    using System.IO;
    using System.Linq;

    internal class SettingsWindow : EditorWindow
    {
        [MenuItem("Tools/SIDGIN/Google Sheets/Settings")]
        static void ShowWindow()
        {
            var window = GetWindow<SettingsWindow>();
            window.titleContent = new GUIContent("Sheets Settings", "SG Google Sheets Settings");

        }
        GoogleSheetsSettings settings;
        void OnEnable()
        {
            settings = EditorSettingsLoader.Get<GoogleSheetsSettings>(true);
        }
        void OnDisable()
        {
            if (settings != null)
            {
                settings.Save();
            }
        }
        Vector3 scroll;
        string newTableName;
        private void OnGUI()
        {
            if (settings == null)
                OnEnable();
            LoadingControl.Begin(isBusy);
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("AC BoldHeader");
            settings.loadOnStart = EditorGUILayout.ToggleLeft("Load Sheets On Game Start", settings.loadOnStart);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import credentials"))
            {
                ImportCredentials();
            }
            var token = Path.Combine(Application.dataPath.Replace("Assets", ""), "sg_googlesheets_token");
            if (Directory.Exists(token) && GUILayout.Button("Reset Authorization"))
            {
                Directory.Delete(token, true);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Tables", new GUIStyle("AM MixerHeader"), GUILayout.Width(80));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();


            EditorGUILayout.BeginHorizontal("AC BoldHeader");
            EditorGUILayout.LabelField("New table name:", GUILayout.Width(100));

            newTableName = EditorGUILayout.TextField(newTableName);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("", "OL Plus"))
            {
                if (!settings.tables.Any(x => x.name == newTableName))
                {
                    AddNewTable();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "The table name already exist!", "OK");
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical("GameViewBackground");
            scroll = EditorGUILayout.BeginScrollView(scroll);
            foreach (var table in settings.tables.ToArray())
            {
                DrawTableSettings(table);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            LoadingControl.End(new Rect(0, 0, Screen.width, Screen.height), isBusy);
        }
        string cacheName;
        bool isBusy;
        void DrawTableSettings(GoogleSheetTable tableSettings)
        {
            EditorGUILayout.BeginHorizontal("AC BoldHeader");
            if (tableSettings.isEditMode)
            {
                tableSettings.name = EditorGUILayout.TextField(tableSettings.name);
            }
            else
            {
                GUILayout.Label(tableSettings.name);
            }
            GUILayout.FlexibleSpace();
            if (tableSettings.isVerified)
            {
                var guiSkin = new GUIStyle(GUI.skin.label);
                guiSkin.normal.textColor = new Color(0.145f, 0.572f, 0.219f);
                GUILayout.Label("Table linked", guiSkin);
            }
            else
            {
                var guiSkin = new GUIStyle(GUI.skin.label);
                guiSkin.normal.textColor = new Color(0.749f, 0.227f, 0.188f);
                GUILayout.Label("Table no link", guiSkin);
            }
            if (tableSettings.isEditMode)
            {
                if (GUILayout.Button("Change Table"))
                {
                    SelectLink(tableSettings);
                }
            }

            if (GUILayout.Button(tableSettings.isEditMode ? "Complete" : "Edit"))
            {
                if (!tableSettings.isEditMode)
                {
                    cacheName = tableSettings.name;
                    tableSettings.isEditMode = !tableSettings.isEditMode;
                }
                else
                {
                    if (cacheName != tableSettings.name && settings.tables.Any(x => x != tableSettings && x.name == tableSettings.name))
                    {
                        EditorUtility.DisplayDialog("Error", "The table name already exist!", "OK");
                    }
                    else
                    {
                        tableSettings.isEditMode = !tableSettings.isEditMode;
                    }
                }

            }
            if (GUILayout.Button("Delete"))
            {
                settings.tables.Remove(tableSettings);
            }
            EditorGUILayout.EndHorizontal();
        }

        void SelectLink(GoogleSheetTable table)
        {
            isBusy = true;
            SheetSelectorWindow.ShowSelector(result =>
            {
                if (result.IsSuccessful)
                {
                    table.id = result.TableId;
                    table.isVerified = true;
                }
                isBusy = false;
            });
            settings?.Save();
        }
        void AddNewTable()
        {
            if (settings?.tables == null)
            {
                settings.tables = new List<GoogleSheetTable>();
            }
            var table = new GoogleSheetTable();
            settings?.tables?.Add(table);
            SelectLink(table);
            table.isEditMode = true;
            table.name = newTableName;


        }

        static void ImportCredentials()
        {
            var settings = EditorSettingsLoader.Get<GoogleSheetsSettings>(true);
            var file = EditorUtility.OpenFilePanel("Select credentials.json", Application.dataPath, "json");
            if (!string.IsNullOrEmpty(file))
            {
                var credentialsData = File.ReadAllText(file);
                settings.Credentials = credentialsData;
                settings.Save();
            }
        }

    }
}