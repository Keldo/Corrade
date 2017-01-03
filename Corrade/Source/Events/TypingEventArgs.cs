﻿///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using OpenMetaverse;

namespace Corrade.Events
{
    /// <summary>
    ///     An event for typing on local or instant message.
    /// </summary>
    public class TypingEventArgs : EventArgs
    {
        public Enumerations.Action Action;
        public UUID AgentUUID;
        public Enumerations.Entity Entity;
        public string FirstName;
        public string LastName;
    }
}