///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CorradeConfigurationSharp;
using OpenMetaverse;
using wasOpenMetaverse;
using wasSharp;
using Reflection = wasSharp.Reflection;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class CorradeCommands
        {
            public static readonly Action<Command.CorradeCommandParameters, Dictionary<string, string>> deletepick =
                (corradeCommandParameters, result) =>
                {
                    if (
                        !HasCorradePermission(corradeCommandParameters.Group.UUID,
                            (int) Configuration.Permissions.Grooming))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.NO_CORRADE_PERMISSIONS);
                    }
                    var AvatarPicksReplyEvent = new ManualResetEvent(false);
                    var input =
                        wasInput(KeyValue.Get(wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.NAME)),
                            corradeCommandParameters.Message));
                    if (string.IsNullOrEmpty(input))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.EMPTY_PICK_NAME);
                    }
                    var pickUUID = UUID.Zero;
                    EventHandler<AvatarPicksReplyEventArgs> AvatarPicksEventHandler = (sender, args) =>
                    {
                        var pick = args.Picks.AsParallel().FirstOrDefault(
                            o => Strings.StringEquals(input, o.Value, StringComparison.Ordinal));
                        if (!pick.Equals(default(KeyValuePair<UUID, string>)))
                            pickUUID = pick.Key;
                        AvatarPicksReplyEvent.Set();
                    };
                    lock (Locks.ClientInstanceAvatarsLock)
                    {
                        Client.Avatars.AvatarPicksReply += AvatarPicksEventHandler;
                        Client.Avatars.RequestAvatarPicks(Client.Self.AgentID);
                        if (!AvatarPicksReplyEvent.WaitOne((int) corradeConfiguration.ServicesTimeout, false))
                        {
                            Client.Avatars.AvatarPicksReply -= AvatarPicksEventHandler;
                            throw new Command.ScriptException(Enumerations.ScriptError.TIMEOUT_GETTING_PICKS);
                        }
                        Client.Avatars.AvatarPicksReply -= AvatarPicksEventHandler;
                    }
                    if (pickUUID.Equals(UUID.Zero))
                    {
                        pickUUID = UUID.Random();
                    }
                    lock (Locks.ClientInstanceSelfLock)
                    {
                        Client.Self.PickDelete(pickUUID);
                    }
                };
        }
    }
}