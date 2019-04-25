﻿using System;
using System.Collections.Generic;
using System.Linq;
using ChickenAPI.Packets.ClientPackets.Drops;
using ChickenAPI.Packets.ClientPackets.Inventory;
using ChickenAPI.Packets.ClientPackets.Specialists;
using ChickenAPI.Packets.Enumerations;
using ChickenAPI.Packets.ServerPackets.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NosCore.Configuration;
using NosCore.Core;
using NosCore.Core.Encryption;
using NosCore.Core.I18N;
using NosCore.Data;
using NosCore.Data.Enumerations.Character;
using NosCore.Data.Enumerations.I18N;
using NosCore.Data.Enumerations.Items;
using NosCore.Data.Enumerations.Map;
using NosCore.Database;
using NosCore.Database.DAL;
using NosCore.GameObject;
using NosCore.GameObject.Map;
using NosCore.GameObject.Networking.ClientSession;
using NosCore.GameObject.Providers.ExchangeProvider;
using NosCore.GameObject.Providers.InventoryService;
using NosCore.GameObject.Providers.ItemProvider;
using NosCore.GameObject.Providers.ItemProvider.Handlers;
using NosCore.GameObject.Providers.ItemProvider.Item;
using NosCore.GameObject.Providers.MapInstanceProvider;
using NosCore.GameObject.Providers.MapItemProvider;
using NosCore.GameObject.Providers.MapItemProvider.Handlers;
using NosCore.PacketHandlers.Inventory;
using Serilog;
using Character = NosCore.GameObject.Character;

namespace NosCore.Tests.PacketHandlerTests
{
    [TestClass]
    public class SpTransformPacketHandlerTests
    {
        private SpTransformPacketHandler _spTransformPacketHandler;
        private static readonly ILogger _logger = Logger.GetLoggerConfiguration().CreateLogger();

        private readonly ClientSession _session = new ClientSession(
            new WorldConfiguration
            { BackpackSize = 2, MaxItemAmount = 999, MaxSpPoints = 10_000, MaxAdditionalSpPoints = 1_000_000 }, _logger, new List<IPacketHandler>());

        private Character _chara;
        private IItemProvider _item;
        private MapInstance _map;
        private MapItemProvider _mapItemProvider;

        [TestCleanup]
        public void Cleanup()
        {
            SystemTime.Freeze(DateTime.Now);
        }

        [TestInitialize]
        public void Setup()
        {
            SystemTime.Freeze();
            var contextBuilder =
                new DbContextOptionsBuilder<NosCoreContext>().UseInMemoryDatabase(
                    databaseName: Guid.NewGuid().ToString());
            DataAccessHelper.Instance.InitializeForTest(contextBuilder.Options);
            var _acc = new AccountDto { Name = "AccountTest", Password = "test".ToSha512() };

            var items = new List<ItemDto>
            {
                new Item {Type = PocketType.Main, VNum = 1012, IsDroppable = true},
                new Item {Type = PocketType.Main, VNum = 1013},
                new Item {Type = PocketType.Equipment, VNum = 1, ItemType = ItemType.Weapon},
                new Item {Type = PocketType.Equipment, VNum = 2, EquipmentSlot = EquipmentType.Fairy, Element = 2},
                new Item
                {
                    Type = PocketType.Equipment, VNum = 912, ItemType = ItemType.Specialist, ReputationMinimum = 2,
                    Element = 1
                },
                new Item {Type = PocketType.Equipment, VNum = 924, ItemType = ItemType.Fashion},
                new Item
                {
                    Type = PocketType.Main, VNum = 1078, ItemType = ItemType.Special,
                    Effect = ItemEffectType.DroppedSpRecharger, EffectValue = 10_000, WaitDelay = 5_000
                }
            };

            _chara = new Character(new InventoryService(items, _session.WorldConfiguration, _logger),
                new ExchangeProvider(null, null, _logger), null, null, null, null, null, _logger, null)
            {
                CharacterId = 1,
                Name = "TestExistingCharacter",
                Slot = 1,
                AccountId = _acc.AccountId,
                MapId = 1,
                State = CharacterState.Active
            };
            _session.InitializeAccount(_acc);

            _item = new ItemProvider(items, new List<IEventHandler<Item, Tuple<IItemInstance, UseItemPacket>>>
            {
                new SpRechargerEventHandler(_session.WorldConfiguration),
                new VehicleEventHandler(_logger),
                new WearEventHandler(_logger)
            });

            _mapItemProvider = new MapItemProvider(new List<IEventHandler<MapItem, Tuple<MapItem, GetPacket>>>
                {new DropEventHandler(), new SpChargerEventHandler(), new GoldDropEventHandler()});
            _map = new MapInstance(new Map
            {
                Name = "testMap",
                Data = new byte[]
                    {
                        8, 0, 8, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 1, 1, 1, 0, 0, 0, 0,
                        0, 1, 1, 1, 0, 0, 0, 0,
                        0, 1, 1, 1, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0
                    }
            }
                , Guid.NewGuid(), false, MapInstanceType.BaseMapInstance,
                _mapItemProvider,
                null, _logger);
            _session.SetCharacter(_chara);
            _session.Character.MapInstance = _map;
            _session.Character.Account = _acc;
            _spTransformPacketHandler = new SpTransformPacketHandler();
        }


