using Assets.Prefabs.Characters.MainHero.Scripts.XPSystem;
using UnityEngine;
using Zenject;

namespace _Shoot_Kill.Architecture.Scripts.Session
{
    public class MatchInstaller : MonoInstaller
    {
        [SerializeField] private HeroConfig _heroConfig;
        [SerializeField] private ExpSystemConfig _expSystemConfig;

        public override void InstallBindings()
        {
            Container.Bind<HeroConfig>().FromInstance(_heroConfig).AsSingle();
            Container.Bind<ExpSystemConfig>().FromInstance(_expSystemConfig).AsSingle();
        }
    }
}