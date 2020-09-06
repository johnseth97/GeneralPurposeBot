using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GeneralPurposeBot.Models
{
    public class AssignableRole
    {
        [Key]
        public ulong RoleId { get; set; }
        public ulong ServerId { get; set; }
        public string RoleName { get; set; }

    }
}
