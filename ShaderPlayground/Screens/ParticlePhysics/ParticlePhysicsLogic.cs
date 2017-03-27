using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShaderPlayground.Screens.ParticlePhysics
{
    public class ParticlePhysicsLogic
    {
        private ParticlePhysicsGUILogic _guiGuiLogic;

        public void Initialize(ScreenManager screenManager, ParticlePhysicsRenderer renderer)
        {
            
            _guiGuiLogic.Initialize(screenManager, renderer);
        }

        public void Update(GameTime gameTime)
        {
            _guiGuiLogic.Update(gameTime);
        }

        public GUICanvas GetCanvas()
        {
            return _guiGuiLogic.getCanvas();
        }

        public void Load(ContentManager content)
        {
            _guiGuiLogic = new ParticlePhysicsGUILogic();
            _guiGuiLogic.Load(content);
        }
    }
}
