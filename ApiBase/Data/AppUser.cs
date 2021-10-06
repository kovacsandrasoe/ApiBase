using ApiBase.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiBase.Data
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string PhotoContentType { get; set; }

        public byte[] PhotoData { get; set; }

        [JsonIgnore]
        public override string PasswordHash { get; set; }

        [JsonIgnore]
        public override string SecurityStamp { get; set; }

        [JsonIgnore]
        public override string ConcurrencyStamp { get; set; }
    }
}
