using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetTankServer.Models;

[Table("nt_player")]
[Index(nameof(Account), Name = "ACCOUNT_UNIQUE", IsUnique = true)]
public class PlayerEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("account", TypeName = "VARCHAR(100)")]

    public string Account { get; set; } = null!;

    [Column("nickname", TypeName = "VARCHAR(100)")]
    public string? Nickname { get; set; }


    [Column("password", TypeName = "CHAR(64)")]
    public string? Password { get; set; }

    [Column("create_at")]
    public DateTime CreateAt { get; set; }

    [Column("create_by")]
    public long? CreateBy { get; set; }

    [Column("update_at")]
    public DateTime? UpdateAt { get; set; }

    [Column("update_by")]
    public long? UpdateBy { get; set; }

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }
}
