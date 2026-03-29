using System.ComponentModel.DataAnnotations;

namespace Lab_03.Models
{
    // Destination: điểm đến/nhóm tour
    public class Destination
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;
        public List<Tour>? Tours { get; set; }
    }
}
