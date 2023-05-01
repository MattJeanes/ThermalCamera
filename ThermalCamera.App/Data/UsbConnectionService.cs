using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Data;

public abstract class BaseUsbConnectionService : IUsbConnectionService
{
    protected IDeviceStream _deviceStream;

    public abstract Task<DataResult<IDeviceStream>> Connect(UsbConnectionData data);
    public abstract Task<List<UsbConnectionData>> GetData();

    public IDeviceStream GetStream()
    {
        return _deviceStream;
    }
}
