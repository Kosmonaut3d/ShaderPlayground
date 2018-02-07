using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShaderPlayground.Screens.FourierTransform
{
    public class FTLogic
    {
        private FTGuiRenderer _guiGuiRenderer;

        public void Initialize(ScreenManager screenManager, FTRenderer renderer)
        {
            
            _guiGuiRenderer.Initialize(screenManager, renderer);
        }

        public void Update(GameTime gameTime)
        {
            _guiGuiRenderer.Update(gameTime);
        }

        public GUICanvas GetCanvas()
        {
            return _guiGuiRenderer.getCanvas();
        }

        public void Load(ContentManager content)
        {
            _guiGuiRenderer = new FTGuiRenderer();
            _guiGuiRenderer.Load(content);
        }
    }
}
