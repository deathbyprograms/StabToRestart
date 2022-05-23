using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using StabToRestart.Configuration;
using StabToRestart.UI.ViewControllers;
using BeatSaberMarkupLanguage.Settings;
using IPALogger = IPA.Logging.Logger;

namespace StabToRestart
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        public Plugin(IPALogger logger, Config config, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;

            StabToRestartConfig.Instance = config.Generated<StabToRestartConfig>(true);

            zenjector.Install<StabToRestartSetupInstaller>(Location.StandardPlayer | Location.CampaignPlayer);

            Log.Info("StabToRestart initialized.");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            BSMLSettings.instance.AddSettingsMenu("Stab To Restart", $"{nameof(StabToRestart)}.UI.Views.StabToRestartConfigView.bsml", new StabToRestartConfigViewController());
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log.Debug("OnApplicationQuit");
            if (BSMLSettings.instance != null)
            {
                BSMLSettings.instance.RemoveSettingsMenu(this);
            }
        }
    }
}
