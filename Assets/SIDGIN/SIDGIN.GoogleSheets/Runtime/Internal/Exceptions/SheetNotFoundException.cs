namespace SIDGIN.GoogleSheets.Internal.Exceptions
{
    public class SheetNotFoundException : System.Exception
    {
        public SheetNotFoundException(string sheetName) : base($"Sheet {sheetName} not found in Google Table. Please check sheet name and try again.")
        {

        }
    }
}
