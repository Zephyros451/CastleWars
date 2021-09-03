namespace SIDGIN.GoogleSheets.Internal.Exceptions
{
    public class CredentialsNotFound : System.Exception
    {
        public CredentialsNotFound() : base("No credentials specified for working with GoogleSheets! \nPlease import credentials:\nTools->SIDGIN->Google Sheets->Settings")
        {

        }
    }
}
