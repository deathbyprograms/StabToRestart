using IPA;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace StabToRestart
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            zenjector.Install<StabToRestartSetupInstaller>(Location.StandardPlayer | Location.CampaignPlayer);
            Log.Info("StabToRestart initialized.");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");

        }
    }
}
