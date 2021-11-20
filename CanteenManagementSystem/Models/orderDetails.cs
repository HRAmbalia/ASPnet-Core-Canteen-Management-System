using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenManagementSystem.Models
{
    public class orderDetails
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        [Display(Name = "Customer Id")]
        public string customerId { get; set; }

        [Display(Name = "Menu Id")]
        public int menuId { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        [Display(Name = "Menu Name")]
        public string menuName { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        [Display(Name = "Menu Price")]
        public string menuPrice { get; set; }

        [Display(Name = "Order Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Column(TypeName = "date")]
        public DateTime orderDate { get; set; }

        [Column(TypeName = "nvarchar(60)")]
        [Display(Name = "Payment Status")]
        public string paymentStatus { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        [Display(Name = "Customer Name")]
        public string customerName { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        [Display(Name = "Customer EmailId")]
        public string customerEmailId { get; set; }

        public static implicit operator orderDetails(List<orderDetails> v)
        {
            throw new NotImplementedException();
        }
    }
}
