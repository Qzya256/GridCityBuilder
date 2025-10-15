using UnityEngine;
using Zenject;

public class GameBootstrapInstaller : MonoInstaller
{
    [SerializeField] private CoroutineRunner coroutineRunner;

    public override void InstallBindings()
    {
        // Infrastructure
        Container.Bind<CoroutineRunner>().FromInstance(coroutineRunner).AsSingle();
        Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle();
        Container.Bind<DataService>().AsSingle();

        // Entry point
        Container.BindInterfacesAndSelfTo<GameBootstrap>().AsSingle().NonLazy();
    }
}

public class GameBootstrap : IInitializable
{
    private readonly SceneLoader _sceneLoader;

    public GameBootstrap(SceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public void Initialize()
    {
        Debug.Log("Bootstrap initialized — loading Game scene...");
        _sceneLoader.Load("Game");
    }
}
