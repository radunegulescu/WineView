using System.ComponentModel.DataAnnotations;

namespace WineView.Models
{
    public class Winery
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Region { get; set; }
    }
}
