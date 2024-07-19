
namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class DamageUpAbility : AbilityFloatCell
    {

        public override void Upgrade()
        {
            gameSession.matchData.currentGun.IncreaseGunDamage(value);
            
            base.Upgrade();
        }
        
        protected override void SetText() => text.text = $"Повышение урона: +{value}%";
    }
}
