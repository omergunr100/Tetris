using UnityEngine;

namespace Stickman.Basic_Physics
{
    public class BalanceRigidbody2D : MonoBehaviour
    {
        [SerializeField] private float targetRotation;
        [SerializeField] private float force;

        private Rigidbody2D _rb;
        
        private void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _rb.MoveRotation(Mathf.LerpAngle(_rb.rotation, targetRotation, force * 10 * Time.deltaTime));
        }
    }
}