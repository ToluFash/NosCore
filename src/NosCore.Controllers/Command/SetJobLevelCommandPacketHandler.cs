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
using System.Linq;
using ChickenAPI.Packets.ServerPackets.UI;
using NosCore.Configuration;
using NosCore.Core;
using NosCore.Core.I18N;
using NosCore.Core.Networking;
using NosCore.Data.CommandPackets;
using NosCore.Data.Enumerations;
using NosCore.Data.Enumerations.I18N;
using NosCore.Data.WebApi;
using NosCore.GameObject;
using NosCore.GameObject.Networking.ClientSession;
using Character = NosCore.Data.WebApi.Character;

namespace NosCore.PacketHandlers.Command
{
    public class SetJobLevelCommandPacketHandler : PacketHandler<SetJobLevelCommandPacket>, IWorldPacketHandler
    {
        public override void Execute(SetJobLevelCommandPacket levelPacket, ClientSession session)
        {
            if (string.IsNullOrEmpty(levelPacket.Name) || levelPacket.Name == session.Character.Name)
            {
                session.Character.SetJobLevel(levelPacket.Level);
                return;
            }

            var data = new StatData
            {
                ActionType = UpdateStatActionType.UpdateJobLevel,
                Character = new Character { Name = levelPacket.Name },
                Data = levelPacket.Level
            };

            var channels = WebApiAccess.Instance.Get<List<ChannelInfo>>(WebApiRoute.Channel)
                ?.Where(c => c.Type == ServerType.WorldServer);

            ConnectedAccount receiver = null;
            ServerConfiguration config = null;

            foreach (var channel in channels ?? new List<ChannelInfo>())
            {
                var accounts =
                    WebApiAccess.Instance.Get<List<ConnectedAccount>>(WebApiRoute.ConnectedAccount, channel.WebApi);

                var target = accounts.FirstOrDefault(s => s.ConnectedCharacter.Name == levelPacket.Name);

                if (target != null)
                {
                    receiver = target;
                    config = channel.WebApi;
                }
            }

            if (receiver == null) //TODO: Handle 404 in WebApi
            {
                session.SendPacket(new InfoPacket
                {
                    Message = Language.Instance.GetMessageFromKey(LanguageKey.CANT_FIND_CHARACTER,
                        session.Account.Language)
                });
                return;
            }

            WebApiAccess.Instance.Post<StatData>(WebApiRoute.Stat, data, config);
        }
    }
}