///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;

namespace wasSharp.Geo
{
    public enum DistanceUnits
    {
        METERS = 1,
        KILOMETERS = 2
    }

    public class Distance : IComparable<Distance>, IEquatable<Distance>
    {
        public Distance(double meters)
        {
            Meters = meters;
        }

        public double Kilometers => Meters/1000d;

        public double Meters { get; }

        public int CompareTo(Distance other)
        {
            return Meters.CompareTo(other.Meters);
        }

        public bool Equals(Distance other)
        {
            return this == other;
        }

        public static bool operator >(Distance a, Distance b)
        {
            return a != null && b != null && a.Meters > b.Meters;
        }

        public static bool operator >=(Distance a, Distance b)
        {
            return (a == null && b == null) || (a != null && b != null && a.Meters >= b.Meters);
        }

        public static bool operator <(Distance a, Distance b)
        {
            return a != null && b != null && a.Meters < b.Meters;
        }

        public static bool operator <=(Distance a, Distance b)
        {
            return (a == null && b == null) || (a != null && b != null && a.Meters <= b.Meters);
        }

        public static bool operator ==(Distance a, Distance b)
        {
            return (ReferenceEquals(a, null) && ReferenceEquals(b, null)) ||
                   (!ReferenceEquals(a, null) && !ReferenceEquals(b, null) && a.Meters == b.Meters);
        }

        public static bool operator !=(Distance a, Distance b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Distance)
                return Equals(obj as Distance);

            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}