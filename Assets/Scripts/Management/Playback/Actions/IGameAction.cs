namespace Management.Playback.Actions
{
    public interface IGameAction
    {
        void Play();
        bool ShouldPlay() => true;

        IGameAction Empty => new GameAction(() => {});
    }
}