using Server.Dtos;

namespace Server.Hubs
{
    public interface ITreeHubClient
    {
        Task ReceiveHello(string msg);
        Task ReceiveWarning(MeasurementDto measurement, bool isWarning);
    }
}
