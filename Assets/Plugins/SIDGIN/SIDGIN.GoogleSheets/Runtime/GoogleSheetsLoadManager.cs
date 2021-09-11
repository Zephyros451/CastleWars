using System.Linq;
using UnityEngine;
namespace SIDGIN.GoogleSheets
{
    using Common;
    using SIDGIN.GoogleSheets.Internal;

    public class GoogleSheetsLoadManager
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            var googleSettings = RuntimeSettingsLoader.Get<GoogleSheetsSettings>();
            if (googleSettings.loadOnStart)
            {
                Load(googleSettings);
            }
        }
        public static void Load(GoogleSheetsSettings googleSettings, ISheetSaver saver = null)
        {
            var loadTypes = InterfaceHelper.GetChildTypesByInterface<IGoogleDataLoader>().ToList();
            var googleManager = new GoogleSheetsManager(googleSettings);
            googleManager.Authorization();
            foreach (var type in loadTypes)
            {
                var loader = (IGoogleDataLoader)System.Activator.CreateInstance(type);
                loader.Load(googleManager, saver);
            }

        }
        public static void Load(ISheetSaver saver = null)
        {
            var googleSettings = RuntimeSettingsLoader.Get<GoogleSheetsSettings>();
            var loadTypes = InterfaceHelper.GetChildTypesByInterface<IGoogleDataLoader>().ToList();
            var googleManager = new GoogleSheetsManager(googleSettings);
            googleManager.Authorization();
            foreach (var type in loadTypes)
            {
                var loader = (IGoogleDataLoader)System.Activator.CreateInstance(type);
                loader.Load(googleManager, saver);
            }

        }
    }
}
