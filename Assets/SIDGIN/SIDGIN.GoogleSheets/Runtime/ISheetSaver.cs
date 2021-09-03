using UnityEngine;

namespace SIDGIN.GoogleSheets
{
    public interface ISheetSaver
    {
        void Save(ScriptableObject obj);
    }
}