        [TestMethod]
        public void Test_Transform_NoSp()
        {
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 0 }, _session);
            var packet = (MsgPacket)_session.LastPacket;
            Assert.IsTrue(packet.Message ==
                Language.Instance.GetMessageFromKey(LanguageKey.NO_SP, _session.Account.Language));
        }

        [TestMethod]
        public void Test_Transform_Vehicle()
        {
            _session.Character.IsVehicled = true;
            _session.Character.Inventory.AddItemToPocket(_item.Create(912, 1));
            var item = _session.Character.Inventory.First();
            item.Value.Type = PocketType.Wear;
            item.Value.Slot = (byte)EquipmentType.Sp;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 0 }, _session);
            var packet = (MsgPacket)_session.LastPacket;
            Assert.IsTrue(packet.Message ==
                Language.Instance.GetMessageFromKey(LanguageKey.REMOVE_VEHICLE, _session.Account.Language));
        }


        [TestMethod]
        public void Test_Transform_Sitted()
        {
            _session.Character.IsSitting = true;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 0 }, _session);
            Assert.IsNull(_session.LastPacket);
        }

        [TestMethod]
        public void Test_RemoveSp()
        {
            _session.Character.Inventory.AddItemToPocket(_item.Create(912, 1));
            var item = _session.Character.Inventory.First();
            _session.Character.UseSp = true;
            item.Value.Type = PocketType.Wear;
            item.Value.Slot = (byte)EquipmentType.Sp;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 1 }, _session);
            Assert.IsFalse(_session.Character.UseSp);
        }

        [TestMethod]
        public void Test_Transform()
        {
            _session.Character.SpPoint = 1;
            _session.Character.Reput = 5000000;
            _session.Character.Inventory.AddItemToPocket(_item.Create(912, 1));
            var item = _session.Character.Inventory.First();
            item.Value.Type = PocketType.Wear;
            item.Value.Slot = (byte)EquipmentType.Sp;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 1 }, _session);
            Assert.IsTrue(_session.Character.UseSp);
        }

        [TestMethod]
        public void Test_Transform_BadFairy()
        {
            _session.Character.SpPoint = 1;
            _session.Character.Reput = 5000000;
            var item = _session.Character.Inventory.AddItemToPocket(_item.Create(912, 1)).First();
            var fairy = _session.Character.Inventory.AddItemToPocket(_item.Create(2, 1)).First();

            item.Type = PocketType.Wear;
            item.Slot = (byte)EquipmentType.Sp;
            fairy.Type = PocketType.Wear;
            fairy.Slot = (byte)EquipmentType.Fairy;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 1 }, _session);
            var packet = (MsgPacket)_session.LastPacket;
            Assert.IsTrue(packet.Message ==
                Language.Instance.GetMessageFromKey(LanguageKey.BAD_FAIRY, _session.Account.Language));
        }

        [TestMethod]
        public void Test_Transform_BadReput()
        {
            _session.Character.SpPoint = 1;
            _session.Character.Inventory.AddItemToPocket(_item.Create(912, 1));
            var item = _session.Character.Inventory.First();
            item.Value.Type = PocketType.Wear;
            item.Value.Slot = (byte)EquipmentType.Sp;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 1 }, _session);
            var packet = (MsgPacket)_session.LastPacket;
            Assert.IsTrue(packet.Message ==
                Language.Instance.GetMessageFromKey(LanguageKey.LOW_REP, _session.Account.Language));
        }


        [TestMethod]
        public void Test_TransformBefore_Cooldown()
        {
            _session.Character.SpPoint = 1;
            _session.Character.LastSp = SystemTime.Now();
            _session.Character.SpCooldown = 30;
            _session.Character.Inventory.AddItemToPocket(_item.Create(912, 1));
            var item = _session.Character.Inventory.First();
            item.Value.Type = PocketType.Wear;
            item.Value.Slot = (byte)EquipmentType.Sp;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 1 }, _session);
            var packet = (MsgPacket)_session.LastPacket;
            Assert.IsTrue(packet.Message ==
                string.Format(Language.Instance.GetMessageFromKey(LanguageKey.SP_INLOADING, _session.Account.Language),
                    30));
        }

        [TestMethod]
        public void Test_Transform_OutOfSpPoint()
        {
            _session.Character.LastSp = SystemTime.Now();
            _session.Character.Inventory.AddItemToPocket(_item.Create(912, 1));
            var item = _session.Character.Inventory.First();
            item.Value.Type = PocketType.Wear;
            item.Value.Slot = (byte)EquipmentType.Sp;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 1 }, _session);
            var packet = (MsgPacket)_session.LastPacket;
            Assert.IsTrue(packet.Message ==
                Language.Instance.GetMessageFromKey(LanguageKey.SP_NOPOINTS, _session.Account.Language));
        }

        [TestMethod]
        public void Test_Transform_Delay()
        {
            _session.Character.SpPoint = 1;
            _session.Character.LastSp = SystemTime.Now();
            _session.Character.Inventory.AddItemToPocket(_item.Create(912, 1));
            var item = _session.Character.Inventory.First();
            item.Value.Type = PocketType.Wear;
            item.Value.Slot = (byte)EquipmentType.Sp;
            _spTransformPacketHandler.Execute(new SpTransformPacket { Type = 0 }, _session);
            var packet = (DelayPacket)_session.LastPacket;
            Assert.IsTrue(packet.Delay == 5000);
        }
    }
}
