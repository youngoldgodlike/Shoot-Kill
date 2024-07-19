namespace Assets.UI.Prefabs.Abilities.Scripts
{
    public class StoreCountUp : AbilityIntCell
    {
        
        public override void Upgrade()
        {
            gameSession.matchData.currentGun.IncreaseStoreCount(value);
            
            base.Upgrade();
        }
        protected override void SetText() => text.text = $"Колличество патрон: +{value}";
    }
}