using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CanteenManagementSystem.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the CmsAppUser class
    public class CmsAppUser : IdentityUser
    {
        [Required(ErrorMessage = "Please enter First Name")]
        [Column(TypeName = "nvarchar(60)")]
        [PersonalData]
        [Display(Name = "First Name")]
        public string userFirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last Name")]
        [Column(TypeName = "nvarchar(60)")]
        [PersonalData]
        [Display(Name = "Last Name")]
        public string userLastSurname { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        [PersonalData]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please select Gender")]
        [Column(TypeName = "nvarchar(20)")]
        [PersonalData]
        [Display(Name = "Gender")]
        public string userGender { get; set; }

        [Required(ErrorMessage = "Please select UserStream")]
        [Column(TypeName = "nvarchar(20)")]
        [PersonalData]
        [Display(Name = "User Stream")]
        public string userStream { get; set; }

        [Required(ErrorMessage = "Please select Role for user")]
        [Column(TypeName = "nvarchar(20)")]
        [PersonalData]
        [Display(Name = "User Role")]
        public string Role { get; set; }

        [DataType(DataType.Date)]
        [PersonalData]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Staff JoiningDate")]
        public string staffJoinDate { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        [PersonalData]
        [Display(Name = "Staff Image")]
        public string staffImage { get; set; }
    }
}
