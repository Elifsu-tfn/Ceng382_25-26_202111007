//Data structure model that holds and controls the basic information of the class

using System.ComponentModel.DataAnnotations;

namespace Week5Project.Models
{
    public class ClassInformationModel
    {
        private static int _idCounter = 1;

        public ClassInformationModel()
        {
            Id = _idCounter++;
            ClassName = string.Empty;
            Description = string.Empty;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required")]
        public string? ClassName { get; set; }

        [Required(ErrorMessage = "Student Count is required")]
        [Range(1, 100, ErrorMessage = "Student Count must be between 1 and 100")]
        public int StudentCount { get; set; }

        public string? Description { get; set; }
    }
}