using UnityEngine;
using StabToRestart.Configuration;
using Zenject;

namespace StabToRestart
{
    internal class StabToRestartSetupInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            if (StabToRestartConfig.Instance.IsEnabled)
            {
                Container.Bind<StabToRestartController>()
                    .FromNewComponentOnNewGameObject()
                    .WithGameObjectName("StabToRestartController")
                    .AsSingle()
                    .NonLazy();
            }
            
        }
    }
}
