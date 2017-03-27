using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShaderPlayground.Screens.DefaultPreset
{
    public class DefaultLogic
    {
        private DefaultGUIRenderer _guiGuiRenderer;

        public void Initialize(ScreenManager screenManager, DefaultRenderer renderer)
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
            _guiGuiRenderer = new DefaultGUIRenderer();
            _guiGuiRenderer.Load(content);
        }
    }
}
