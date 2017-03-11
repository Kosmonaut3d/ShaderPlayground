using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ShaderPlayground.HelperSuite.GUI;

namespace ShaderPlayground.Screens.RadialBlur
{
    public class PixelizerLogic
    {
        private PixelizerGUILogic guiLogic;

        public void Initialize(ScreenManager screenManager, PixelizerRenderer renderer)
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
            guiLogic = new PixelizerGUILogic();
            guiLogic.Load(content);
        }
    }
}
