using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Stickman
{
    public class StickController : Singleton<StickController>
    {
        [SerializeField] private Rigidbody2D stickBody;
        [SerializeField] private float speed;
        [SerializeField] private float verticalSpeed;
        [SerializeField] private float maxSpeed;
        [SerializeField] private Animator animator;

        private Vector2 _movement;

        public Direction Look { get; private set; } = Direction.Right;
        public bool Grounded { get; set; } = true;

        public bool Moving => _movement.x != 0;
        public bool MovingRight => Look == Direction.Right && Moving;
        public bool MovingLeft => Look == Direction.Left && Moving;
        public bool Jump => _movement.y > 0;
        
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!stickBody.gameObject.activeSelf) return;
            _movement = context.ReadValue<Vector2>();
            Look = _movement.x < 0 ? Direction.Left : _movement.x > 0 ? Direction.Right : Look;
        }

        public enum Direction
        {
            Left = -1, Right = 1
        }

        public float GetHorizontalDelta()
        {
            // get velocity parameters
            var currVelocity = stickBody.velocity.x;
            var targetVelocity = _movement.x * speed;

            // calculate delta
            return Mathf.Min(targetVelocity - currVelocity, maxSpeed);
        }

        public float GetVerticalDelta()
        {
            // get velocity parameters
            var currVelocity = stickBody.velocity.y;
            var targetVelocity = _movement.y * verticalSpeed;
            
            // calculate delta
            return targetVelocity - currVelocity;
        }
    }
}