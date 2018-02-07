using System;
using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Screens.DefaultPreset.ShaderModules;
using ShaderPlayground.Screens.SubsurfaceScattering.ShaderModules;

namespace ShaderPlayground.Screens.DefaultPreset
{
    public class SubsurfaceRenderer
    {
        private GraphicsDevice _graphics;

        public GuiTextBlockLoadDialog _backgroundTextureLoader;

        public SpriteBatch _spriteBatch;

        public SubsurfaceScatteringShader _ssShader;
        private Texture2D DefaultTexture2D;


        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;

            _ssShader.Initialize(graphics);

            _spriteBatch = new SpriteBatch(graphics);
        }

        public void Load(ContentManager content)
        {
            _ssShader = new SubsurfaceScatteringShader();
            _ssShader.Load(content, "shaders/Subsurface/SubsurfaceScattering");
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
