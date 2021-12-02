namespace ThermalCamera.App.Data;

public class DeviceStream : BaseDeviceStream
{
    public override Task SendData(string data)
    {
        throw new NotImplementedException();
    }

    protected override Task CheckStream()
    {
        throw new NotImplementedException();
    }
}
