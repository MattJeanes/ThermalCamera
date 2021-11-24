using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace ThermalCamera.App.Data
{
    public partial class UsbConnectionService
    {
        public async Task<List<UsbConnectionData>> GetData()
        {
            var devices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

            var usbConnectionData = devices.Select(x => new UsbConnectionData
            {
                Summary = x.Name
            }).ToList();

            return usbConnectionData;
        }
    }
}
