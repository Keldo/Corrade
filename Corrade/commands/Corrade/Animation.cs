///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Corrade.Constants;
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
            public static readonly Action<Command.CorradeCommandParameters, Dictionary<string, string>> animation =
                (corradeCommandParameters, result) =>
                {
                    if (
                        !HasCorradePermission(corradeCommandParameters.Group.UUID,
                            (int) Configuration.Permissions.Grooming))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.NO_CORRADE_PERMISSIONS);
                    }
                    var item = wasInput(
                        KeyValue.Get(wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.ITEM)),
                            corradeCommandParameters.Message));
                    if (string.IsNullOrEmpty(item))
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.NO_ITEM_SPECIFIED);
                    }
                    InventoryItem inventoryItem = null;
                    UUID itemUUID;
                    switch (UUID.TryParse(item, out itemUUID))
                    {
                        case true:
                            lock (Locks.ClientInstanceInventoryLock)
                            {
                                if (Client.Inventory.Store.Contains(itemUUID))
                                {
                                    inventoryItem = Client.Inventory.Store[itemUUID] as InventoryItem;
                                }
                            }
                            break;
                        default:
                            inventoryItem = Inventory.FindInventory<InventoryItem>(Client, item,
                                CORRADE_CONSTANTS.PATH_SEPARATOR, CORRADE_CONSTANTS.PATH_SEPARATOR_ESCAPE,
                                corradeConfiguration.ServicesTimeout);
                            break;
                    }
                    if (inventoryItem == null)
                    {
                        throw new Command.ScriptException(Enumerations.ScriptError.INVENTORY_ITEM_NOT_FOUND);
                    }
                    switch (
                        Reflection.GetEnumValueFromName<Enumerations.Action>(
                            wasInput(
                                KeyValue.Get(
                                    wasOutput(Reflection.GetNameFromEnumValue(Command.ScriptKeys.ACTION)),
                                    corradeCommandParameters.Message))))
                    {
                        case Enumerations.Action.START:
                            lock (Locks.ClientInstanceSelfLock)
                            {
                                Client.Self.AnimationStart(inventoryItem.AssetUUID, true);
                            }
                            break;
                        case Enumerations.Action.STOP:
                            lock (Locks.ClientInstanceSelfLock)
                            {
                                Client.Self.AnimationStop(inventoryItem.AssetUUID, true);
                            }
                            break;
                        default:
                            throw new Command.ScriptException(Enumerations.ScriptError.UNKNOWN_ANIMATION_ACTION);
                    }
                };
        }
    }
}