using ThermalCamera.App.Data.Interfaces;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace ThermalCamera.App.Data;

public partial class UsbConnectionService : BaseUsbConnectionService
{
    private SerialDevice _serialDevice;

    public override async Task<List<UsbConnectionData>> GetData()
    {
        var devices = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

        var usbConnectionData = devices.Select(x => new UsbConnectionData
        {
            Id = x.Id,
            Summary = x.Name
        }).ToList();

        return usbConnectionData;
    }

    public override async Task<DataResult<IDeviceStream>> Connect(UsbConnectionData data)
    {
        if (_deviceStream != null)
        {
            await _deviceStream.DisposeAsync();
            _deviceStream = null;
        }
        _serialDevice = await SerialDevice.FromIdAsync(data.Id);
        _deviceStream = new DeviceStream(_serialDevice);
        await _deviceStream.Start();
        return DataResult.GetSuccess(_deviceStream);
    }
}
