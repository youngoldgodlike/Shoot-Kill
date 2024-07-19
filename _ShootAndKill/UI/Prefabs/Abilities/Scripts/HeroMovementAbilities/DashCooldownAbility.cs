
namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class DashCooldownAbility : AbilityFloatCell
    {
         public override void Upgrade()
         {
            gameSession.matchData.heroData.ReduceDashDelay(value);
            
            base.Upgrade();
         }

        protected override void SetText() => text.text = $"Перезарядка рывка: -{value}%";
        
    }
}