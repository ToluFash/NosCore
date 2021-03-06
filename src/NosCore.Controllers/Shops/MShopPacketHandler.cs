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

using System;
using ChickenAPI.Packets.ClientPackets.Shops;
using ChickenAPI.Packets.Enumerations;
using ChickenAPI.Packets.ServerPackets.Shop;
using ChickenAPI.Packets.ServerPackets.UI;
using NosCore.Core.I18N;
using NosCore.Data.Enumerations.Group;
using NosCore.Data.Enumerations.I18N;
using NosCore.GameObject;
using NosCore.GameObject.ComponentEntities.Extensions;
using NosCore.GameObject.Networking.ChannelMatcher;
using NosCore.GameObject.Networking.ClientSession;
using NosCore.GameObject.Networking.Group;
using NosCore.GameObject.Providers.ItemProvider.Item;
using NosCore.PathFinder;

namespace NosCore.PacketHandlers.Shops
{
    public class MShopPacketHandler : PacketHandler<MShopPacket>, IWorldPacketHandler
    {
        public override void Execute(MShopPacket mShopPacket, ClientSession clientSession)
        {
            if (clientSession.Character.InExchangeOrTrade)
            {
                //todo log
                return;
            }

            var portal = clientSession.Character.MapInstance.Portals.Find(port =>
                Heuristic.Octile(Math.Abs(clientSession.Character.PositionX - port.SourceX),
                    Math.Abs(clientSession.Character.PositionY - port.SourceY)) <= 6);
            if (portal != null)
            {
                clientSession.SendPacket(new MsgPacket
                {
                    Message = Language.Instance.GetMessageFromKey(LanguageKey.SHOP_NEAR_PORTAL,
                        clientSession.Account.Language),
                    Type = 0
                });
                return;
            }

            if (clientSession.Character.Group != null && clientSession.Character.Group?.Type != GroupType.Group)
            {
                clientSession.SendPacket(new MsgPacket
                {
                    Message = Language.Instance.GetMessageFromKey(LanguageKey.SHOP_NOT_ALLOWED_IN_RAID,
                        clientSession.Account.Language),
                    Type = MessageType.White
                });
                return;
            }

            if (!clientSession.Character.MapInstance.ShopAllowed)
            {
                clientSession.SendPacket(new MsgPacket
                {
                    Message = Language.Instance.GetMessageFromKey(LanguageKey.SHOP_NOT_ALLOWED,
                        clientSession.Account.Language),
                    Type = MessageType.White
                });
                return;
            }

            switch (mShopPacket.Type)
            {
                case CreateShopPacketType.Open:
                    clientSession.Character.Shop = new Shop();
                    sbyte shopSlot = -1;
                    foreach (var item in mShopPacket.ItemList)
                    {
                        shopSlot++;
                        if (item.Amount == 0)
                        {
                            continue;
                        }

                        var inv = clientSession.Character.Inventory.LoadBySlotAndType<IItemInstance>(item.Slot, item.Type);
                        if (inv == null)
                        {
                            //log
                            continue;
                        }

                        if (inv.Amount < item.Amount)
                        {
                            //todo log
                            return;
                        }

                        if (!inv.Item.IsTradable || inv.BoundCharacterId != null)
                        {
                            clientSession.SendPacket(new ShopEndPacket { Type = VisualType.Map });
                            clientSession.SendPacket(clientSession.Character.GenerateSay(
                                Language.Instance.GetMessageFromKey(LanguageKey.SHOP_ONLY_TRADABLE_ITEMS,
                                    clientSession.Account.Language),
                                SayColorType.Yellow));
                            clientSession.Character.Shop = null;
                            return;
                        }

                        clientSession.Character.Shop.ShopItems.TryAdd(shopSlot,
                            new ShopItem
                            {
                                Amount = item.Amount,
                                Price = item.Price,
                                Slot = (byte)shopSlot,
                                Type = 0,
                                ItemInstance = inv
                            });
                    }

                    if (clientSession.Character.Shop.ShopItems.Count == 0)
                    {
                        clientSession.SendPacket(new ShopEndPacket { Type = VisualType.Map });
                        clientSession.SendPacket(clientSession.Character.GenerateSay(
                            Language.Instance.GetMessageFromKey(LanguageKey.SHOP_EMPTY, clientSession.Account.Language),
                            SayColorType.Yellow));
                        clientSession.Character.Shop = null;
                        return;
                    }

                    clientSession.Character.Shop.Session = clientSession;
                    clientSession.Character.Shop.MenuType = 3;
                    clientSession.Character.Shop.ShopId = 501;
                    clientSession.Character.Shop.Size = 60;
                    clientSession.Character.Shop.Name = string.IsNullOrWhiteSpace(mShopPacket.Name) ?
                        Language.Instance.GetMessageFromKey(LanguageKey.SHOP_PRIVATE_SHOP, clientSession.Account.Language) :
                        mShopPacket.Name.Substring(0, Math.Min(mShopPacket.Name.Length, 20));

                    clientSession.Character.MapInstance.Sessions.SendPacket(clientSession.Character.GenerateShop());
                    clientSession.SendPacket(new InfoPacket
                    {
                        Message = Language.Instance.GetMessageFromKey(LanguageKey.SHOP_OPEN,
                            clientSession.Account.Language)
                    });

                    clientSession.Character.Requests.Subscribe(data =>
                        data.ClientSession.SendPacket(clientSession.Character.GenerateNpcReq(clientSession.Character.Shop.ShopId)));
                    clientSession.Character.MapInstance.Sessions.SendPacket(clientSession.Character.GeneratePFlag(),
                        new EveryoneBut(clientSession.Channel.Id));
                    clientSession.Character.IsSitting = true;
                    clientSession.Character.LoadSpeed();
                    clientSession.SendPacket(clientSession.Character.GenerateCond());
                    clientSession.Character.MapInstance.Sessions.SendPacket(clientSession.Character.GenerateRest());
                    break;
                case CreateShopPacketType.Close:
                    clientSession.Character.CloseShop();
                    break;
                case CreateShopPacketType.Create:
                    clientSession.SendPacket(new IshopPacket());
                    break;
                default:
                    //todo log
                    return;
            }
        }
    }

}