using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;

namespace Practice.Maui.Application.Services;

public interface ISheetServiceWrapper
{
    Task Initialization { get; }
    Task<Sheet?> GetMainSheet();
    Task UpdateRow(RowData rowData);
}