using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.HelperSuite.GUI;
using ShaderPlayground.Screens.Bokeh.ShaderModules;
using ShaderPlayground.Screens.RadialBlur.ShaderModules;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.RadialBlur
{
    public class BokehRenderer
    {
        private GraphicsDevice _graphics;

        public GuiTextBlockLoadDialog _backgroundTextureLoader;

        public SpriteBatch _spriteBatch;

        public BokehShader _bokehShader;

        public Texture2D DefaultTexture2D;

        private Texture2D _shapeStar;
        private Texture2D _shapePentagon;
        private Texture2D _shapeCircle;

        public float Brightness = 1;
        public float BokehSize = 4;
        public int Downsize = 1;

        public BokehShapes BokehShape = BokehShapes.Pentagon;

        public enum BokehShapes
        {
            Pentagon,
            Circle,
            Star
        };

        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;

            _bokehShader.Initialize(graphics);

            _spriteBatch = new SpriteBatch(graphics);
        }

        public void Load(ContentManager content)
        {
            _bokehShader = new BokehShader();
            _bokehShader.Load(content, "shaders/Bokeh/Bokeh");

            DefaultTexture2D = content.Load<Texture2D>("carred");

            _shapeStar = content.Load<Texture2D>("shaders/bokeh/ShapeSTar");
            _shapeCircle = content.Load<Texture2D>("shaders/bokeh/ShapeCircle");
            _shapePentagon = content.Load<Texture2D>("shaders/bokeh/ShapePentagon");
        }

        public void Draw(GameTime gameTime, RenderTarget2D outputRT)
        {
            Texture2D input = CheckRenderChanges();

            _graphics.Clear(Color.Black);

            //_spriteBatch.Begin();
            
            //_spriteBatch.Draw(input, new Rectangle(0, 0, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), Color.White);
            
            //_spriteBatch.End();

            _bokehShader.Draw(input, GetBokehTex(), outputRT, Brightness * 0.1f, BokehSize, 1 << Downsize);

        }

        private Texture2D GetBokehTex()
        {
            switch (BokehShape)
            {
                case BokehShapes.Pentagon:
                    return _shapePentagon;
                case BokehShapes.Circle:
                    return _shapeCircle;
                case BokehShapes.Star:
                    return _shapeStar;
            }

            //Should not be happening
            return _shapePentagon;
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
