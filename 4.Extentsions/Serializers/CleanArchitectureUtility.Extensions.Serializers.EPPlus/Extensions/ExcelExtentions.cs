using System.Data;
using System.Drawing;
using CleanArchitectureUtility.Extensions.Abstractions.Translations;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CleanArchitectureUtility.Extensions.Serializers.EPPlus.Extensions;

public static class ExcelExtensions
{
    public static byte[] ToExcelByteArray<T>(this List<T> list, ITranslator translator, string sheetName = "Result")
    {
        using var excelPackage = new ExcelPackage();
        var ws = excelPackage.Workbook.Worksheets.Add(sheetName);
        var t = typeof(T);
        var headings = t.GetProperties();
        for (var i = 0; i < headings.Length; i++)
            ws.Cells[1, i + 1].Value = translator[headings[i].Name];

        if (list.Any())
            ws.Cells["A2"].LoadFromCollection(list);

        using (var rng = ws.Cells["A1:BZ1"])
        {
            rng.Style.Font.Bold = true;
            rng.Style.Fill.PatternType = ExcelFillStyle.Solid; //Set Pattern for the background to Solid
            rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189)); //Set color to dark blue
            rng.Style.Font.Color.SetColor(Color.White);
        }

        var array = excelPackage.GetAsByteArray();
        return array;
    }

    public static DataTable ToDataTableFromExcel(this byte[] bytes)
    {
        var excelPackage = new ExcelPackage();
        using (var memoryStream = new MemoryStream(bytes))
            excelPackage.Load(memoryStream);

        var ws = excelPackage.Workbook.Worksheets.First();
        DataTable dataTable = new();
        var hasHeader = true;
        foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
            dataTable.Columns.Add(hasHeader ? firstRowCell.Text : $"Column {firstRowCell.Start.Column}");

        var startRow = hasHeader ? 2 : 1;
        for (var rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
        {
            var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
            var row = dataTable.NewRow();
            foreach (var cell in wsRow)
                row[cell.Start.Column - 1] = cell.Text;

            dataTable.Rows.Add(row);
        }

        excelPackage.Dispose();
        return dataTable;
    }
}