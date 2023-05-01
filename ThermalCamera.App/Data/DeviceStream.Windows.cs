using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace ThermalCamera.App.Data;

public class DeviceStream : BaseDeviceStream
{
    private readonly SerialDevice _serialDevice;
    private Task _task;

    public DeviceStream(SerialDevice serialDevice)
    {
        _serialDevice = serialDevice;
        _serialDevice.ReadTimeout = TimeSpan.FromSeconds(1);
        _serialDevice.WriteTimeout = TimeSpan.FromSeconds(1);
        _serialDevice.BaudRate = 115200;
        _serialDevice.Parity = SerialParity.None;
        _serialDevice.StopBits = SerialStopBitCount.One;
        _serialDevice.DataBits = 8;
        _serialDevice.Handshake = SerialHandshake.None;
    }

    public override Task Start()
    {
        _task = Task.Run(BackgroundThread);
        return Task.CompletedTask;
    }

    private async Task BackgroundThread()
    {
        while (true)
        {
            if (CancellationToken.IsCancellationRequested)
            {
                break;
            }
            await CheckStream();
        }
    }

    private async Task CheckStream()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1000);
        using var dataReader = new DataReader(_serialDevice.InputStream);
        try
        {
            dataReader.InputStreamOptions = InputStreamOptions.Partial;
            var inBufferCnt = await dataReader.LoadAsync(256).AsTask(cts.Token);
            var runStr = dataReader.ReadString(inBufferCnt);
            ProcessData(runStr);
        }
        catch (TaskCanceledException)
        {
            cts.Dispose();
        }
        finally
        {
            dataReader.DetachStream();
        }
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        if (_task != null)
        {
            await _task;
        }
        _serialDevice.Dispose();
    }

    public override async Task SendData(string data)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1000);
        using var dataWriter = new DataWriter(_serialDevice.OutputStream);
        try
        {
            dataWriter.WriteString(data);
            await dataWriter.StoreAsync().AsTask(cts.Token);
        }
        catch (TaskCanceledException)
        {
            cts.Dispose();
        }
        finally
        {
            dataWriter.DetachStream();
        }
    }
}
