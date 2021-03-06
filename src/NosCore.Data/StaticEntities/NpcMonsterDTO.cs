﻿//  __  _  __    __   ___ __  ___ ___  
// |  \| |/__\ /' _/ / _//__\| _ \ __| 
// | | ' | \/ |`._`.| \_| \/ | v / _|  
// |_|\__|\__/ |___/ \__/\__/|_|_\___| 
// 
// Copyright (C) 2019 - NosCore
// 
// NosCore is a free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NosCore.Data.AliveEntities;
using NosCore.Data.DataAttributes;
using LogLanguageKey = NosCore.Data.Enumerations.I18N.LogLanguageKey;
using MonsterType = NosCore.Data.Enumerations.Map.MonsterType;

namespace NosCore.Data.StaticEntities
{
    [StaticDto(LoadedMessage = LogLanguageKey.NPCMONSTERS_LOADED)]
    public class NpcMonsterDto : IStaticDto
    {
        public byte AmountRequired { get; set; }

        public byte AttackClass { get; set; }

        public byte AttackUpgrade { get; set; }

        public byte BasicArea { get; set; }

        public short BasicCooldown { get; set; }

        public byte BasicRange { get; set; }

        public short BasicSkill { get; set; }

        public short CloseDefence { get; set; }

        public short Concentrate { get; set; }

        public byte CriticalChance { get; set; }

        public short CriticalRate { get; set; }

        public short DamageMaximum { get; set; }

        public short DamageMinimum { get; set; }

        public short DarkResistance { get; set; }

        public short DefenceDodge { get; set; }

        public byte DefenceUpgrade { get; set; }

        public short DistanceDefence { get; set; }

        public short DistanceDefenceDodge { get; set; }

        public virtual ICollection<DropDto> Drop { get; set; }

        public byte Element { get; set; }

        public short ElementRate { get; set; }

        public short FireResistance { get; set; }

        public byte HeroLevel { get; set; }

        public int HeroXp { get; set; }

        public bool IsHostile { get; set; }

        public int JobXp { get; set; }

        public byte Level { get; set; }

        public short LightResistance { get; set; }

        public short MagicDefence { get; set; }

        public virtual ICollection<MapMonsterDto> MapMonster { get; set; }

        public virtual ICollection<MapNpcDto> MapNpc { get; set; }

        public virtual ICollection<MateDto> Mate { get; set; }

        public int MaxHp { get; set; }

        public int MaxMp { get; set; }

        public MonsterType MonsterType { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public bool NoAggresiveIcon { get; set; }

        public byte NoticeRange { get; set; }

        public virtual ICollection<NpcMonsterSkillDto> NpcMonsterSkill { get; set; }

        [Key]
        public short NpcMonsterVNum { get; set; }

        public byte Race { get; set; }

        public byte RaceType { get; set; }

        public int RespawnTime { get; set; }

        public byte Speed { get; set; }

        public short VNumRequired { get; set; }

        public short WaterResistance { get; set; }

        public int Xp { get; set; }

        public bool IsPercent { get; set; }

        public int TakeDamages { get; set; }

        public int GiveDamagePercentage { get; set; }

        public virtual ICollection<BCardDto> BCards { get; set; }
    }
}