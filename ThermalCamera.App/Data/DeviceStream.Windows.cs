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

        public DeviceStream(SerialDevice serialDevice)
        {
            _serialDevice = serialDevice;
            _serialDevice.ReadTimeout = TimeSpan.FromSeconds(1);
            _serialDevice.BaudRate = 115200;
        }

        protected override async Task CheckStream()
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(1000);
            using var dataReader = new DataReader(_serialDevice.InputStream);
            try
            {
                dataReader.InputStreamOptions = InputStreamOptions.Partial;
                var inBufferCnt = await dataReader.LoadAsync(1024).AsTask(cts.Token);
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
            _serialDevice.Dispose();
        }
    }
}
