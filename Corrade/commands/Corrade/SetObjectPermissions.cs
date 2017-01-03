///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CorradeConfigurationSharp;
using OpenMetaverse;
using wasOpenMetaverse;
using wasSharp;
using Inventory = wasOpenMetaverse.Inventory;
using Reflection = wasSharp.Reflection;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class CorradeCommands
        {
            public static readonly Action<Command.CorradeCommandParameters, Dictionary<string, string>>
                setobjectpermissions =
                    (corradeCommandParameters, result) =>
                    {
                        if (
                            !HasCorradePermission(corradeCommandParameters.Group.UUID,
                                (int) Configuration.Permissions.Interact))
                        {
                            throw new Command.ScriptException(Enumerations.ScriptError.NO_CORRADE_PERMISSIONS);
                        }
                        float range;
                        if (
                            !float.TryParse(
                                wasInput(KeyValue.Get(
                                    wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.RANGE)),
                                    corradeCommandParameters.Message)), NumberStyles.Float, Utils.EnUsCulture,
                                out range))
                        {
                            range = corradeConfiguration.Range;
                        }
                        Primitive primitive = null;
                        var item = wasInput(KeyValue.Get(
                            wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.ITEM)),
                            corradeCommandParameters.Message));
                        if (string.IsNullOrEmpty(item))
                        {
                            throw new Command.ScriptException(Enumerations.ScriptError.NO_ITEM_SPECIFIED);
                        }
                        UUID itemUUID;
                        switch (UUID.TryParse(item, out itemUUID))
                        {
                            case true:
                                if (
                                    !Services.FindObject(Client,
                                        itemUUID,
                                        range,
                                        ref primitive,
                                        corradeConfiguration.DataTimeout))
                                {
                                    throw new Command.ScriptException(Enumerations.ScriptError.OBJECT_NOT_FOUND);
                                }
                                break;
                            default:
                                if (
                                    !Services.FindObject(Client,
                                        item,
                                        range,
                                        ref primitive,
                                        corradeConfiguration.DataTimeout))
                                {
                                    throw new Command.ScriptException(Enumerations.ScriptError.OBJECT_NOT_FOUND);
                                }
                                break;
                        }
                        Simulator simulator;
                        lock (Locks.ClientInstanceNetworkLock)
                        {
                            simulator = Client.Network.Simulators.AsParallel()
                                .FirstOrDefault(o => o.Handle.Equals(primitive.RegionHandle));
                        }
                        if (simulator == null)
                            throw new Command.ScriptException(Enumerations.ScriptError.REGION_NOT_FOUND);
                        var itemPermissions =
                            wasInput(
                                KeyValue.Get(
                                    wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.PERMISSIONS)),
                                    corradeCommandParameters.Message));
                        if (string.IsNullOrEmpty(itemPermissions))
                        {
                            throw new Command.ScriptException(Enumerations.ScriptError.NO_PERMISSIONS_PROVIDED);
                        }
                        var permissions = Inventory.wasStringToPermissions(itemPermissions);
                        lock (Locks.ClientInstanceObjectsLock)
                        {
                            Client.Objects.SetPermissions(simulator,
                                new List<uint> {primitive.LocalID},
                                PermissionWho.Base, permissions.BaseMask, true);
                            Client.Objects.SetPermissions(simulator,
                                new List<uint> {primitive.LocalID},
                                PermissionWho.Owner, permissions.OwnerMask, true);
                            Client.Objects.SetPermissions(simulator,
                                new List<uint> {primitive.LocalID},
                                PermissionWho.Group, permissions.GroupMask, true);
                            Client.Objects.SetPermissions(simulator,
                                new List<uint> {primitive.LocalID},
                                PermissionWho.Everyone, permissions.EveryoneMask, true);
                            Client.Objects.SetPermissions(simulator,
                                new List<uint> {primitive.LocalID},
                                PermissionWho.NextOwner, permissions.NextOwnerMask, true);
                        }
                    };
        }
    }
}