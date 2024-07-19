
namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class SpeedUpAbility : AbilityFloatCell
    {
        public override void Upgrade()
        {
            gameSession.matchData.heroData.IncreaseMoveSpeed(value);
            
            base.Upgrade();
        }

        protected override void SetText() => text.text = $"Скорость: +{value}%";
    }
}
