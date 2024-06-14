using System.Data;
using CleanArchitectureUtility.Core.Abstractions.Serializers;
using CleanArchitectureUtility.Core.Abstractions.Translations;

namespace CleanArchitectureUtility.Utilities.Serializers.EPPlus;

public class EpPlusExcelSerializer : IExcelSerializer
{
    private readonly ITranslator _translator;

    public EpPlusExcelSerializer(ITranslator translator)
    {
        _translator = translator;
    }

    public byte[] ListToExcelByteArray<T>(List<T> list, string sheetName = "Result") => list.ToExcelByteArray(_translator, sheetName);

    public DataTable ExcelToDataTable(byte[] bytes) => bytes.ToDataTableFromExcel();

    public List<T> ExcelToList<T>(byte[] bytes) => bytes.ToDataTableFromExcel().ToList<T>(_translator);
}
