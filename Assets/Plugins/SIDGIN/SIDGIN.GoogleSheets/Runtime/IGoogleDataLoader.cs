namespace SIDGIN.GoogleSheets
{
    public interface IGoogleDataLoader
    {
        void Load(GoogleSheetsManager manager, ISheetSaver saver = null);
    }
}