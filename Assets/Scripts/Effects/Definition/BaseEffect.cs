using Management;
using UnityEngine;

namespace Effects.Definition
{
    public abstract class BaseEffect : MonoBehaviour
    {
        protected virtual void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
        }

        protected virtual void Update()
        {
            if (IsFinished())
                Remove();
        }

        protected virtual void GamePhaseListener(GamePhase phase)
        {
            Remove();
        }

        protected virtual void Remove()
        {
            GameManager.Instance.RemoveGamePhaseListener(GamePhaseListener);
            Destroy(this);
        }

        protected virtual bool IsFinished() => true;
    }
}