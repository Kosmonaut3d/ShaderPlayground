using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShaderPlayground.Screens.RadialBlur
{
    public class RadialBlurLogic
    {
        private RadialBlurGUILogic guiLogic;

        public void Initialize(ScreenManager screenManager, RadialBlurRenderer renderer)
        {
            
            guiLogic.Initialize(screenManager, renderer);
        }

        public void Update(GameTime gameTime)
        {
            guiLogic.Update(gameTime);
        }

        public GUICanvas GetCanvas()
        {
            return guiLogic.getCanvas();
        }

        public void Load(ContentManager content)
        {
            guiLogic = new RadialBlurGUILogic();
            guiLogic.Load(content);
        }
    }
}
