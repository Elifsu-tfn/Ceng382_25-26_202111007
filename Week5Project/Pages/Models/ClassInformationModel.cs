using System.ComponentModel.DataAnnotations;

namespace Week5Project.Models
{
    public class ClassInformationModel
    {
        // Id'nin otomatik olarak arttırılması için statik sayaç
        private static int _idCounter = 1;

        // Constructor, Id'yi otomatik arttırarak atar
        public ClassInformationModel()
        {
            Id = _idCounter++;  // Her yeni sınıf için Id'yi arttır
            ClassName = string.Empty;  // Varsayılan olarak boş string
            Description = string.Empty;  // Varsayılan olarak boş string
        }

        // Id, her yeni sınıf için otomatik olarak atanacak
        public int Id { get; set; }

        // ClassName için zorunluluk ve hata mesajı
        [Required(ErrorMessage = "Class Name is required")]
        public string? ClassName { get; set; }  // Null olabilen özellik

        // StudentCount için zorunluluk ve belirli bir aralık doğrulaması
        [Required(ErrorMessage = "Student Count is required")]
        [Range(1, 100, ErrorMessage = "Student Count must be between 1 and 100")]
        public int StudentCount { get; set; }

        // Description için null olabilen özellik
        public string? Description { get; set; }  // Null olabilen özellik
    }
}
