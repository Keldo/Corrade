///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using Corrade.Constants;
using OpenMetaverse;
using wasOpenMetaverse;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class RLVBehaviours
        {
            public static readonly Action<string, wasOpenMetaverse.RLV.RLVRule, UUID> version =
                (message, rule, senderUUID) =>
                {
                    int channel;
                    if (!int.TryParse(rule.Param, NumberStyles.Integer, Utils.EnUsCulture, out channel) || channel < 1)
                    {
                        return;
                    }
                    lock (Locks.ClientInstanceSelfLock)
                    {
                        Client.Self.Chat(
                            $"{wasOpenMetaverse.RLV.RLV_CONSTANTS.VIEWER} v{wasOpenMetaverse.RLV.RLV_CONSTANTS.SHORT_VERSION} (Corrade Version: {CORRADE_CONSTANTS.CORRADE_VERSION} Compiled: {CORRADE_CONSTANTS.CORRADE_COMPILE_DATE})",
                            channel,
                            ChatType.Normal);
                    }
                };
        }
    }
}