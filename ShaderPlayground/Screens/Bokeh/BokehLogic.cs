using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ShaderPlayground.Screens.Bokeh
{
    public class BokehLogic
    {
        private BokehGUILogic guiLogic;

        public void Initialize(ScreenManager screenManager, BokehRenderer renderer)
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
            guiLogic = new BokehGUILogic();
            guiLogic.Load(content);
        }
    }
}
