namespace ThermalCamera.App.Data.Interfaces;

public interface IDeviceStream : IAsyncDisposable
{
    Task Start();
    Task SendData(string data);
    public event EventHandler<OutputEventArgs>? Output;
}
