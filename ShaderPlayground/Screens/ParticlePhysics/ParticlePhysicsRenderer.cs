using System;
using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Screens.Bokeh.ShaderModules;
using ShaderPlayground.Screens.ParticlePhysics.ShaderModules;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.ParticlePhysics
{
    public class ParticlePhysicsRenderer
    {
        private GraphicsDevice _graphics;

        public GuiTextBlockLoadDialog _backgroundTextureLoader;

        public SpriteBatch _spriteBatch;

        public ParticlePhysicsShader _ParticlePhysicsShader;
        private BokehShader _bokehShader;
        private Texture2D DefaultTexture2D;
        private Texture2D _shapePentagon;
        private RenderTarget2D _renderTarget0;
        
        public float TestFloat = 1;
        public int TestInt = 10;
        public float SpringConstant = 0.02f;
        public float DampeningConstant = 0.98f;

        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;

            _ParticlePhysicsShader.Initialize(graphics);
            _bokehShader.Initialize(graphics);

            _spriteBatch = new SpriteBatch(graphics);

            CreateRenderTargets();
        }

        public void Load(ContentManager content)
        {
            _ParticlePhysicsShader = new ParticlePhysicsShader();
            _ParticlePhysicsShader.Load(content, "shaders/ParticlePhysics/ParticlePhysics");

            _bokehShader = new BokehShader();
            _bokehShader.Load(content, "Shaders/bokeh/bokeh");
            
            _shapePentagon = content.Load<Texture2D>("shaders/bokeh/ShapePentagon");
            DefaultTexture2D = content.Load<Texture2D>("carred");


        }

        public void Draw(GameTime gameTime, RenderTarget2D outputRT)
        {
            Texture2D input = CheckRenderChanges();

            _graphics.Clear(Color.Black);

            

            _ParticlePhysicsShader.Draw(outputRT, input, Input.GetMousePosition().ToVector2(), TestInt, SpringConstant, DampeningConstant);
           
            //_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointWrap);

            ////_spriteBatch.Draw(_ParticlePhysicsShader.GetTexBuffer(), new Rectangle(0, 0, 20, 20), Color.White);
            //_spriteBatch.Draw(_renderTarget0, new Rectangle(0, 0, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), Color.White);

            //_spriteBatch.End();

            //Dynamic
        }

        private void CreateRenderTargets()
        {
            _renderTarget0 = new RenderTarget2D(_graphics, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

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
