///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace wasSharp
{
    public struct NetHash
    {
        private readonly int hashCode;

        public NetHash(int hashCode = 17)
        {
            this.hashCode = hashCode;
        }

        public static NetHash Init => new NetHash();

        public static implicit operator int(NetHash hashCode)
        {
            return hashCode.GetHashCode();
        }

        public NetHash Hash<T>(T obj)
        {
            var c = EqualityComparer<T>.Default;
            var h = c.Equals(obj, default(T)) ? 0 : obj.GetHashCode();
            unchecked
            {
                h += hashCode*31;
            }
            return new NetHash(h);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}