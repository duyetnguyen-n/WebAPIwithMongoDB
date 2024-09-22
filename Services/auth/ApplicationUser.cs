using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebAPIwithMongoDB.Services.auth
{
    public class ApplicationUser : IdentityUser
    {
        public string NumberPhone { get; set; }
        public string PositionId { get; set; }
        public string Avatar { get; set; }
    }
}