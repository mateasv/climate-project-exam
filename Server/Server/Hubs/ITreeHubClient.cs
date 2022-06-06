using Server.Dtos;

namespace Server.Hubs
{
    public interface ITreeHubClient
    {
        Task ReceiveWarning(MeasurementDto measurement, bool isWarning);
        Task ReceiveDataloggerPair(DataloggerDto datalogger);
    }
}
