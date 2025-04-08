using ChoiceofaNation_WebAPI.Logic.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Entity
{
    public class User : IdentityUser
    {
        [Required, MinLength(3)]
        public string FirstName { get; set; }
        [Required, MinLength(3)]
        public string LastName { get; set; }
        public string RoleId { get; set; }
        public Roles? Role { get; set; }
        public string Url { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public int PlayedHours { get; set; }
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public ICollection<Topic> Topics { get; set; } = new HashSet<Topic>();
    }
}
