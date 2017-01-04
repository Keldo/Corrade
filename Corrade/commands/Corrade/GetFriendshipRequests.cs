///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
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
            public static readonly Action<Command.CorradeCommandParameters, Dictionary<string, string>>
                getfriendshiprequests =
                    (corradeCommandParameters, result) =>
                    {
                        if (
                            !HasCorradePermission(corradeCommandParameters.Group.UUID,
                                (int) Configuration.Permissions.Friendship))
                        {
                            throw new Command.ScriptException(Enumerations.ScriptError.NO_CORRADE_PERMISSIONS);
                        }
                        Dictionary<UUID, UUID> friendshipRequests;
                        lock (Locks.ClientInstanceFriendsLock)
                        {
                            friendshipRequests = Client.Friends.FriendRequests.Copy();
                        }
                        var csv = new List<string>();
                        var LockObject = new object();
                        friendshipRequests.AsParallel().ForAll(o =>
                        {
                            var agentName = string.Empty;
                            if (Resolvers.AgentUUIDToName(Client, o.Key, corradeConfiguration.ServicesTimeout,
                                ref agentName))
                            {
                                lock (LockObject)
                                {
                                    csv.AddRange(new[] {agentName, o.Key.ToString(), o.Value.ToString()});
                                }
                            }
                        });
                        if (csv.Any())
                        {
                            result.Add(Reflection.GetNameFromEnumValue(Command.ResultKeys.DATA),
                                CSV.FromEnumerable(csv));
                        }
                    };
        }
    }
}