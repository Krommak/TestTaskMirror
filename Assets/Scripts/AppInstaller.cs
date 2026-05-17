using App.Client;
using App.Server;
using Zenject;

namespace App
{
    public class AppInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<HelloMessageHandler>().AsSingle();
            Container.Bind<MirrorServer>().AsSingle();
            Container.Bind<MirrorClient>().AsSingle();

            Container.BindInterfacesAndSelfTo<AppStart>().AsSingle().NonLazy();
        }
    }
}
