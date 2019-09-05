using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VzlomKazino.Models
{
    public class Player
    {
        public static readonly double NeedsXpPerLevel = 1.1d;
        public static readonly uint NeedsXpForLevel = 100;

        private List<decimal> _endedsMoney = new List<decimal>();

        private uint _xp = 0;
        private decimal _money = 0;

        public decimal Money
        {
            get => _money;
            set
            {
                _money = Math.Round(value, 2);
            }
        }

        #region Stats
        public int HackCount { get; set; } = 0;
        public int FullHackCount { get; set; } = 0;
        public double Average { get => _endedsMoney.Count > 0 ? (double)_endedsMoney.Average() : 0.0; }
        #endregion

        [JsonIgnore]
        public ushort Level { get; private set; } = 1;
        [JsonIgnore]
        public uint XP
        {
            get => _xp;
            set
            {
                TotalXP += value;

                _xp += value;

                if (_xp >= NeedsXpForLevel * (NeedsXpPerLevel * Level))
                {
                    for (; _xp > NeedsXpForLevel * (NeedsXpPerLevel * Level); Level++)
                    {
                        _xp -= (uint)(NeedsXpForLevel * (NeedsXpPerLevel * Level));
                    }
                }
            }
        }

        [JsonProperty("XP")]
        public uint TotalXP { get; private set; } = 0;

        public void AddStat(decimal Moneys)
        {
            _endedsMoney.Add(Moneys);
            Money += Moneys;
        }
    }
}
