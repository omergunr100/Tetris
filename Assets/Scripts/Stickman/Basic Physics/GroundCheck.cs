using UnityEngine;

namespace Stickman.Basic_Physics
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask groundLayer;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if ((other.gameObject.layer & groundLayer) != 0)
            {
                StickController.Instance.Grounded = true;
            }
        }
    }
}