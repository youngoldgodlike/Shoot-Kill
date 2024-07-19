
namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class SpeedFireAbility : AbilityFloatCell
    {
        public override void Upgrade()
        {
           gameSession.matchData.currentGun.IncreaseFireRate(value);
           
           base.Upgrade();
        }
        protected override void SetText() => text.text = $"Скорость стрельбы: +{value}%";
    }
}
