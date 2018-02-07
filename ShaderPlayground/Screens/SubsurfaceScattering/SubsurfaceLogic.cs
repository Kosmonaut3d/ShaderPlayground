using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShaderPlayground.Screens.DefaultPreset
{
    public class SubsurfaceLogic
    {
        private SubsurfaceGUIRenderer _guiRenderer;

        public void Initialize(ScreenManager screenManager, SubsurfaceRenderer renderer)
        {
            
            _guiRenderer.Initialize(screenManager, renderer);
        }

        public void Update(GameTime gameTime)
        {
            _guiRenderer.Update(gameTime);
        }

        public GUICanvas GetCanvas()
        {
            return _guiRenderer.getCanvas();
        }

        public void Load(ContentManager content)
        {
            _guiRenderer = new SubsurfaceGUIRenderer();
            _guiRenderer.Load(content);
        }
    }
}
