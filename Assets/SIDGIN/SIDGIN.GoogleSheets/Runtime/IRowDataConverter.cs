using System.Collections.Generic;
namespace SIDGIN.GoogleSheets
{
    public interface IRowDataConverter<T>
    {
        IList<T> Convert(IList<IList<object>> table);
        IList<IList<object>> Convert(IList<T> data);
    }

    public interface IRowDataConverter
    {
        IList<object> Convert(IList<IList<object>> table, System.Type outObjectType);
        IList<IList<object>> Convert(IList<object> data);
    }
}