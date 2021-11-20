using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenManagementSystem.Models
{
    public class menuDetails
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter Menu Name")]
        [Column(TypeName = "nvarchar(60)")]
        [Display(Name = "Menu Name")]
        public string manuName { get; set; }

        [Required(ErrorMessage = "Please enter Menu Price")]
        [Column(TypeName = "nvarchar(60)")]
        [Display(Name = "Menu Price")]
        public string menuPrice { get; set; }

        [Required(ErrorMessage = "is menu Available?")]
        [Column(TypeName = "nvarchar(60)")]
        [Display(Name = "Menu Availbility")]
        public string manuAvailbility { get; set; }

        [Required(ErrorMessage = "Please enter Menu Type")]
        [Column(TypeName = "nvarchar(60)")]
        [Display(Name = "Menu Type")]
        public string manuType { get; set; }

        [Required(ErrorMessage = "Please choose Veg or NonVeg")]
        [Column(TypeName = "nvarchar(60)")]
        [Display(Name = "Veg or NonVeg")]
        public string vegOrNonVeg { get; set; }

        [Column(TypeName = "nvarchar(2000)")]
        [Display(Name = "Menu Image")]
        public string manuImage { get; set; }

        [Column(TypeName = "nvarchar(2000)")]
        [Display(Name = "Menu Description")]
        public string manuDescription { get; set; }

        public menuDetails()
        {
        }
    }
}
