using System.Collections.Generic;

namespace Week5Project.Models
{
    public class ClassInformationTable
    {
        public IEnumerable<Class> Classes { get; set; } = new List<Class>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string FilterClassName { get; set; } = string.Empty;
        public List<string> SelectedColumns { get; set; } = new();

        public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
    }
}
