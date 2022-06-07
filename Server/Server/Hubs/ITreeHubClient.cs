using Server.Dtos;

namespace Server.Hubs
{
    /// <summary>
    /// Methods that can be called on the clients connected to SignalR
    /// </summary>
    public interface ITreeHubClient
    {
        /// <summary>
        /// Calls the warning handler method on the clients. Invoked when the server receives
        /// a measurement from a datalogger.
        /// </summary>
        /// <param name="measurement">Data of the measurement</param>
        /// <param name="isWarning">Indicates if the measurement is a warning for dry soil or not</param>
        /// <returns></returns>
        Task ReceiveWarning(MeasurementDto measurement, bool isWarning);

        /// <summary>
        /// Calls the handler on the datalogger, which is responsible for handling updates regarding 
        /// pairs corresponding to their datalogger id.
        /// </summary>
        /// <param name="plant"></param>
        /// <returns></returns>
        Task ReceiveDataloggerPair(PlantDto plant);
    }
}
