namespace Week5Project.Models
{
    public class ClassInformationTable
    {
        public IEnumerable<ClassInformationModel> Classes { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string FilterClassName { get; set; }

        public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
    }
}
