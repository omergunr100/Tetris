using Management;
using Management.Board;
using Tetromino;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerController : Singleton<PlayerController>
    {
        [SerializeField] private float movementSpeed = 3f;
        private float ActualMovementSpeed => movementSpeed * GameManager.Instance.GameSpeed;
        
        private float _timeSinceLastMove;
        
        private void Update()
        {
            _timeSinceLastMove += Time.deltaTime;
            
            if (GetRotationInput())
                BoardManager.Instance.TryRotate();
            
            if (_timeSinceLastMove >= 1f / ActualMovementSpeed)
            {
                _timeSinceLastMove = 0f;
                if (!TetrominoScript.Instance.Removed) BoardManager.Instance.TryMove(GetMovementInput());
            }
        }

        private Vector3 GetMovementInput()
        {
            var result = Vector3.zero;
            
            if (Input.GetKey(KeyCode.DownArrow))
                result += Vector3.down;
            
            if (Input.GetKey(KeyCode.LeftArrow))
                result += Vector3.left;
            
            if (Input.GetKey(KeyCode.RightArrow))
                result += Vector3.right;
            
            return result;
        }
        
        private bool GetRotationInput() => Input.GetKeyDown(KeyCode.UpArrow);
    }
}