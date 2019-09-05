using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VzlomKazino.Models
{
    public class PlayerContext
    {
        public int HackCount { get; set; } = 0;
        public int FullHackCount { get; set; } = 0;

        [JsonProperty("XP")]
        public uint TotalXP { get; set; } = 0;

        public decimal Money { get; set; }

        public static implicit operator Player(PlayerContext e)
        {
            Player x = new Player();
            x.XP = e.TotalXP;
            x.HackCount = e.HackCount;
            x.FullHackCount = e.FullHackCount;
            x.Money = e.Money;
            return x;
        }
    }
}
