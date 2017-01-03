///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Corrade.Constants;
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
            public static readonly Action<Command.CorradeCommandParameters, Dictionary<string, string>> createprimitive
                =
                (corradeCommandParameters, result) =>
                {
                    if (
                        !HasCorradePermission(corradeCommandParameters.Group.UUID,
                            (int) Configuration.Permissions.Interact))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.NO_CORRADE_PERMISSIONS);
                    }
                    Vector3 position;
                    if (
                        !Vector3.TryParse(
                            wasInput(
                                KeyValue.Get(
                                    wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.POSITION)),
                                    corradeCommandParameters.Message)),
                            out position))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.INVALID_POSITION);
                    }
                    if (wasOpenMetaverse.Helpers.IsSecondLife(Client) &&
                        position.Z > wasOpenMetaverse.Constants.PRIMITIVES.MAXIMUM_REZ_HEIGHT)
                    {
                        throw new Command.ScriptException(
                            Enumerations.ScriptError.POSITION_WOULD_EXCEED_MAXIMUM_REZ_ALTITUDE);
                    }
                    Quaternion rotation;
                    if (
                        !Quaternion.TryParse(
                            wasInput(
                                KeyValue.Get(
                                    wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.ROTATION)),
                                    corradeCommandParameters.Message)),
                            out rotation))
                    {
                        rotation = Quaternion.Identity;
                    }
                    var region =
                        wasInput(
                            KeyValue.Get(wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.REGION)),
                                corradeCommandParameters.Message));
                    Simulator simulator;
                    lock (Locks.ClientInstanceNetworkLock)
                    {
                        simulator =
                            Client.Network.Simulators.AsParallel().FirstOrDefault(
                                o =>
                                    o.Name.Equals(
                                        string.IsNullOrEmpty(region) ? Client.Network.CurrentSim.Name : region,
                                        StringComparison.OrdinalIgnoreCase));
                    }
                    if (simulator == null)
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.REGION_NOT_FOUND);
                    }
                    Parcel parcel = null;
                    if (
                        !Services.GetParcelAtPosition(Client, simulator, position, corradeConfiguration.ServicesTimeout,
                            ref parcel))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.COULD_NOT_FIND_PARCEL);
                    }
                    if (!parcel.Flags.IsMaskFlagSet(ParcelFlags.CreateObjects))
                    {
                        if (!simulator.IsEstateManager)
                        {
                            if (!parcel.OwnerID.Equals(Client.Self.AgentID))
                            {
                                if (!parcel.IsGroupOwned && !parcel.GroupID.Equals(corradeCommandParameters.Group.UUID))
                                {
                                    throw new Command.ScriptException(
                                        Enumerations.ScriptError.NO_GROUP_POWER_FOR_COMMAND);
                                }
                                if (
                                    !Services.HasGroupPowers(Client, Client.Self.AgentID,
                                        corradeCommandParameters.Group.UUID,
                                        GroupPowers.AllowRez,
                                        corradeConfiguration.ServicesTimeout, corradeConfiguration.DataTimeout,
                                        new DecayingAlarm(corradeConfiguration.DataDecayType)))
                                {
                                    throw new Command.ScriptException(
                                        Enumerations.ScriptError.NO_GROUP_POWER_FOR_COMMAND);
                                }
                            }
                        }
                    }
                    Vector3 scale;
                    if (
                        !Vector3.TryParse(
                            wasInput(
                                KeyValue.Get(wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.SCALE)),
                                    corradeCommandParameters.Message)),
                            out scale))
                    {
                        scale = wasOpenMetaverse.Constants.PRIMITIVES.DEFAULT_NEW_PRIMITIVE_SCALE;
                    }
                    if (wasOpenMetaverse.Helpers.IsSecondLife(Client) &&
                        (scale.X < wasOpenMetaverse.Constants.PRIMITIVES.MINIMUM_SIZE_X ||
                         scale.Y < wasOpenMetaverse.Constants.PRIMITIVES.MINIMUM_SIZE_Y ||
                         scale.Z < wasOpenMetaverse.Constants.PRIMITIVES.MINIMUM_SIZE_Z ||
                         scale.X > wasOpenMetaverse.Constants.PRIMITIVES.MAXIMUM_SIZE_X ||
                         scale.Y > wasOpenMetaverse.Constants.PRIMITIVES.MAXIMUM_SIZE_Y ||
                         scale.Z > wasOpenMetaverse.Constants.PRIMITIVES.MAXIMUM_SIZE_Z))
                    {
                        throw new Command.ScriptException(
                            Enumerations.ScriptError.SCALE_WOULD_EXCEED_BUILDING_CONSTRAINTS);
                    }
                    // build the primitive shape from presets by supplying "type" (or not)...
                    var primitiveShapesFieldInfo = typeof(CORRADE_CONSTANTS.PRIMTIVE_BODIES).GetFields(
                        BindingFlags.Public |
                        BindingFlags.Static)
                        .AsParallel().FirstOrDefault(
                            o =>
                                o.Name.Equals(
                                    wasInput(
                                        KeyValue.Get(
                                            wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.TYPE)),
                                            corradeCommandParameters.Message)),
                                    StringComparison.OrdinalIgnoreCase));
                    Primitive.ConstructionData constructionData;
                    switch (primitiveShapesFieldInfo != null)
                    {
                        case true:
                            constructionData = (Primitive.ConstructionData) primitiveShapesFieldInfo.GetValue(null);
                            break;
                        default:
                            // Build the construction data as a default primitive box.
                            constructionData = CORRADE_CONSTANTS.PRIMTIVE_BODIES.CUBE;
                            break;
                    }
                    // ... and overwrite with manual data settings.
                    constructionData =
                        constructionData.wasCSVToStructure(
                            wasInput(KeyValue.Get(wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.DATA)),
                                corradeCommandParameters.Message)));
                    // Get any primitive flags.
                    PrimFlags primFlags = 0;
                    CSV.ToEnumerable(
                        wasInput(
                            KeyValue.Get(wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.FLAGS)),
                                corradeCommandParameters.Message)))
                        .ToArray()
                        .AsParallel()
                        .Where(o => !string.IsNullOrEmpty(o))
                        .ForAll(
                            o =>
                                typeof(PrimFlags).GetFields(BindingFlags.Public | BindingFlags.Static)
                                    .AsParallel()
                                    .Where(p => Strings.StringEquals(o, p.Name, StringComparison.Ordinal))
                                    .ForAll(
                                        q => { BitTwiddling.SetMaskFlag(ref primFlags, (PrimFlags) q.GetValue(null)); }));

                    // Finally, add the primitive to the simulator.
                    lock (Locks.ClientInstanceObjectsLock)
                    {
                        Client.Objects.AddPrim(simulator, constructionData, corradeCommandParameters.Group.UUID,
                            position,
                            scale, rotation,
                            primFlags);
                    }
                };
        }
    }
}