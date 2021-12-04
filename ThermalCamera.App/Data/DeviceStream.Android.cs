using Android.Content;
using Android.Hardware.Usb;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Extensions;
using System.Text;

namespace ThermalCamera.App.Data;

public class DeviceStream : BaseDeviceStream
{
    private readonly UsbManager _usbManager;
    private readonly SerialInputOutputManager _serialIOManager;
    private readonly UsbSerialPort _port;
    const int WRITE_WAIT_MILLIS = 200;

    public DeviceStream(IUsbSerialDriver usbSerialDriver)
    {
        var usbManager = Application.Context.GetSystemService(Context.UsbService) as UsbManager;
        if (usbManager == null) { throw new Exception("Failed to get UsbManager"); }
        _usbManager = usbManager;
        _port = usbSerialDriver.Ports.First();
        _serialIOManager = new SerialInputOutputManager(_port)
        {
            BaudRate = 115200,
            DataBits = 8,
            StopBits = StopBits.One,
            Parity = Parity.None,
        };
        _serialIOManager.DataReceived += (sender, e) =>
        {
            var str = Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
            ProcessData(str);
        };
        _serialIOManager.ErrorReceived += (sender, e) =>
        {
            WriteOutput($"error:Error in serial connection: {e.ToString}");
        };
    }

    public override Task Start()
    {
        _serialIOManager.Open(_usbManager);
        return Task.CompletedTask;
    }

    public override Task SendData(string data)
    {
        if (_serialIOManager.IsOpen)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            _port.Write(buffer, WRITE_WAIT_MILLIS);
        }
        return Task.CompletedTask;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        _serialIOManager.Dispose();
    }
}
