using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.HelperSuite.GUI;
using ShaderPlayground.Screens.DefaultPreset.ShaderModules;

namespace ShaderPlayground.Screens.DefaultPreset
{
    public class DefaultRenderer
    {
        private GraphicsDevice _graphics;

        public GuiTextBlockLoadDialog _backgroundTextureLoader;

        public SpriteBatch _spriteBatch;

        public DefaultShader _defaultShader;
        private Texture2D DefaultTexture2D;


        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;

            _defaultShader.Initialize(graphics);

            _spriteBatch = new SpriteBatch(graphics);
        }

        public void Load(ContentManager content)
        {
            _defaultShader = new DefaultShader();
            _defaultShader.Load(content, "shaders/Bokeh/Bokeh");
        }

        public void Draw(GameTime gameTime, RenderTarget2D outputRT)
        {
            Texture2D input = CheckRenderChanges();

            _graphics.Clear(Color.Black);

            //_spriteBatch.Begin();
            
            //_spriteBatch.Draw(input, new Rectangle(0, 0, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), Color.White);
            
            //_spriteBatch.End();

            //Dynamic
        }
        

        private Texture2D CheckRenderChanges()
        {
            //What image are we using?
            if (_backgroundTextureLoader.LoadedObject != null)
            {
                return _backgroundTextureLoader.LoadedObject as Texture2D;
            }
            else
            {
                return DefaultTexture2D;
            }
        }
    }
}
