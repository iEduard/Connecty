
namespace Connecty
{
    /// <summary>
    /// This Clas represents the Summary of all Settings could be Stored and loaded for and with the Application
    /// </summary>
    public class ConnectySetings
    {
        public ConnectionSettings connectionSettings { set; get; }
        public ApplicationSettings applicationSettings { set; get; }
        public ViewSettings viewSettings { set; get; }

        public ConnectySetings()
        {
            // Init the local Variables
            connectionSettings = new ConnectionSettings();
            applicationSettings = new ApplicationSettings();
            viewSettings = new ViewSettings();

        }

    }
}
