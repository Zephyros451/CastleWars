
using System.Collections.Generic;
namespace SIDGIN.GoogleSheets
{
    using Common;
    using SIDGIN.GoogleSheets.Internal;


    public class GoogleSheetsSendManager
    {
        public static void Send(IList<object> objects)
        {
            var googleSettings = RuntimeSettingsLoader.Get<GoogleSheetsSettings>();
            var googleManager = new GoogleSheetsManager(googleSettings);
            googleManager.Authorization();
            GoogleSheetsMapSender.Send(googleManager, objects);

        }
    }
}
