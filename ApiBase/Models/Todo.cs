using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBase.Models
{
    public class Todo
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public int Hours { get; set; }

        [ForeignKey(nameof(IdentityUser))]
        public string OwnerId { get; set; }

        [NotMapped]
        public virtual IdentityUser Owner { get; set; }

        public Todo()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
