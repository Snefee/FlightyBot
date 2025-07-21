using FlightyBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace FlightyBot.Services
{
    public static class MapPathGenerator
    {
        private record MapView(LocationInfo Center, double Zoom);
        private record BoundingBox(double MinLon, double MinLat, double MaxLon, double MaxLat);

        public static string GenerateMapUrl(LocationInfo startPoint, LocationInfo endPoint, string mapboxApiKey)
        {
            const int MAP_WIDTH = 600;
            const int MAP_HEIGHT = 400;
            const double MIN_ZOOM = 0.5;


            var pathPoints = GetPathPoints(startPoint, endPoint);
            var boundingBox = GetBoundingBox(pathPoints);
            var calculatedView = GetCalculatedMapView(boundingBox, MAP_WIDTH, MAP_HEIGHT);
            double finalZoom = Math.Max(calculatedView.Zoom, MIN_ZOOM);

            string planeMarker = $"pin-s-airport({startPoint.Longitude.ToString(CultureInfo.InvariantCulture)},{startPoint.Latitude.ToString(CultureInfo.InvariantCulture)})";
            string airportMarker = $"pin-s-observation-tower({endPoint.Longitude.ToString(CultureInfo.InvariantCulture)},{endPoint.Latitude.ToString(CultureInfo.InvariantCulture)})";
            string pathOverlay = GeneratePathGeoJson(pathPoints);

            string lon = calculatedView.Center.Longitude.ToString(CultureInfo.InvariantCulture);
            string lat = calculatedView.Center.Latitude.ToString(CultureInfo.InvariantCulture);
            string zoom = finalZoom.ToString(CultureInfo.InvariantCulture);

            string mapImageUrl = $"https://api.mapbox.com/styles/v1/snefee/cmc74i0mb014q01sd88iig5ol/static/{planeMarker},{airportMarker},geojson({pathOverlay})/{lon},{lat},{zoom},0/{MAP_WIDTH}x{MAP_HEIGHT}?access_token={mapboxApiKey}";
            return mapImageUrl;
        }

        private static string GeneratePathGeoJson(List<LocationInfo> pathPoints)
        {
            var coordinates = pathPoints.Select(p => new List<double> { p.Longitude, p.Latitude }).ToList();

            var geometry = new JObject { ["type"] = "LineString", ["coordinates"] = JArray.FromObject(coordinates) };
            var feature = new JObject
            {
                ["type"] = "Feature",
                ["properties"] = new JObject { ["stroke"] = "#5500FF", ["stroke-width"] = 2 },
                ["geometry"] = geometry
            };

            return WebUtility.UrlEncode(feature.ToString(Formatting.None));
        }

        private static List<LocationInfo> GetPathPoints(LocationInfo start, LocationInfo end)
        {
            const int steps = 20;
            var points = new List<LocationInfo>();
            for (int i = 0; i <= steps; i++)
            {
                points.Add(GetIntermediatePoint(start, end, (double)i / steps));
            }
            return points;
        }

        private static BoundingBox GetBoundingBox(List<LocationInfo> points)
        {
            return new BoundingBox(
                MinLon: points.Min(p => p.Longitude),
                MinLat: points.Min(p => p.Latitude),
                MaxLon: points.Max(p => p.Longitude),
                MaxLat: points.Max(p => p.Latitude)
            );
        }

        private static MapView GetCalculatedMapView(BoundingBox bbox, int width, int height)
        {
            const double PADDING_FACTOR = 0.15;

            double lonSpan = bbox.MaxLon - bbox.MinLon;
            double latSpan = bbox.MaxLat - bbox.MinLat;

            var paddedBbox = new BoundingBox(
                MinLon: bbox.MinLon - (lonSpan * PADDING_FACTOR),
                MinLat: bbox.MinLat - (latSpan * PADDING_FACTOR),
                MaxLon: bbox.MaxLon + (lonSpan * PADDING_FACTOR),
                MaxLat: bbox.MaxLat + (latSpan * PADDING_FACTOR)
            );

            double centerLon = (paddedBbox.MinLon + paddedBbox.MaxLon) / 2;
            double centerLat = (paddedBbox.MinLat + paddedBbox.MaxLat) / 2;

            double zoom;
            if (bbox.MaxLon == bbox.MinLon || bbox.MaxLat == bbox.MinLat)
            {
                zoom = 8;
            }
            else
            {
                double lonFraction = (paddedBbox.MaxLon - paddedBbox.MinLon) / 360;
                double latFraction = Math.Abs(MercatorY(paddedBbox.MaxLat) - MercatorY(paddedBbox.MinLat)) / (2 * Math.PI);

                double zoomLon = Math.Log(width / (lonFraction * 512), 2);
                double zoomLat = Math.Log(height / (latFraction * 512), 2);
                zoom = Math.Min(zoomLat, zoomLon);
            }

            return new MapView(new LocationInfo { Latitude = centerLat, Longitude = centerLon }, Math.Min(zoom, 8));
        }

        private static double MercatorY(double lat) => Math.Log(Math.Tan(ToRadians(lat) / 2 + Math.PI / 4));


        //Orthodome Math
        private static LocationInfo GetIntermediatePoint(LocationInfo start, LocationInfo end, double fraction)
        {
            double lat1 = ToRadians(start.Latitude);
            double lon1 = ToRadians(start.Longitude);
            double lat2 = ToRadians(end.Latitude);
            double lon2 = ToRadians(end.Longitude);

            double d = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((lat1 - lat2) / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon1 - lon2) / 2), 2)));

            if (Math.Abs(Math.Sin(d)) < 1e-9) return start;

            double A = Math.Sin((1 - fraction) * d) / Math.Sin(d);
            double B = Math.Sin(fraction * d) / Math.Sin(d);

            double x = A * Math.Cos(lat1) * Math.Cos(lon1) + B * Math.Cos(lat2) * Math.Cos(lon2);
            double y = A * Math.Cos(lat1) * Math.Sin(lon1) + B * Math.Cos(lat2) * Math.Sin(lon2);
            double z = A * Math.Sin(lat1) + B * Math.Sin(lat2);

            double lat = Math.Atan2(z, Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
            double lon = Math.Atan2(y, x);

            return new LocationInfo { Latitude = ToDegrees(lat), Longitude = ToDegrees(lon) };
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
        private static double ToDegrees(double radians) => radians * 180.0 / Math.PI;
    }
}