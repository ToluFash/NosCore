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

using ChickenAPI.Packets.ClientPackets.Inventory;
using ChickenAPI.Packets.Enumerations;
using ChickenAPI.Packets.ServerPackets.UI;
using NosCore.Core.I18N;
using NosCore.Data.Enumerations.I18N;
using NosCore.GameObject;
using NosCore.GameObject.ComponentEntities.Extensions;
using NosCore.GameObject.Networking.ClientSession;
using NosCore.GameObject.Networking.Group;
using NosCore.GameObject.Providers.ItemProvider.Item;
using Serilog;

namespace NosCore.PacketHandlers.Inventory
{
    public class RemovePacketHandler : PacketHandler<RemovePacket>, IWorldPacketHandler
    {
        private readonly ILogger _logger;

        public RemovePacketHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override void Execute(RemovePacket removePacket, ClientSession clientSession)
        {
            if (clientSession.Character.InExchangeOrShop)
            {
                _logger.Error(LogLanguage.Instance.GetMessageFromKey(LogLanguageKey.CANT_MOVE_ITEM_IN_SHOP));
                return;
            }

            IItemInstance inventory =
                clientSession.Character.Inventory.LoadBySlotAndType<IItemInstance>((short)removePacket.InventorySlot,
                    PocketType.Wear);
            if (inventory == null)
            {
                return;
            }

            IItemInstance inv = clientSession.Character.Inventory.MoveInPocket((short)removePacket.InventorySlot,
                PocketType.Wear, PocketType.Equipment);

            if (inv == null)
            {
                clientSession.SendPacket(new MsgPacket
                {
                    Message = Language.Instance.GetMessageFromKey(LanguageKey.NOT_ENOUGH_PLACE,
                        clientSession.Account.Language),
                    Type = 0
                });
                return;
            }

            clientSession.SendPacket(inv.GeneratePocketChange(inv.Type, inv.Slot));

            clientSession.Character.MapInstance.Sessions.SendPacket(clientSession.Character.GenerateEq());
            clientSession.SendPacket(clientSession.Character.GenerateEquipment());

            if (inv.Item.EquipmentSlot == EquipmentType.Fairy)
            {
                clientSession.Character.MapInstance.Sessions.SendPacket(
                    clientSession.Character.GeneratePairy((WearableInstance)null));
            }
        }
    }
}