using System.Threading;
using System.Threading.Tasks;

namespace ThermalCamera.App.Data
{
    public class DeviceStream : BaseDeviceStream
    {
        protected override Task CheckStream(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
