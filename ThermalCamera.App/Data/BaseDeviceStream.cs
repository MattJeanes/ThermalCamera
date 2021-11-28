using System;
using System.Threading;
using System.Threading.Tasks;
using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Data
{
    public abstract class BaseDeviceStream : IDeviceStream
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task? _task;

        public BaseDeviceStream()
        {
            _cancellationTokenSource = new CancellationTokenSource();
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

        protected void WriteOutput(string output)
        {
            Output?.Invoke(this, new OutputEventArgs { Output = output });
        }
    }
}
