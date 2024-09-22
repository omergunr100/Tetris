using Tetromino;

namespace Effects.Definition
{
    public static class EffectHelper
    {
        public static bool IsStranger(this BlockScript block, BlockScript neighbor)
        {
            return neighbor != null && block.Id != neighbor.Id;
        }
    }
}