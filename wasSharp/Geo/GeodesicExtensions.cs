///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;

namespace wasSharp.Geo
{
    public static class GeodesicExtensions
    {
        public static Distance HaversineDistanceTo(this GeographicCoordinate sourceGeographicCoordinate,
            GeographicCoordinate targetGeographicCoordinate)
        {
            return HaversineDistanceTo(sourceGeographicCoordinate, targetGeographicCoordinate, DistanceUnits.KILOMETERS);
        }

        public static Distance HaversineDistanceTo(this GeographicCoordinate sourceGeographicCoordinate,
            GeographicCoordinate targetGeographicCoordinate,
            DistanceUnits distanceUnits)
        {
            var sourcePhi = Math.PI*sourceGeographicCoordinate.Latitude/180;
            var targetPhi = Math.PI*targetGeographicCoordinate.Latitude/180;
            var deltaPhi = Math.PI*(targetPhi - sourcePhi)/180;
            var deltaLam = Math.PI*(targetGeographicCoordinate.Longitude - sourceGeographicCoordinate.Longitude)/180;


            var a = Math.Sin(deltaPhi/2)*Math.Sin(deltaPhi/2) +
                    Math.Cos(sourcePhi)*Math.Cos(targetPhi)*
                    Math.Sin(deltaLam/2)*Math.Sin(deltaLam/2);

            var c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return new Distance(Constants.EARTH_MEAN_RADIUS.Meters*c);
        }
    }
}