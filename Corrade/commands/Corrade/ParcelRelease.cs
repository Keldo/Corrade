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
using wasSharp.Timers;
using Reflection = wasSharp.Reflection;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class CorradeCommands
        {
            public static readonly Action<Command.CorradeCommandParameters, Dictionary<string, string>> parcelrelease =
                (corradeCommandParameters, result) =>
                {
                    if (!HasCorradePermission(corradeCommandParameters.Group.UUID, (int) Configuration.Permissions.Land))
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
                        position = Client.Self.SimPosition;
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
                    if (!simulator.IsEstateManager)
                    {
                        if (!parcel.OwnerID.Equals(Client.Self.AgentID))
                        {
                            if (!parcel.IsGroupOwned && !parcel.GroupID.Equals(corradeCommandParameters.Group.UUID))
                            {
                                throw new Command.ScriptException(Enumerations.ScriptError.NO_GROUP_POWER_FOR_COMMAND);
                            }
                            if (
                                !Services.HasGroupPowers(Client, Client.Self.AgentID,
                                    corradeCommandParameters.Group.UUID,
                                    GroupPowers.LandRelease,
                                    corradeConfiguration.ServicesTimeout, corradeConfiguration.DataTimeout,
                                    new DecayingAlarm(corradeConfiguration.DataDecayType)))
                            {
                                throw new Command.ScriptException(Enumerations.ScriptError.NO_GROUP_POWER_FOR_COMMAND);
                            }
                        }
                    }
                    lock (Locks.ClientInstanceParcelsLock)
                    {
                        Client.Parcels.ReleaseParcel(simulator, parcel.LocalID);
                    }
                };
        }
    }
}