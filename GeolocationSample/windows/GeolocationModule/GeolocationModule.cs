using Microsoft.ReactNative.Managed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace GeolocationModule
{
    [ReactModule]
    class GeolocationModule
    {
        [ReactMethod("getCoordinatesWithPromise")]
        public async void GetCoordinatesWithPromise(IReactPromise<string> promise)
        {
            try
            {
                Geolocator geolocator = new Geolocator();
                var position = await geolocator.GetGeopositionAsync();

                string result = $"Latitude: {position.Coordinate.Point.Position.Latitude} - Longitude: {position.Coordinate.Point.Position.Longitude}";

                promise.Resolve(result);
            }
            catch (Exception e)
            {
                promise.Reject(new ReactError { Exception = e });
            }
        }

        [ReactMethod("getCoordinatesWithCallback")]
        public async void GetCoordinatesWithCallback(Action<string> resolve, Action<string> reject)
        {
            try
            {
                Geolocator geolocator = new Geolocator();
                var position = await geolocator.GetGeopositionAsync();

                string result = $"Latitude: {position.Coordinate.Point.Position.Latitude} - Longitude: {position.Coordinate.Point.Position.Longitude}";

                resolve(result);
            }
            catch (Exception e)
            {
                reject(e.Message);
            }
        }
    }
}
