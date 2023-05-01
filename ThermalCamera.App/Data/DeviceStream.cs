using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Data;

public abstract class BaseDeviceStream : IDeviceStream
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private string _dataBuffer;

    public BaseDeviceStream()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _dataBuffer = string.Empty;
    }

    protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

    public abstract Task Start();

    public virtual ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        return ValueTask.CompletedTask;
    }

    public event EventHandler<OutputEventArgs> Output;

    public abstract Task SendData(string data);

    protected void ProcessData(string data)
    {
        _dataBuffer += data;
        var split = _dataBuffer.Split('\n');
        foreach (var dataSplit in split.Skip(1).SkipLast(1))
        {
            WriteOutput(dataSplit);
        }
        _dataBuffer = "\n" + split.Last();
    }

    protected void WriteOutput(string output)
    {
        Output?.Invoke(this, new OutputEventArgs { Output = output });
    }
}
