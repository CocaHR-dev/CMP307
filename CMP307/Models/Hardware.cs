using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMP307.Models
{
    public partial class Hardware
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HardwareId { get; set; }

        [Required(ErrorMessage = "System Name is required")]
        [MaxLength(100)]
        public string SystemName { get; set; } = null!;

        [Required(ErrorMessage = "Model is required")]
        [MaxLength(50)]
        public string Model { get; set; } = null!;

        [Required(ErrorMessage = "Manufacturer is required")]
        public string Manufacturer { get; set; } = null!;

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = null!;

        [Required(ErrorMessage = "IP Address is required")]
        public string IpAddress { get; set; } = null!;

        public DateTime? PurchaseDate { get; set; }

        public string? Note { get; set; } = null!;

        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;
    }
}
