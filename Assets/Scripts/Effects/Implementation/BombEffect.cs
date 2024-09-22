using Effects.Definition;
using Management.Audio;
using Management.Board;
using Management.Score;
using Unity.Mathematics;
using UnityEngine;

namespace Effects.Implementation
{
    public class BombEffect : BaseEffect
    {
        private Color _color;

        private bool _finished;
        
        private void Start()
        {
            _color = Block.GetColor();
            Block.SetColor(Color.black);
        }

        private void Explode()
        {
            SoundManager.Instance.Explode();
            var (x, y) = Block.BoardLocation;
            for (var dX = -2; dX <= 2; dX++)
                for (var dY = -2; dY <= 2; dY++)
                    if (math.abs(dY) + math.abs(dX) <= 2)
                        if (BoardManager.Instance.ReleaseBoardPosition(x + dX, y + dY))
                            ScoreManager.Instance.AddScore(50);
        }

        protected override bool IsFinished()
        {
            return _finished;
        }

        protected override void OnLanding()
        {
            var (x, y) = Block.BoardLocation;
            var shouldExplode = (x > 0 && Block.IsStranger(BoardManager.Instance.GetBoardPosition(x - 1, y))) ||
                                (y > 0 && Block.IsStranger(BoardManager.Instance.GetBoardPosition(x, y - 1))) ||
                                (x < BoardManager.Instance.width && Block.IsStranger(BoardManager.Instance.GetBoardPosition(x + 1, y))) ||
                                (y < BoardManager.Instance.height && Block.IsStranger(BoardManager.Instance.GetBoardPosition(x, y + 1)));
            if (shouldExplode) Explode();
            else Block.SetColor(_color);
            
            _finished = true;
        }
    }
}