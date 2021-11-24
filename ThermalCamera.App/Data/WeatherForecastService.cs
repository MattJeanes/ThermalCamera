#if ANDROID
using Android.Content;
using Android.Hardware.Usb;
#endif
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ThermalCamera.App.Data
{
	public class WeatherForecastService
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
		{
#if ANDROID
            var usbService = (UsbManager)Android.App.Application.Context.GetSystemService(Context.UsbService);
			var accessories = usbService.GetAccessoryList();
			var text = accessories?.FirstOrDefault()?.Description ?? "no accessory found";
#else
			var text = "windows placeholder";
#endif
			return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = startDate.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = text
			}).ToArray());;
		}
	}
}
