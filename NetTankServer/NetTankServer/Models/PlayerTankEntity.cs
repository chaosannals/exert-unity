using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTankServer.Models;

[Table("nt_player_tank")]
public class PlayerTankEntity
{

    public long PlayerId { get; set; }

    public long TankId { get; set; }
}
