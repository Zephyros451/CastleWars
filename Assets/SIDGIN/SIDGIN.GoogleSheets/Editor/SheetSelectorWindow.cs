using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
namespace SIDGIN.GoogleSheets.Editors
{
    using Common.Editors.Controls;
    using SIDGIN.Common.Editors;
    using SIDGIN.GoogleSheets.Internal;
    using System.Linq;
    using System.Threading.Tasks;


    public struct SheetSelectResult
    {
        public bool IsSuccessful;
        public string TableId;
        public string SheetName;
    }
    public class SheetSelectorParmeters
    {
        public bool hasSheetSelector;
        public bool isOverride;
    }

    public class SheetSelectorWindow : EditorWindow
    {
        Action<SheetSelectResult> onSelectComplete;
        SheetSelectorParmeters settings;
        public static void ShowSelector(Action<SheetSelectResult> onSelectComplete, SheetSelectorParmeters parameters = null)
        {
            EditorDispatcher.Dispatch(() =>
            {
                var window = GetWindow<SheetSelectorWindow>(true, "Select Google Sheet");
                window.minSize = new Vector2(400, 150);
                window.maxSize = window.minSize;
                window.onSelectComplete = onSelectComplete;
                window.settings = parameters;
            });
        }
        void OnEnable()
        {
            LoadSheets();
        }
        void OnDisable()
        {
            if (onSelectComplete != null && !isContinue)
                onSelectComplete.Invoke(new SheetSelectResult
                {
                    IsSuccessful = false
                });
        }
        bool isContinue;
        bool isLoading;
        string[] sheetNames;
        string errorMessage = "";
        string tableLink;
        string tableId;
        string sheetName;
        bool isOverride;
        bool isSuccessful;
        void OnGUI()
        {
            LoadingControl.Begin(isLoading);
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Table Link: ", GUILayout.Width(120));
            EditorGUI.BeginChangeCheck();
            tableLink = EditorGUILayout.TextField(tableLink);
            if (EditorGUI.EndChangeCheck())
            {
                errorMessage = "";
                LoadSheets();

            }
            EditorGUILayout.EndHorizontal();
            if (settings != null)
            {
                if (settings.hasSheetSelector)
                {
                    if (sheetNames != null && sheetNames.Length > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Sheet Name: ", GUILayout.Width(120));
                        EditorGUI.BeginChangeCheck();
                        int index = EditorGUILayout.Popup(Array.IndexOf(sheetNames, sheetName), sheetNames);
                        if (index > 0 && index < sheetNames.Length)
                        {
                            sheetName = sheetNames[index];
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
          
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
            }
            LoadingControl.End(new Rect(0, 0, Screen.width, Screen.height), isLoading);

            if (settings != null && settings.hasSheetSelector)
            {
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(sheetName));

                isOverride = GUILayout.Toggle(isOverride, "Override");
            }
            else
            {
                EditorGUI.BeginDisabledGroup(!isSuccessful);
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Continue", GUILayout.Width(70)))
            {
                isContinue = true;
                if (onSelectComplete != null)
                    onSelectComplete.Invoke(new SheetSelectResult
                    {
                        IsSuccessful = isSuccessful,
                        TableId = tableId,
                        SheetName = sheetName
                    });
                Close();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
            if (isLoading)
            {
                Repaint();
            }

        }
        void LoadSheets()
        {
            if (!string.IsNullOrEmpty(tableLink))
            {
                var regexSpriteSheetId = new Regex("/spreadsheets/d/([a-zA-Z0-9-_]+)");
                tableId = regexSpriteSheetId.Match(tableLink)?.Groups[1].Value;
                var regexSheetId = new Regex("[#&]gid=([0-9]+)");

                var sheetIdString = regexSheetId.Match(tableLink)?.Groups[1].Value;
                int? sheetId = null;
                int sheetIdValue;
                if (int.TryParse(sheetIdString, out sheetIdValue))
                {
                    sheetId = sheetIdValue;
                }
                if (!string.IsNullOrEmpty(tableId))
                {
                    isLoading = true;
                    var persistentDataPath = Application.persistentDataPath;
                    var googleSettings = EditorSettingsLoader.Get<GoogleSheetsSettings>(true);
                    var googleSheetProvider = new GoogleSheetsManager(googleSettings);
                    var task = new Task(() =>
                    {
                        using (googleSheetProvider)
                        {
                            googleSheetProvider.Authorization();
                            var result = googleSheetProvider.GetSheets(tableId, sheetId);
                            if (result.IsSuccessful)
                            {
                                sheetNames = result.sheetNames.ToArray();
                                sheetName = result.sheetName;
                                if (string.IsNullOrEmpty(sheetName))
                                {
                                    sheetName = sheetNames.FirstOrDefault();
                                }
                            }
                            isSuccessful = result.IsSuccessful;
                        }

                    });
                    task.ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            isSuccessful = false;
                            EditorDispatcher.Dispatch(() =>
                            {
                                if (x.Exception == null)
                                    return;
                                if (x.Exception.InnerException.Message.Contains("404"))
                                {
                                    errorMessage = "Sprite Sheet not found. Please check link!";
                                    Debug.LogWarning(x.Exception);
                                }
                                else if (x.Exception.InnerException.Message.Contains("403"))
                                {
                                    errorMessage = "You don't have permission to this table!";
                                    Debug.LogWarning(x.Exception);
                                }
                                else
                                {
                                    errorMessage = "An error occurred while trying to get data from Google.";
                                    Debug.LogError(x.Exception);
                                }
                            });
                        }
                        isLoading = false;
                    });
                    task.Start();
                }
                else
                {
                    errorMessage = "The link is incorrect. Please enter the link like format: https://docs.google.com/spreadsheets/d/1e9999999999999999999999bS7E/edit#gid=777888554";
                }
            }
            else
            {
                sheetNames = null;
            }
        }
    }
}




    