using UnityEngine;

namespace Tetromino
{
    public class BlockScript : MonoBehaviour
    {
        public bool IsOnBoard { get; set; } = false;
        public (int, int) BoardLocation { get; set; }
        public long Id { get; set; }

        public void SetColor(Color color) => GetComponent<SpriteRenderer>().color = color;
        public Color GetColor() => GetComponent<SpriteRenderer>().color;
    }
}