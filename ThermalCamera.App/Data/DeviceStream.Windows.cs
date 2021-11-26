using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace ThermalCamera.App.Data
{
    public class DeviceStream : BaseDeviceStream
    {
        private readonly SerialDevice _serialDevice;
        private readonly DataReader _dataReader;

        public DeviceStream(SerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
            _dataReader = new DataReader(_serialDevice.InputStream);
            _dataReader.InputStreamOptions = InputStreamOptions.Partial;
        }

        protected override async Task CheckStream(CancellationToken cancellationToken)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            try
            {
                var inBufferCnt = await _dataReader.LoadAsync(256).AsTask(cts.Token);
                var runStr = _dataReader.ReadString(inBufferCnt);
                WriteOutput(runStr);
            }
            catch (TaskCanceledException)
            {
                cts.Dispose();
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await base.DisposeAsync();
            _dataReader.Dispose();
            _serialDevice.Dispose();
        }
    }
}
