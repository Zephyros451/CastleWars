namespace SIDGIN.GoogleSheets.Editors
{
    using Internal;
    using SIDGIN.Common.Editors;
    using UnityEditor;
    using UnityEngine;

    internal static class GoogleSheetsLoadManagerEditor
    {
        [UnityEditor.MenuItem("Tools/SIDGIN/Google Sheets/Load from Google")]
        public static void LoadFromEditor()
        {
            var googleSettings = EditorSettingsLoader.Get<GoogleSheetsSettings>(true);
            GoogleSheetsLoadManager.Load(googleSettings, new ScriptableObjectSaver());
        }

    }
    internal class ScriptableObjectSaver : ISheetSaver
    {
        public void Save(ScriptableObject obj)
        {
            EditorUtility.SetDirty(obj);
        }
    }
}