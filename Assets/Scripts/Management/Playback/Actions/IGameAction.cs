namespace Management.Playback.Actions
{
    public interface IGameAction
    {
        float GetTime();
        void Play();
        bool ShouldPlay() => GameRecorder.Instance.TimeSinceStart >= GetTime();

        IGameAction Empty => new GameAction(() => {});
    }
}