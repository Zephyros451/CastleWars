using UnityEngine;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Threading;
using Google.Apis.Util.Store;
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
namespace SIDGIN.GoogleSheets
{
    using Google;
    using Google.Apis.Auth.OAuth2;
    using Internal;
    using Internal.Exceptions;
    using System.Linq;

    public sealed class GoogleSheetsManager : System.IDisposable
    {

        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        SheetsService service;
        static string tokenPath;
        GoogleSheetsSettings settings;
        public GoogleSheetsManager(GoogleSheetsSettings settings)
        {
            this.settings = settings;
            tokenPath = Path.Combine(Application.dataPath.Replace("Assets", ""), "sg_googlesheets_token");
        }
        public void Authorization()
        {
            var credential = DoAuthorization();
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = settings.ApplicationName,
            });
        }
        public IList<TResult> LoadData<TResult>(string tableName, IRowDataConverter<TResult> dataConverter, string sheetName = "") where TResult : class
        {
            string tableId = settings.GetTableId(tableName);
            string range = "A:AA";
            if (!string.IsNullOrEmpty(sheetName))
                range = string.Format("{0}!A:AA", sheetName);
            SpreadsheetsResource.ValuesResource.GetRequest request =
                                    service.Spreadsheets.Values.Get(tableId, range);
            try
            {
                ValueRange response = request.Execute();
                var values = response.Values;

                if (values != null && values.Count > 0)
                {
                    return dataConverter.Convert(values);
                }
                else
                {
                    throw new System.Exception("Table is empty.");
                }
            }
            catch (GoogleApiException ex)
            {
                if(ex.Error is Google.Apis.Requests.RequestError)
                {
                    var requestError = ex.Error as Google.Apis.Requests.RequestError;
                    if(requestError != null)
                    {
                        if(requestError.Code == 400 && requestError.Message.Contains("Unable to parse range"))
                        {
                            Debug.LogError($"Sheet {sheetName} not found in Google Table. Please check sheet name and try again.");
                            return null;
                        }
                    }
                }
                throw ex;
            }
        }
        public IList<object> LoadData(string tableName, IRowDataConverter dataConverter, System.Type outObjectType, string sheetName = "")
        {
            string tableId = settings.GetTableId(tableName);
            string range = "A:AA";
            if (!string.IsNullOrEmpty(sheetName))
                range = string.Format("{0}!A:AA", sheetName);
            SpreadsheetsResource.ValuesResource.GetRequest request =
                                    service.Spreadsheets.Values.Get(tableId, range);
            try
            {
                ValueRange response = request.Execute();
                var values = response.Values;

                if (values != null && values.Count > 0)
                {
                    return dataConverter.Convert(values, outObjectType);
                }
                else
                {
                    throw new System.Exception("Table is empty.");
                }
            }
            catch (GoogleApiException ex)
            {
                if (ex.Error is Google.Apis.Requests.RequestError)
                {
                    var requestError = ex.Error as Google.Apis.Requests.RequestError;
                    if (requestError != null)
                    {
                        if (requestError.Code == 400 && requestError.Message.Contains("Unable to parse range"))
                        {
                            Debug.LogError($"Sheet {sheetName} not found in Google Table. Please check sheet name and try again.");
                            return null;
                        }
                    }
                }
                throw ex;
            }
        }

        public IList<string> LoadHeaders(string tableName, string sheetName = "")
        {
            string tableId = settings.GetTableId(tableName);
            string range = "A:AA";
            if (!string.IsNullOrEmpty(sheetName))
                range = string.Format("{0}!A:AA", sheetName);
            SpreadsheetsResource.ValuesResource.GetRequest request =
                                    service.Spreadsheets.Values.Get(tableId, range);
            try
            {
                ValueRange response = request.Execute();
                var values = response.Values;

                if (values != null && values.Count > 0)
                {
                    return values[0].Cast<string>().ToList();
                }
                else
                {
                    throw new System.Exception("Table is empty.");
                }
            }
            catch (GoogleApiException ex)
            {
                if (ex.Error is Google.Apis.Requests.RequestError)
                {
                    var requestError = ex.Error as Google.Apis.Requests.RequestError;
                    if (requestError != null)
                    {
                        if (requestError.Code == 400 && requestError.Message.Contains("Unable to parse range"))
                        {
                            Debug.LogError($"Sheet {sheetName} not found in Google Table. Please check sheet name and try again.");
                            return null;
                        }
                    }
                }
                throw ex;
            }
        }

        public void SendData<TData>(string tableName, IRowDataConverter<TData> dataConverter, IList<TData> sendData, string range = "A:W") where TData : class
        {
            string tableId = settings.GetTableId(tableName);
            ValueRange body = new ValueRange();
            body.Values = dataConverter.Convert(sendData);
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            SpreadsheetsResource.ValuesResource.UpdateRequest request =
                                    service.Spreadsheets.Values.Update(body, tableId, range);

            request.ValueInputOption = valueInputOption;
            request.Execute();
        }
        public void SendData(string tableName, IRowDataConverter dataConverter, IList<object> sendData, string range = "A:W")
        {
            string tableId = settings.GetTableId(tableName);
            ValueRange body = new ValueRange();
            body.Values = dataConverter.Convert(sendData);
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption =
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

            SpreadsheetsResource.ValuesResource.UpdateRequest request =
                                    service.Spreadsheets.Values.Update(body, tableId, range);

            request.ValueInputOption = valueInputOption;
            request.Execute();
        }
        UserCredential DoAuthorization()
        {
            var credentialsData = settings.Credentials;
            ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
            UserCredential credential;
            using (var stream = GetStreamData(credentialsData))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(tokenPath, true)).Result;
                Debug.Log("Credential file saved to: " + tokenPath);
            }
            return credential;
        }

        public GoogleGetSheetsResult GetSheets(string spreadSheetId, int? sheetId)
        {
            var result = new GoogleGetSheetsResult();
            var response = service.Spreadsheets.Get(spreadSheetId).Execute();
            if (response != null && response.Sheets != null && response.Sheets.Count > 0)
            {

                result.sheetNames = new List<string>();
                foreach (Sheet sheet in response.Sheets)
                {
                    if (sheetId.HasValue && sheet.Properties.SheetId == sheetId)
                    {
                        result.sheetName = sheet.Properties.Title;
                    }
                    result.sheetNames.Add(sheet.Properties.Title);
                }
                result.IsSuccessful = true;
                return result;
            }
            else
            {
                result.IsSuccessful = false;
                return result;
            }
        }

        public void Dispose()
        {
            if (service != null)
                service.Dispose();
        }
        static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            // If there are errors in the certificate chain, look at each error to determine the cause.
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid)
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        static Stream GetStreamData(string credentialsData)
        {
            if (string.IsNullOrEmpty(credentialsData))
                throw new CredentialsNotFound();

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(credentialsData);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }
}