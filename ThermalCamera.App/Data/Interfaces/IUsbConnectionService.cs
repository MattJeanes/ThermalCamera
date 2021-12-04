namespace ThermalCamera.App.Data.Interfaces;

public interface IUsbConnectionService
{
    Task<DataResult<IDeviceStream>> Connect(UsbConnectionData data);
    Task<List<UsbConnectionData>> GetData();
    IDeviceStream? GetStream();
}
