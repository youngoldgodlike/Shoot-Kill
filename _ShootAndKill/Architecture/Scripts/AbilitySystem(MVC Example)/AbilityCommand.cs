public class AbilityCommand : ICommand { // Скрипт отвечает за то что произойдет при нажатии
    private readonly AbilityData data;
    public float duration => data.duration;

    public AbilityCommand(AbilityData data) {
        this.data = data;
    }

    // выполнение команды
    public void Execute() {
        // EventBus<PlayerAnimationEvent>.Raise(new PlayerAnimationEvent {
        //     animationHash = data.animationHash
        // });
        // Какой-то ивент вызывающийся ебейшим образом через EventBus
    }
}