﻿///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using Corrade.Events;
using CorradeConfigurationSharp;
using OpenMetaverse;
using wasOpenMetaverse;
using Inventory = wasOpenMetaverse.Inventory;

namespace Corrade
{
    public partial class Corrade
    {
        public partial class RLVBehaviours
        {
            public static readonly Action<string, wasOpenMetaverse.RLV.RLVRule, UUID> attachall =
                (message, rule, senderUUID) =>
                {
                    if (!rule.Param.Equals(wasOpenMetaverse.RLV.RLV_CONSTANTS.FORCE))
                    {
                        return;
                    }
                    var RLVFolder = Inventory.FindInventory<InventoryFolder>(Client,
                        wasOpenMetaverse.RLV.RLV_CONSTANTS.SHARED_FOLDER_PATH,
                        wasOpenMetaverse.RLV.RLV_CONSTANTS.PATH_SEPARATOR, null, corradeConfiguration.ServicesTimeout,
                        Client.Inventory.Store.RootFolder);
                    if (RLVFolder == null)
                    {
                        return;
                    }

                    lock (RLVInventoryLock)
                    {
                        var attachments = Inventory.GetAttachments(Client, corradeConfiguration.DataTimeout);
                        var inventoryFolder = Inventory.FindInventory<InventoryFolder>(Client,
                            rule.Option,
                            wasOpenMetaverse.RLV.RLV_CONSTANTS.PATH_SEPARATOR, null,
                            corradeConfiguration.ServicesTimeout, RLVFolder,
                            StringComparison.OrdinalIgnoreCase);

                        if (inventoryFolder == null)
                            return;

                        inventoryFolder.GetInventoryRecursive(Client, corradeConfiguration.ServicesTimeout)
                            .AsParallel()
                            .Where(Inventory.CanBeWorn).ForAll(
                                o =>
                                {
                                    var inventoryItem = o as InventoryItem;
                                    if (inventoryItem is InventoryWearable)
                                    {
                                        Inventory.Wear(Client, CurrentOutfitFolder, inventoryItem, true,
                                            corradeConfiguration.ServicesTimeout);
                                        CorradeThreadPool[Threading.Enumerations.ThreadType.NOTIFICATION].Spawn(
                                            () => SendNotification(
                                                Configuration.Notifications.OutfitChanged,
                                                new OutfitEventArgs
                                                {
                                                    Action = Enumerations.Action.ATTACH,
                                                    Name = inventoryItem.Name,
                                                    Description = inventoryItem.Description,
                                                    Item = inventoryItem.UUID,
                                                    Asset = inventoryItem.AssetUUID,
                                                    Entity = inventoryItem.AssetType,
                                                    Creator = inventoryItem.CreatorID,
                                                    Permissions =
                                                        Inventory.wasPermissionsToString(
                                                            inventoryItem.Permissions),
                                                    Inventory = inventoryItem.InventoryType,
                                                    Slot =
                                                        (inventoryItem as InventoryWearable).WearableType
                                                            .ToString(),
                                                    Replace = true
                                                }),
                                            corradeConfiguration.MaximumNotificationThreads);
                                        return;
                                    }
                                    if (inventoryItem is InventoryObject || inventoryItem is InventoryAttachment)
                                    {
                                        Inventory.Attach(Client, CurrentOutfitFolder, inventoryItem,
                                            AttachmentPoint.Default, true, corradeConfiguration.ServicesTimeout);
                                        var slot = Inventory.GetAttachments(
                                            Client,
                                            corradeConfiguration.DataTimeout)
                                            .ToArray()
                                            .AsParallel()
                                            .Where(p => p.Key.Properties.ItemID.Equals(inventoryItem.UUID))
                                            .Select(p => p.Value.ToString())
                                            .FirstOrDefault() ?? AttachmentPoint.Default.ToString();
                                        CorradeThreadPool[Threading.Enumerations.ThreadType.NOTIFICATION].Spawn(
                                            () => SendNotification(
                                                Configuration.Notifications.OutfitChanged,
                                                new OutfitEventArgs
                                                {
                                                    Action = Enumerations.Action.ATTACH,
                                                    Name = inventoryItem.Name,
                                                    Description = inventoryItem.Description,
                                                    Item = inventoryItem.UUID,
                                                    Asset = inventoryItem.AssetUUID,
                                                    Entity = inventoryItem.AssetType,
                                                    Creator = inventoryItem.CreatorID,
                                                    Permissions =
                                                        Inventory.wasPermissionsToString(
                                                            inventoryItem.Permissions),
                                                    Inventory = inventoryItem.InventoryType,
                                                    Slot = slot,
                                                    Replace = true
                                                }),
                                            corradeConfiguration.MaximumNotificationThreads);
                                    }
                                });

                        RebakeTimer.Change(corradeConfiguration.RebakeDelay, 0);
                    }
                };
        }
    }
}