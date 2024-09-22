using System.Collections;
using Effects.Definition;
using Management;
using Management.Board;
using UnityEngine;

namespace Effects.Implementation
{
    public class SpeedEffect : BaseEffect
    {
        private Color _color;

        private bool _finished;
        
        protected override bool IsFinished()
        {
            return _finished;
        }

        private void Start()
        {
            _color = Block.GetColor();
            Block.SetColor(Color.white);
        }

        protected override void OnLanding()
        {
            Debug.Log("Calling OnLanding");
            var (x, y) = Block.BoardLocation;
            var shouldSpeedUp = (x > 0 && Block.IsStranger(BoardManager.Instance.GetBoardPosition(x - 1, y))) ||
                                (y > 0 && Block.IsStranger(BoardManager.Instance.GetBoardPosition(x, y - 1))) ||
                                (x < BoardManager.Instance.width && Block.IsStranger(BoardManager.Instance.GetBoardPosition(x + 1, y))) ||
                                (y < BoardManager.Instance.height && Block.IsStranger(BoardManager.Instance.GetBoardPosition(x, y + 1)));
            if (shouldSpeedUp) StartCoroutine(SpeedUp());
            else Block.SetColor(_color);
        }

        private IEnumerator SpeedUp()
        {
            GameManager.Instance.AdditionalSpeedModifier += 2;
            yield return new WaitForSeconds(10);
            GameManager.Instance.AdditionalSpeedModifier -= 2;
            Block.SetColor(_color);
            _finished = true;
        }
    }
}