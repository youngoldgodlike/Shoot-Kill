using Helpers.Debugging;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class GamesessionInstaller : MonoInstaller
{
    [SerializeField, ReadOnly] private GameSession _session;
    [SerializeField, ReadOnly] private GameDebug _debug;
    private DiContainer _container;

    private const string MY_DEBUG_PATH = "MyDebug";

    public override void InstallBindings()
    {
        _session = FindObjectOfType<GameSession>();
        Container.Bind<GameSession>().FromInstance(_session).AsSingle();
        
        _debug = Instantiate(Resources.Load<GameDebug>(MY_DEBUG_PATH));
        Container.Bind<GameDebug>().FromInstance(_debug).AsSingle();
    }
}
