///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CorradeConfigurationSharp;
using OpenMetaverse;
using wasOpenMetaverse;
using wasSharp;
using wasSharp.Timers;
using Reflection = wasSharp.Reflection;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class CorradeCommands
        {
            public static readonly Action<Command.CorradeCommandParameters, Dictionary<string, string>> getrolesmembers
                =
                (corradeCommandParameters, result) =>
                {
                    if (
                        !HasCorradePermission(corradeCommandParameters.Group.UUID, (int) Configuration.Permissions.Group))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.NO_CORRADE_PERMISSIONS);
                    }
                    UUID groupUUID;
                    var target = wasInput(
                        KeyValue.Get(
                            wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.TARGET)),
                            corradeCommandParameters.Message));
                    switch (string.IsNullOrEmpty(target))
                    {
                        case false:
                            if (!UUID.TryParse(target, out groupUUID) &&
                                !Resolvers.GroupNameToUUID(Client, target, corradeConfiguration.ServicesTimeout,
                                    corradeConfiguration.DataTimeout,
                                    new DecayingAlarm(corradeConfiguration.DataDecayType), ref groupUUID))
                                throw new Command.ScriptException(Enumerations.ScriptError.GROUP_NOT_FOUND);
                            break;
                        default:
                            groupUUID = corradeCommandParameters.Group.UUID;
                            break;
                    }
                    var currentGroups = Enumerable.Empty<UUID>();
                    if (
                        !Services.GetCurrentGroups(Client, corradeConfiguration.ServicesTimeout,
                            ref currentGroups))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.COULD_NOT_GET_CURRENT_GROUPS);
                    }
                    if (!new HashSet<UUID>(currentGroups).Contains(groupUUID))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.NOT_IN_GROUP);
                    }
                    var groupRolesMembers = new HashSet<KeyValuePair<UUID, UUID>>();
                    var GroupRoleMembersReplyEvent = new ManualResetEvent(false);
                    var groupRolesMembersRequestUUID = UUID.Zero;
                    EventHandler<GroupRolesMembersReplyEventArgs> GroupRolesMembersEventHandler =
                        (sender, args) =>
                        {
                            if (!groupRolesMembersRequestUUID.Equals(args.RequestID)) return;
                            groupRolesMembers.UnionWith(args.RolesMembers);
                            GroupRoleMembersReplyEvent.Set();
                        };
                    Client.Groups.GroupRoleMembersReply += GroupRolesMembersEventHandler;
                    groupRolesMembersRequestUUID = Client.Groups.RequestGroupRolesMembers(groupUUID);
                    if (!GroupRoleMembersReplyEvent.WaitOne((int) corradeConfiguration.ServicesTimeout, false))
                    {
                        Client.Groups.GroupRoleMembersReply -= GroupRolesMembersEventHandler;
                        throw new Command.ScriptException(Enumerations.ScriptError.TIMEOUT_GETING_GROUP_ROLES_MEMBERS);
                    }
                    Client.Groups.GroupRoleMembersReply -= GroupRolesMembersEventHandler;
                    // First resolve the all the role names to role UUIDs
                    var roleUUIDNames = new Hashtable(groupRolesMembers.Count);
                    var LockObject = new object();
                    groupRolesMembers.AsParallel().GroupBy(o => o.Key).Select(o => o.First().Key).ForAll(
                        o =>
                        {
                            var roleName = string.Empty;
                            switch (Resolvers.RoleUUIDToName(Client, o, groupUUID,
                                corradeConfiguration.ServicesTimeout,
                                ref roleName))
                            {
                                case true:
                                    lock (LockObject)
                                    {
                                        roleUUIDNames.Add(o, roleName);
                                    }
                                    break;
                            }
                        });
                    // Next, associate role names with agent names and UUIDs.
                    var csv = new List<string>();
                    groupRolesMembers.AsParallel().Where(o => roleUUIDNames.ContainsKey(o.Key)).ForAll(o =>
                    {
                        var agentName = string.Empty;
                        if (Resolvers.AgentUUIDToName(Client, o.Value, corradeConfiguration.ServicesTimeout,
                            ref agentName) && roleUUIDNames.ContainsKey(o.Key))
                        {
                            lock (LockObject)
                            {
                                csv.Add(roleUUIDNames[o.Key] as string);
                                csv.Add(agentName);
                                csv.Add(o.Value.ToString());
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