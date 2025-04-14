//  Provides standardized filename generation and JSON formatting for exports
using Newtonsoft.Json;
using System;

namespace Week5Project.Helpers
{
    public static class ExportHelper
    {
        public static string GenerateExportFilename(string filter, int page)
        {
            var cleanFilter = string.IsNullOrEmpty(filter) ? "all" : filter.ToLower().Replace(" ", "-");
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            return $"classes-{cleanFilter}-page{page}-{timestamp}.json";
        }

        public static string FormatExportData(object data)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };

            try
            {
                return JsonConvert.SerializeObject(data, settings);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { 
                    Error = "Export failed",
                    Message = ex.Message 
                }, settings);
            }
        }
    }
}