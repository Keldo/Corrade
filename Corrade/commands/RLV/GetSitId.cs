///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using OpenMetaverse;
using wasOpenMetaverse;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class RLVBehaviours
        {
            public static readonly Action<string, wasOpenMetaverse.RLV.RLVRule, UUID> getsitid =
                (message, rule, senderUUID) =>
                {
                    int channel;
                    if (!int.TryParse(rule.Param, NumberStyles.Integer, Utils.EnUsCulture, out channel) || channel < 1)
                    {
                        return;
                    }
                    Avatar me;
                    bool isSitting;
                    lock (Locks.ClientInstanceNetworkLock)
                    {
                        isSitting = Client.Network.CurrentSim.ObjectsAvatars.TryGetValue(Client.Self.LocalID, out me);
                    }
                    if (isSitting)
                    {
                        if (me.ParentID != 0)
                        {
                            Primitive sit;
                            lock (Locks.ClientInstanceNetworkLock)
                            {
                                isSitting = Client.Network.CurrentSim.ObjectsPrimitives.TryGetValue(me.ParentID, out sit);
                            }
                            if (isSitting)
                            {
                                lock (Locks.ClientInstanceSelfLock)
                                {
                                    Client.Self.Chat(sit.ID.ToString(), channel, ChatType.Normal);
                                }
                                return;
                            }
                        }
                    }
                    var zero = UUID.Zero;
                    lock (Locks.ClientInstanceSelfLock)
                    {
                        Client.Self.Chat(zero.ToString(), channel, ChatType.Normal);
                    }
                };
        }
    }
}