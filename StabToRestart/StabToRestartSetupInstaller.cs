using UnityEngine;
using Zenject;

namespace StabToRestart
{
    internal class StabToRestartSetupInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<StabToRestartController>().FromNewComponentOnNewGameObject().WithGameObjectName("StabToRestartController").AsSingle().NonLazy();
            Container.Bind<CapsuleCollider>().FromNewComponentOnNewGameObject().WithGameObjectName("Body stab collider").WhenInjectedInto<StabToRestartController>();
        }
    }
}
