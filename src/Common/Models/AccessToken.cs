﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BioEngine.Common.Base;

namespace BioEngine.Common.Models
{
    [Table("be_oauth2server_access_tokens")]
    public class AccessToken: BaseModel<string>
    {
        [Key]
        [Column("access_token")]
        public override string Id { get; set; }

        public string ClientId { get; set; }

        public int MemberId { get; set; }

        public DateTime Expires { get; set; }

        public string Scope { get; set; }

        [ForeignKey(nameof(MemberId))]
        public User User { get; set; }
    }
}