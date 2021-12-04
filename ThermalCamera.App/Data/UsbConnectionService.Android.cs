using Android.Content;
using Android.Hardware.Usb;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Extensions;
using Hoho.Android.UsbSerial.Util;
using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Data;

public partial class UsbConnectionService : BaseUsbConnectionService
{
    private readonly UsbManager _usbManager;
    private readonly UsbSerialProber _prober;

    public UsbConnectionService()
    {
        var usbManager = Application.Context.GetSystemService(Context.UsbService) as UsbManager;
        if (usbManager == null) { throw new Exception("Failed to get UsbManager"); }
        _usbManager = usbManager;
        _prober = UsbSerialProber.GetDefaultProber();
    }

    public override async Task<List<UsbConnectionData>> GetData()
    {
        var drivers = await _prober.FindAllDriversAsync(_usbManager);
        var usbConnectionData = drivers.Select(x => new UsbConnectionData
        {
            Id = x.Device.DeviceId.ToString(),
            Summary = x.Device.DeviceName
        }).ToList() ?? new List<UsbConnectionData>();

        if (usbConnectionData.Count == 0)
        {
            usbConnectionData.Add(new UsbConnectionData
            {
                Id = "N/A",
                Summary = "No devices"
            });
        }

        return usbConnectionData;
    }

    public override async Task<DataResult<IDeviceStream>> Connect(UsbConnectionData data)
    {
        var drivers = await _prober.FindAllDriversAsync(_usbManager);
        var driver = drivers.FirstOrDefault(x => x.Device.DeviceId.ToString() == data.Id);
        if (driver == null)
        {
            return DataResult.GetFailure<IDeviceStream>("Failed to find device");
        }
        var permissionGranted = await _usbManager.RequestPermissionAsync(driver.Device, Application.Context);
        if (!permissionGranted)
        {
            return DataResult.GetFailure<IDeviceStream>("Permission not granted to access device");
        }
        _deviceStream = new DeviceStream(driver);
        await _deviceStream.Start();
        return DataResult.GetSuccess(_deviceStream);
    }
}
