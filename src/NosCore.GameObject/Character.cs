﻿using NosCore.Shared.Logger;
using NosCore.Data;
using NosCore.GameObject.Helper;
using System;
using NosCore.Data.AliveEntities;
using NosCore.Shared.Account;
using NosCore.GameObject.ComponentEntities.Interfaces;
using NosCore.GameObject.Networking;
using NosCore.Packets.ServerPackets;
using NosCore.Shared.Character;

namespace NosCore.GameObject
{
    public class Character : CharacterDTO, ICharacterEntity
    {
        public AccountDTO Account { get; set; }

        public bool IsChangingMapInstance { get; set; }

        public MapInstance MapInstance { get; set; }

        public ClientSession Session { get; set; }

        public byte VisualType { get; set; } = 1;

        public short VNum { get; set; }

        public long VisualId { get { return CharacterId; } }

        public byte? Direction { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public short? Amount { get; set; }
        private byte _speed;

        public byte Speed
        {
            get
            {
                //    if (HasBuff(CardType.Move, (byte)AdditionalTypes.Move.MovementImpossible))
                //    {
                //        return 0;
                //    }

                byte bonusSpeed = 0;/*(byte)GetBuff(CardType.Move, (byte)AdditionalTypes.Move.SetMovementNegated)[0];*/
                if (_speed + bonusSpeed > 59)
                {
                    return 59;
                }
                return (byte)(_speed + bonusSpeed);
            }

            set
            {
                LastSpeedChange = DateTime.Now;
                _speed = value > 59 ? (byte)59 : value;
            }
        }

        public DateTime LastSpeedChange { get; set; }

        public byte Morph { get; set; }

        public byte MorphUpgrade { get; set; }

        public byte MorphDesign { get; set; }

        public byte MorphBonus { get; set; }

        public bool NoAttack { get; set; }

        public DateTime LastMove { get; set; }

        public bool NoMove { get; set; }
        public bool IsSitting { get; set; }
        public Guid MapInstanceId { get; set; }
        public byte Authority { get; set; }

        public byte Equipment { get; set; }

        public FdPacket GenerateFd()
        {
            return new FdPacket()
            {
                Reput = Reput,
                Dignity = (int)Dignity,
                ReputIcon = GetReputIco(),
                DignityIcon = Math.Abs(GetDignityIco())
            };
        }

        public int GetDignityIco()
        {
            int icoDignity = 1;

            if (Dignity <= -100)
            {
                icoDignity = 2;
            }
            if (Dignity <= -200)
            {
                icoDignity = 3;
            }
            if (Dignity <= -400)
            {
                icoDignity = 4;
            }
            if (Dignity <= -600)
            {
                icoDignity = 5;
            }
            if (Dignity <= -800)
            {
                icoDignity = 6;
            }

            return icoDignity;
        }

        public int IsReputHero()
        {
            int i = 0;
            //foreach (CharacterDTO characterDto in ServerManager.Instance.TopReputation)
            //{
            //    Character character = (Character)characterDto;
            //    i++;
            //    if (character.CharacterId != CharacterId)
            //    {
            //        continue;
            //    }
            //    switch (i)
            //    {
            //        case 1:
            //            return 5;
            //        case 2:
            //            return 4;
            //        case 3:
            //            return 3;
            //    }
            //    if (i <= 13)
            //    {
            //        return 2;
            //    }
            //    if (i <= 43)
            //    {
            //        return 1;
            //    }
            //}
            return 0;
        }

        public int GetReputIco()
        {
            if (Reput >= 5000001)
            {
                switch (IsReputHero())
                {
                    case 1:
                        return 28;

                    case 2:
                        return 29;

                    case 3:
                        return 30;

                    case 4:
                        return 31;

                    case 5:
                        return 32;
                }
            }
            if (Reput <= 50)
            {
                return 1;
            }
            if (Reput <= 150)
            {
                return 2;
            }
            if (Reput <= 250)
            {
                return 3;
            }
            if (Reput <= 500)
            {
                return 4;
            }
            if (Reput <= 750)
            {
                return 5;
            }
            if (Reput <= 1000)
            {
                return 6;
            }
            if (Reput <= 2250)
            {
                return 7;
            }
            if (Reput <= 3500)
            {
                return 8;
            }
            if (Reput <= 5000)
            {
                return 9;
            }
            if (Reput <= 9500)
            {
                return 10;
            }
            if (Reput <= 19000)
            {
                return 11;
            }
            if (Reput <= 25000)
            {
                return 12;
            }
            if (Reput <= 40000)
            {
                return 13;
            }
            if (Reput <= 60000)
            {
                return 14;
            }
            if (Reput <= 85000)
            {
                return 15;
            }
            if (Reput <= 115000)
            {
                return 16;
            }
            if (Reput <= 150000)
            {
                return 17;
            }
            if (Reput <= 190000)
            {
                return 18;
            }
            if (Reput <= 235000)
            {
                return 19;
            }
            if (Reput <= 285000)
            {
                return 20;
            }
            if (Reput <= 350000)
            {
                return 21;
            }
            if (Reput <= 500000)
            {
                return 22;
            }
            if (Reput <= 1500000)
            {
                return 23;
            }
            if (Reput <= 2500000)
            {
                return 24;
            }
            if (Reput <= 3750000)
            {
                return 25;
            }
            return Reput <= 5000000 ? 26 : 27;
        }

        public void LoadSpeed()
        {
            Speed = CharacterHelper.Instance.SpeedData[(byte)Class];
        }

        public double MPLoad()
        {
            const int mp = 0;
            const double multiplicator = 1.0;
            return (int)((CharacterHelper.Instance.MpData[(byte)Class, Level] + mp) * multiplicator);
        }

        public double HPLoad()
        {
            const double multiplicator = 1.0;
            const int hp = 0;

            return (int)((CharacterHelper.Instance.HpData[(byte)Class, Level] + hp) * multiplicator);
        }

        //TODO move to extension
        public AtPacket GenerateAt()
        {
            return new AtPacket()
            {
                CharacterId = CharacterId,
                MapId = MapId,
                PositionX = PositionX,
                PositionY = PositionY,
                Unknown1 = 2,
                Unknown2 = 0,
                Music = MapInstance.Map.Music,
                Unknown3 = -1
            };
        }

        public TitPacket GenerateTit()
        {
            return new TitPacket()
            {
                ClassType = Language.Instance.GetMessageFromKey(Enum.Parse(typeof(CharacterClassType), Class.ToString()).ToString().ToUpper()),
                Name = Name
            };
        }

        public CInfoPacket GenerateCInfo()
        {
            return new CInfoPacket()
            {
                Name = (Account.Authority == AuthorityType.Moderator ? $"[{Language.Instance.GetMessageFromKey("SUPPORT")}]" + Name : Name),
                Unknown1 = string.Empty,
                Unknown2 = -1,
                FamilyId = -1,
                FamilyName = string.Empty,
                CharacterId = CharacterId,
                Authority = (byte)Account.Authority,
                Gender = (byte)Gender,
                HairStyle = (byte)HairStyle,
                HairColor = (byte)HairColor,
                Class = (byte)Class,
                Icon = 1,
                Compliment = (short)(Account.Authority == AuthorityType.Moderator ? 500 : Compliment),
                Invisible = false,
                FamilyLevel = 0,
                MorphUpgrade = 0,
                ArenaWinner = false
            };
        }
    }
}
