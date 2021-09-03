namespace SIDGIN.GoogleSheets
{
    public interface IValueConverter
    {
        System.Type Type { get; }
        object Convert(string input, System.Type type);

        string Convert(object input);
    }

}

