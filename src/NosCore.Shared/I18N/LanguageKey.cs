﻿//  __  _  __    __   ___ __  ___ ___  
// |  \| |/__\ /' _/ / _//__\| _ \ __| 
// | | ' | \/ |`._`.| \_| \/ | v / _|  
// |_|\__|\__/ |___/ \__/\__/|_|_\___| 
// 
// Copyright (C) 2018 - NosCore
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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NosCore.Shared.I18N
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum LanguageKey
    {
        REGISTRED_ON_MASTER,
        AUTHENTICATED_SUCCESS,
        AUTHENTICATED_ERROR,
        DATABASE_INITIALIZED,
        DATABASE_NOT_UPTODATE,
        CLIENT_DISCONNECTED,
        CHARACTER_NOT_INIT,
        ERROR_CHANGE_MAP,
        AUTH_INCORRECT,
        AUTH_ERROR,
        FORCED_DISCONNECTION,
        CLIENT_CONNECTED,
        CLIENT_ARRIVED,
        CORRUPTED_KEEPALIVE,
        INVALID_PASSWORD,
        INVALID_ACCOUNT,
        ACCOUNT_ARRIVED,
        SUCCESSFULLY_LOADED,
        MASTER_SERVER_RETRY,
        LISTENING_PORT,
        ENTER_PATH,
        PARSE_ALL,
        PARSE_MAPS,
        PARSE_MAPTYPES,
        PARSE_ACCOUNTS,
        PARSE_PORTALS,
        PARSE_TIMESPACES,
        PARSE_ITEMS,
        PARSE_NPCMONSTERS,
        PARSE_NPCMONSTERDATA,
        PARSE_CARDS,
        PARSE_SKILLS,
        PARSE_MAPNPCS,
        PARSE_MONSTERS,
        PARSE_SHOPS,
        PARSE_TELEPORTERS,
        PARSE_SHOPITEMS,
        PARSE_SHOPSKILLS,
        PARSE_RECIPES,
        PARSE_QUESTS,
        DONE,
        AT_LEAST_ONE_FILE_MISSING,
        CARDS_PARSED,
        ITEMS_PARSED,
        MAPS_PARSED,
        PORTALS_PARSED,
        MAPS_LOADED,
        NO_MAP,
        MAPMONSTERS_LOADED,
        CORRUPT_PACKET,
        HANDLER_ERROR,
        HANDLER_NOT_FOUND,
        SELECT_MAPID,
        WRONG_SELECTED_MAPID,
        I18N_ACTDESC_PARSED,
        I18N_CARD_PARSED,
        I18N_BCARD_PARSED,
        I18N_ITEM_PARSED,
        I18N_MAPIDDATA_PARSED,
        I18N_MAPPOINTDATA_PARSED,
        I18N_MPCMONSTER_PARSED,
        I18N_NPCMONSTERTALK_PARSED,
        I18N_QUEST_PARSED,
        I18N_SKILL_PARSED,
        PARSE_I18N,
        ALREADY_TAKEN,
        INVALID_CHARNAME,
        BAD_PASSWORD,
        SUPPORT,
        [UsedImplicitly] ADVENTURER,
        [UsedImplicitly] SWORDMAN,
        [UsedImplicitly] ARCHER,
        [UsedImplicitly] MAGICIAN,
        [UsedImplicitly] MARTIALARTIST,
        NPCMONSTERS_PARSED,
        PARSE_DROPS,
        MAPTYPES_PARSED,
        RESPAWNTYPE_PARSED,
        SKILLS_PARSED,
        NPCS_PARSED,
        MONSTERS_PARSED,
        CHANNEL,
        ADMINISTRATOR,
        CHARACTER_OFFLINE,
        SEND_MESSAGE_TO_CHARACTER,
        MAPNPCS_LOADED,
        NO_ITEM,
        NOT_ENOUGH_PLACE,
        ITEM_ACQUIRED,
        ITEMS_LOADED,
        ASK_TO_DELETE,
        SURE_TO_DELETE,
        ITEM_NOT_DROPPABLE_HERE,
        DROP_MAP_FULL,
        BAD_DROP_AMOUNT,
        ITEM_NOT_DROPPABLE,
        FRIENDLIST_FULL,
        ALREADY_FRIEND,
        FRIEND_REQUEST_BLOCKED,
        MAX_GOLD,
        ITEM_ACQUIRED_LOD,
        SP_POINTSADDED,
        FRIEND_ADD,
        FRIEND_ADDED,
        FRIEND_REJECTED,
        CANT_BLOCK_FRIEND,
        NOT_IN_BLACKLIST,
        BLACKLIST_ADDED,
        SAVING_ALL,
        BLACKLIST_BLOCKED,
        FRIEND_DELETED,
        FRIEND_OFFLINE,
        CANT_FIND_CHARACTER,
        GROUP_FULL,
        ALREADY_IN_GROUP,
        GROUP_BLOCKED,
        INVITED_YOU_GROUP,
        INVITED_GROUP_SHARE,
        GROUP_SHARE_INFO,
        JOINED_GROUP,
        GROUP_ADMIN,
        GROUP_REFUSED,
        SHARED_REFUSED,
        ACCEPTED_SHARE,
        NEW_LEADER,
        LEAVE_GROUP,
        GROUP_LEFT,
        GROUP_CLOSED,
        GROUP_INVITE,
        NPCMONSTERS_LOADED,
        UNABLE_TO_REQUEST_GROUP,
        ENCODE_ERROR,
        MAP_DONT_EXIST,
        UNKNOWN_PICKERTYPE,
        POCKETTYPE_UNKNOWN,
        USER_NOT_CONNECTED,
        FRIEND_REQUEST_SENT,
        ALREADY_BLACKLISTED,
        USER_IS_NOT_A_FRIEND,
        VISUALTYPE_UNKNOWN,
        UNKNOWN_EQUIPMENTTYPE,
        ERROR_DECODING,
        LANGUAGE_MISSING,
        INVITETYPE_UNKNOWN,
        GROUPREQUESTTYPE_UNKNOWN,
        ITEMTYPE_UNKNOWN,
        UNKWNOWN_RECEIVERTYPE,
        NO_SPECIAL_PROPERTIES_WEARABLE,
        NOT_YOUR_ITEM,
        LEVEL_CHANGED,
        JOB_LEVEL_CHANGED,
        HERO_LEVEL_CHANGED,
        MASTER_SERVER_PING,
        MASTER_SERVER_PING_FAILED,
        CONNECTION_LOST,
        CHANNEL_WILL_EXIT,
        EXCEPTION,
        VISUALENTITY_DOES_NOT_EXIST
    }
}