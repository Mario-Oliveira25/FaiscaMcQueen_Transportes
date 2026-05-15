using Microsoft.AspNetCore.Identity;
using System;

namespace FaiscaMcQueen_Transportes.Models
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? TecnicoId { get; set; }
    }
}