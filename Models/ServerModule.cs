using Discord.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GeneralPurposeBot.Models
{
    public class ServerModule
    {
        [Key]
        public Guid Id { get; set; }
        public ulong ServerId { get; set; }
        public string Name { get; set; }
        public bool Disabled { get; set; }
    }
}
