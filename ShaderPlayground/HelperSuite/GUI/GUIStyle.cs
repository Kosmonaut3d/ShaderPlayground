using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShaderPlayground.HelperSuite.GUI
{
    public class GUIStyle
    {
        public Color BlockColor;
        public Color TextColor;
        public SpriteFont TextFont;

        public enum GUIAlignment
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Center
        }

        public enum TextAlignment
        {
            Left, Center, Right
        }
    }
}
