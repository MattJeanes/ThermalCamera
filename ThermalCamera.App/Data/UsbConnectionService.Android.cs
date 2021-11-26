using Android.Content;
using Android.Hardware.Usb;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Data
{
    public partial class UsbConnectionService
    {
        public Task<List<UsbConnectionData>> GetData()
        {
            var usbService = (UsbManager)Android.App.Application.Context.GetSystemService(Context.UsbService);
            var accessories = usbService.GetAccessoryList();
            var usbConnectionData = accessories?.Select(x => new UsbConnectionData
            {
                Id = x.Serial,
                Summary = x.Description
            }).ToList() ?? new List<UsbConnectionData>();

            if (usbConnectionData.Count == 0)
            {
                usbConnectionData.Add(new UsbConnectionData
                {
                    Id = "N/A",
                    Summary = "No devices"
                });
            }

            return Task.FromResult(usbConnectionData);
        }

        public async Task<IDeviceStream> Connect(UsbConnectionData data)
        {
            return new DeviceStream();
            // placeholder
        }

        public IDeviceStream? GetStream()
        {
            return null;
            // placeholder
        }
    }
}
