using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShaderPlayground.Screens.Pixelizer
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
