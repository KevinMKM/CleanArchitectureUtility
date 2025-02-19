using System.Data;
using CleanArchitectureUtility.Extensions.Abstractions.Translations;

namespace CleanArchitectureUtility.Extensions.Serializers.EPPlus.Extensions;

public static class LinqExtensions
{
    public static List<T> ToList<T>(this DataTable dataTable, ITranslator translator)
    {
        List<T> data = new();
        foreach (DataRow row in dataTable.Rows) 
            data.Add(GetItem<T>(row, translator));

        return data;
    }

    private static T GetItem<T>(DataRow dr, ITranslator translator)
    {
        var temp = typeof(T);
        var obj = Activator.CreateInstance<T>();
        foreach (DataColumn column in dr.Table.Columns)
        {
            foreach (var pro in temp.GetProperties())
                if (pro.Name == column.ColumnName || translator[pro.Name] == column.ColumnName)
                    pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], pro.PropertyType), null);
        }

        return obj;
    }
}