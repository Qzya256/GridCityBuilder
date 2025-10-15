using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private GridController gridController;
    [SerializeField] private Camera mainCamera;

    public override void InstallBindings()
    {
        // === Services ===
        Container.Bind<DataService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InputService>().AsSingle();

        // === Camera & Grid ===
        Container.Bind<Camera>().FromInstance(mainCamera).AsSingle();
        Container.Bind<GridController>().FromInstance(gridController).AsSingle();
        Container.Bind<CursorManager>().FromComponentInHierarchy().AsSingle();

        // === Building systems ===
        Container.BindInterfacesAndSelfTo<BuildingSelector>().AsSingle();
        Container.BindInterfacesAndSelfTo<BuildingPlacer>().AsSingle();
    }
}
