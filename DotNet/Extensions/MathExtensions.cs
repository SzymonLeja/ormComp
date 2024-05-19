namespace Endpoints.Extensions;

public static class MathExtensions
{
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        int R = 6371;
        double latDistance = ToRadians(lat2 - lat1);
        double lonDistance = ToRadians(lon2 - lon1);
        double a = Math.Sin(latDistance / 2) * Math.Sin(latDistance / 2)
                + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
                * Math.Sin(lonDistance / 2) * Math.Sin(lonDistance / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = R * c;

        return distance;
        
        static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }

}