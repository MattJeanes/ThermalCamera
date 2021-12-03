using Excubo.Blazor.Canvas;
using Microsoft.AspNetCore.Components;
using ThermalCamera.App.Data;
using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Pages;

public partial class FetchData
{
    [Inject]
    private UsbConnectionService UsbConnectionService { get; set; } = default!;

    private const int SENSOR_WIDTH = 32;
    private const int SENSOR_HEIGHT = 24;
    private const int CANVAS_SCALE = 16;


    private List<UsbConnectionData>? _usbConnectionData = new List<UsbConnectionData>();
    private IDeviceStream? _deviceStream;
    private string _streamData = string.Empty;
    private decimal[]? _temperatureData;
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private Canvas _canvas = default!;

    protected override async Task OnInitializedAsync()
    {
        await Refresh();
        ConnectStream();
    }

    private async Task Refresh()
    {
        _usbConnectionData = await UsbConnectionService.GetData();
    }

    private void ConnectStream()
    {
        var deviceStream = UsbConnectionService.GetStream();
        if (deviceStream != null)
        {
            HandleStream(deviceStream);
        }
    }

    private void HandleStream(IDeviceStream deviceStream)
    {
        _deviceStream = deviceStream;
        _deviceStream.Output += HandleOutputEvent;
    }

    private async void HandleOutputEvent(object? sender, OutputEventArgs args)
    {
        await InvokeAsync(async () =>
        {
            var splitIndex = args.Output.IndexOf(':');
            if (splitIndex == -1)
            {
                ProcessUnknown("N/A", args.Output);
                return;
            }
            var command = args.Output.Substring(0, splitIndex);
            string data;
            if (args.Output.Length > splitIndex + 1)
            {
                data = args.Output.Substring(splitIndex + 1);
            }
            else
            {
                data = string.Empty;
            }
            switch (command)
            {
                case "data":
                    await ProcessDataAsync(data);
                    break;
                case "error":
                    ProcessError(data);
                    break;
                case "status":
                    ProcessStatus(data);
                    break;
                default:
                    ProcessUnknown(command, data);
                    break;
            }
            StateHasChanged();
        });
    }

    private async Task ProcessDataAsync(string data)
    {
        try
        {
            if (data.Contains("data:")) { return; } // sometimes the stream contains randomly 'data:' on first connection, so we ignore this
            var array = data.TrimEnd(',').Split(',').Select(x => decimal.Parse(x)).ToArray();
            if (array.Length != (SENSOR_WIDTH * SENSOR_HEIGHT))
            {
                _errorMessage = $"Array length is different from expected, length: {array.Length}";
                return;
            }
            _streamData = Timestamp(data);
            _temperatureData = array;
            await UpdateTemperatureDisplay();
        }
        catch (Exception e)
        {
            _errorMessage = Timestamp($"Failed to process data: {e}");
        }
    }

    private async Task UpdateTemperatureDisplay()
    {
        if (_temperatureData == null) { return; }
        await using var ctx = await _canvas.GetContext2DAsync();
        await using var batch = ctx.CreateBatch();
        for (var i = 0; i < _temperatureData.Length; i++)
        {
            var column = i % SENSOR_WIDTH;
            var row = (i - column) / SENSOR_WIDTH;
            var tempX = (SENSOR_HEIGHT - row - 1) * SENSOR_WIDTH;
            var temp = _temperatureData[tempX + column];
            var tempRGB = Math.Clamp(temp * 8, 0, 255);
            await batch.FillAndStrokeStyles.FillStyleAsync($"rgb({tempRGB},{tempRGB},{tempRGB})");
            await batch.FillRectAsync(column * CANVAS_SCALE, row * CANVAS_SCALE, CANVAS_SCALE, CANVAS_SCALE);
        }
        var midTemp = _temperatureData[((SENSOR_HEIGHT/2) * SENSOR_WIDTH) + (SENSOR_WIDTH / 2)];
        await batch.FontAsync("20px serif");
        await batch.FillAndStrokeStyles.FillStyleAsync("#ff0000");
        await batch.FillTextAsync(midTemp + "°C", (SENSOR_WIDTH * CANVAS_SCALE) / 2, (SENSOR_HEIGHT * CANVAS_SCALE) / 2);
    }

    private void ProcessError(string data)
    {
        _errorMessage = Timestamp(data);
    }

    private void ProcessStatus(string data)
    {
        _statusMessage = Timestamp(data);
    }

    private void ProcessUnknown(string command, string data)
    {
        _errorMessage = Timestamp($"Unknown data received: {command} {data}");
    }

    private string Timestamp(string data)
    {
        return $"[{DateTime.Now:T}] {data}";
    }

    private async Task Connect(UsbConnectionData data)
    {
        if (_deviceStream != null)
        {
            _deviceStream.Output -= HandleOutputEvent;
            _deviceStream = null;
        }
        _deviceStream = null;
        var deviceStream = await UsbConnectionService.Connect(data);
        HandleStream(deviceStream);
    }

    private async Task RequestStatus()
    {
        if (_deviceStream != null)
        {
            await _deviceStream.SendData("status\n");
        }
    }
}
