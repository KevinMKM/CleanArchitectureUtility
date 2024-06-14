using System.Data;
using CleanArchitectureUtility.Core.Abstractions.Translations;

namespace CleanArchitectureUtility.Utilities.Serializers.EPPlus;

public static class LinqX
{
    public static List<T> ToList<T>(this DataTable dataTable, ITranslator translator)
    {
        var list = new List<T>();
        foreach (DataRow row in dataTable.Rows) 
            list.Add(GetItem<T>(row, translator));

        return list;
    }

    private static T GetItem<T>(DataRow dr, ITranslator translator)
    {
        var temp = typeof(T);
        var obj = Activator.CreateInstance<T>();
        foreach (DataColumn column in dr.Table.Columns)
        foreach (var pro in temp.GetProperties())
            if (pro.Name == column.ColumnName || translator.GetString(pro.Name) == column.ColumnName)
                pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], pro.PropertyType), null);
            else
                continue;

        return obj;
    }
}