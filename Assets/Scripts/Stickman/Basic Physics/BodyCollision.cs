using Management;
using UnityEngine;

namespace Stickman.Basic_Physics
{
    public class BodyCollision : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if ((other.gameObject.layer & LayerMask.NameToLayer("Tetromino")) != 0)
            {
                GameManager.Instance.SetGamePhase(GamePhase.Loss);
            }
        }
    }
}