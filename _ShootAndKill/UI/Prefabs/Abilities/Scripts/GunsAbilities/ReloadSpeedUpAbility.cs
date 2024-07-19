
namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class ReloadSpeedUpAbility : AbilityFloatCell
    {
        
        public override void Upgrade()
        {
            gameSession.matchData.currentGun.ReduceReloadSpeed(value);
            base.Upgrade();
        }

        protected override void SetText() => text.text = $"Скорость перезарядки: -{value}%";
    }
}