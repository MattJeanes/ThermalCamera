using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Data;

public abstract class BaseDeviceStream : IDeviceStream
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private string _dataBuffer;
    private Task? _task;

    public BaseDeviceStream()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _dataBuffer = string.Empty;
    }

    public Task Start()
    {
        _task = Task.Run(BackgroundThread);
        return Task.CompletedTask;
    }

    private async Task BackgroundThread()
    {
        while (true)
        {
            if (_cancellationTokenSource.IsCancellationRequested)
            {
                break;
            }
            await CheckStream();
        }
    }

    public async virtual ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();

        if (_task != null)
        {
            await _task;
        }
    }

    public event EventHandler<OutputEventArgs>? Output;

    protected abstract Task CheckStream();
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
