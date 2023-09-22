using System;
using System.Net.Http;
using System.Threading.Tasks;
using ScanBus.Models;

namespace ScanBus.Service;

public class CoordService
{
    static readonly HttpClient client = new();

    public static async Task<Address> GetAddress(string longitude, string latitude)
    {
        string apiUrl = $"https://geocode.maps.co/reverse?lat={latitude}&lon={longitude}";

        GeoCode geoCode = null;
        HttpResponseMessage response = await client.GetAsync(apiUrl);
        geoCode = await response.Content.ReadAsAsync<GeoCode>();

        return geoCode.Address;
    }
}

