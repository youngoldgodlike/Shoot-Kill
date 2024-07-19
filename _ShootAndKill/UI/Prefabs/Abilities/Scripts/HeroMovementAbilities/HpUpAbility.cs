namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class HpUpAbility : AbilityIntCell
    {
        public override void Upgrade()
        {
          gameSession.matchData.heroData.healthData.IncreaseHealth(value);
        }

        protected override void SetText() => text.text = $"Увеличить здоровье: +{value}";
    }
}
