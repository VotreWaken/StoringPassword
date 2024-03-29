﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using StoringPassword.Models;

namespace StoringPassword.Models
{
    [Table("Users")]
    public class User
    {
        public User() => Messages = new List<Message>();

        [Key]
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [NotNull]
        public string? Login { get; set; }

        [NotNull]
        public string? Password { get; set; }

        [NotNull]
        public string? Salt { get; set; }
        [JsonIgnore]
        public virtual ICollection<Message>? Messages { get; set; }
    }
}