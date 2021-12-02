using Microsoft.AspNetCore.Components;
using ThermalCamera.App.Data;
using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App.Pages;

public partial class FetchData
{
    [Inject]
    private UsbConnectionService UsbConnectionService { get; set; } = default!;


    private List<UsbConnectionData>? _usbConnectionData = new List<UsbConnectionData>();
    private IDeviceStream? _deviceStream;
    private string _streamData = string.Empty;
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;

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
        await InvokeAsync(() =>
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
                    ProcessData(data);
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

    private void ProcessData(string data)
    {
        try
        {
            if (data.Contains("data:")) { return; } // sometimes the stream contains randomly 'data:' on first connection, so we ignore this
            var array = data.TrimEnd(',').Split(',').Select(x => decimal.Parse(x)).ToArray();
            if (array.Length != 768)
            {
                _errorMessage = $"Array length is different from expected, length: {array.Length}";
                return;
            }
            _streamData = Timestamp(data);
        }
        catch (Exception e)
        {
            _errorMessage = Timestamp($"Failed to process data: {e}");
        }
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
