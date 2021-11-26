using System;
using System.Threading.Tasks;

namespace ThermalCamera.App.Data.Interfaces
{
    public interface IDeviceStream : IAsyncDisposable
    {
        Task Start();
        public event EventHandler<OutputEventArgs>? Output;
    }
}
