using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspireDemo.Data.Models
{
    [Table("Weathers")]
    public class Weather
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public int TemperatureC { get; set; }

        [Required]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTimeOffset.Now.DateTime);

        public string? Summary { get; set; }
    }
}
