///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
                setprimitivematerial =
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
                                    !Services.FindPrimitive(Client,
                                        itemUUID,
                                        range,
                                        ref primitive,
                                        corradeConfiguration.DataTimeout))
                                {
                                    throw new Command.ScriptException(Enumerations.ScriptError.PRIMITIVE_NOT_FOUND);
                                }
                                break;
                            default:
                                if (
                                    !Services.FindPrimitive(Client,
                                        item,
                                        range,
                                        ref primitive,
                                        corradeConfiguration.DataTimeout))
                                {
                                    throw new Command.ScriptException(Enumerations.ScriptError.PRIMITIVE_NOT_FOUND);
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
                        var materialFieldInfo = typeof(Material).GetFields(BindingFlags.Public |
                                                                           BindingFlags.Static)
                            .AsParallel().FirstOrDefault(
                                o =>
                                    Strings.StringEquals(o.Name, wasInput(KeyValue.Get(
                                        wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.MATERIAL)),
                                        corradeCommandParameters.Message)),
                                        StringComparison.OrdinalIgnoreCase));
                        if (materialFieldInfo == null)
                            throw new Command.ScriptException(Enumerations.ScriptError.UNKNOWN_MATERIAL_TYPE);
                        lock (Locks.ClientInstanceObjectsLock)
                        {
                            Client.Objects.SetMaterial(simulator,
                                primitive.LocalID, (Material) materialFieldInfo.GetValue(null));
                        }
                    };
        }
    }
}