using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models.Entities
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [Required(ErrorMessage = "Status Name is required")]
        [StringLength(50, ErrorMessage = "Status Name must be less than 50 characters")]
        public string StatusName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
