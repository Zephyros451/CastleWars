using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace SIDGIN.GoogleSheets.Internal
{
    using Common;
    [System.Serializable]
    public class GoogleSheetTable
    {
        public string name;
        public string id;
        public bool isEditMode;
        public bool isVerified;
    }
    public class GoogleSheetsSettings : ScriptableObject
    {
        [SerializeField]
        string applicationName = "SG Google Sheets";
        public string ApplicationName { get { return applicationName; } }

        [HideInInspector, SerializeField]
        string credentialsData = "{\"installed\":{\"client_id\":\"811653934009-r55vkv03tdc91jfb0scjg77kjmmiaeah.apps.googleusercontent.com\",\"project_id\":\"concrete-sol-270608\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"g-P9EUuOevXi_2OBN76Km-sS\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}";

        public string Credentials { get { return credentialsData; } set { credentialsData = value; } }

        public bool loadOnStart;

        public List<GoogleSheetTable> tables = new List<GoogleSheetTable>();

        public string GetTableId(string tableName)
        {
            var table = tables.Find(x => x.name == tableName);
            if (table == null)
            {
                throw new System.ArgumentException($"No settings for table: {tableName}.");
            }
            return table.id;
        }
    }
}