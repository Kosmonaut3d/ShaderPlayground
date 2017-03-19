using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ShaderPlayground.HelperSuite.GUI;

namespace ShaderPlayground.Screens.ParticlePhysics
{
    public class ParticlePhysicsLogic
    {
        private ParticlePhysicsGUIRenderer _guiGuiRenderer;

        public void Initialize(ScreenManager screenManager, ParticlePhysicsRenderer renderer)
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
            _guiGuiRenderer = new ParticlePhysicsGUIRenderer();
            _guiGuiRenderer.Load(content);
        }
    }
}
