using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerDataApp.Models
{
    public partial class CustomerData
    {
        [Key]
        [Column("serno")]
        public int Serno { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Business")]
        public string Bname { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Customer")]
        public string Cname { get; set; }
        [Required]
        [StringLength(50)]
        public string City { get; set; }
        [Required]
        [StringLength(50)]
        public string State { get; set; }
        [Required]
        [StringLength(200)]
        public string Address { get; set; }
        [Required]
        [Column("cell1")]
        [StringLength(50)]
        public string Cell1 { get; set; }
        [Required]
        [Column("cell2")]
        [StringLength(50)]
        public string Cell2 { get; set; }
        [Required]
        [Column("cell3")]
        [StringLength(50)]
        public string Cell3 { get; set; }
        [Required]
        [StringLength(50)]
        public string Mail { get; set; }
        [Required]
        [Column("GSTIN")]
        [StringLength(50)]
        public string Gstin { get; set; }
        [Required]
        [Column("CIN")]
        [StringLength(50)]
        public string Cin { get; set; }
        [Required]
        [Column("DIN")]
        [StringLength(50)]
        public string Din { get; set; }
        [Required]
        [StringLength(50)]
        public string Turnover { get; set; }
        [Required]
        [StringLength(50)]
        public string Position { get; set; }
        [Required]
        [StringLength(50)]
        public string Category { get; set; }
    }
}
