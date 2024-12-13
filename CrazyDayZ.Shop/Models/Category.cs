using System.ComponentModel.DataAnnotations;

namespace CrazyDayZ.Shop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
