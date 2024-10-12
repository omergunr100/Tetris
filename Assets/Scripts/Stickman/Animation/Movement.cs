using System.Collections;
using UnityEngine;

namespace Stickman.Animation
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private GameObject head;
        [SerializeField] private GameObject leftLeg;
        [SerializeField] private GameObject rightLeg;
        [SerializeField] private Animator animator;
        [SerializeField] private float waitTime = 0.5f;

        private Rigidbody2D _headRb;
        private Rigidbody2D _leftLegRb;
        private Rigidbody2D _rightLegRb;

        private SpriteRenderer _headSprite;
        private StickController.Direction _lookingAt;

        private void Start()
        {
            _headRb = head.GetComponent<Rigidbody2D>();
            _leftLegRb = leftLeg.GetComponent<Rigidbody2D>();
            _rightLegRb = rightLeg.GetComponent<Rigidbody2D>();
            _headSprite = head.GetComponent<SpriteRenderer>();
            _lookingAt = StickController.Direction.Right;
        }

        private void FixedUpdate()
        {
            if (StickController.Instance.Look != _lookingAt)
            {
                ChangeLookDirection();
                _lookingAt = StickController.Instance.Look;
            }
            if (StickController.Instance.MovingRight)
            {
                animator.Play("Walk Right");
                StartCoroutine(MoveRight(waitTime));
            }
            else if (StickController.Instance.MovingLeft)
            {
                animator.Play("Walk Left");
                StartCoroutine(MoveLeft(waitTime));
            }
            else if (!StickController.Instance.Moving)
            {
                animator.Play("Idle");
            }

            if (StickController.Instance.Jump && StickController.Instance.Grounded)
            {
                Debug.Log("Jumping");
                StartCoroutine(Jump());
            }
        }

        private IEnumerator MoveRight(float seconds)
        {
            var deltaV = StickController.Instance.GetHorizontalDelta() * (1000 * Time.deltaTime);
            _rightLegRb.AddForce(Vector2.right * deltaV);
            yield return new WaitForSeconds(seconds);
            _rightLegRb.AddForce(Vector2.right * deltaV);
        }

        private IEnumerator MoveLeft(float seconds)
        {
            var deltaV = StickController.Instance.GetHorizontalDelta() * (1000 * Time.deltaTime);
            _rightLegRb.AddForce(Vector2.right * deltaV);
            yield return new WaitForSeconds(seconds);
            _leftLegRb.AddForce(Vector2.right * deltaV);
        }

        private IEnumerator Jump()
        {
            var deltaV = Vector2.up * StickController.Instance.GetVerticalDelta();
            _headRb.velocity = deltaV;
            _leftLegRb.AddForce(deltaV);
            _rightLegRb.AddForce(deltaV);
            StickController.Instance.Grounded = false;
            yield return null;
        }

        private void ChangeLookDirection()
        {
            _headSprite.flipX = !_headSprite.flipX;
        }
    }
}