using Management;
using Tetromino;
using UnityEngine;

namespace Effects.Definition
{
    public abstract class BaseEffect : MonoBehaviour
    {
        public BlockScript Block { get; set; }
        private bool _landed;
        
        protected virtual void Awake()
        {
            GameManager.Instance.AddGamePhaseListener(GamePhaseListener);
        }

        protected virtual void Update()
        {
            if (IsFinished())
            {
                Remove();
                return;
            }
            if (Block == null || !Block.IsOnBoard) return;
            if (!_landed) OnLanding();
            _landed = true;
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

        protected abstract void OnLanding();
    }
}