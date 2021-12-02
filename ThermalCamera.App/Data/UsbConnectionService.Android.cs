using Android.Content;
using Android.Hardware.Usb;
using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Data;

public partial class UsbConnectionService
{
    public Task<List<UsbConnectionData>> GetData()
    {
        var usbService = Android.App.Application.Context.GetSystemService(Context.UsbService) as UsbManager;
        if (usbService == null)
        {
            return Task.FromResult(new List<UsbConnectionData>
                {
                    new UsbConnectionData
                    {
                        Id = "N/A",
                        Summary = "Unable to access USB service"
                    }
                });
        }
        var accessories = usbService.GetAccessoryList();
        var usbConnectionData = accessories?.Where(x => !string.IsNullOrEmpty(x.Serial) && !string.IsNullOrEmpty(x.Description)).Select(x => new UsbConnectionData
        {
            Id = x.Serial!,
            Summary = x.Description!
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

    public Task<IDeviceStream> Connect(UsbConnectionData data)
    {
        return Task.FromResult((IDeviceStream)new DeviceStream());
        // placeholder
    }

    public IDeviceStream? GetStream()
    {
        return null;
        // placeholder
    }
}
