using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.HelperSuite.GUI;
using ShaderPlayground.Screens.Bokeh.ShaderModules;

namespace ShaderPlayground.Screens.Bokeh
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
        private Texture2D _shapeHexagon;

        public float Brightness = 1;
        public float BokehSize = 4;
        public int Downsize = 1;

        public BokehShapes BokehShape = BokehShapes.Pentagon;
        public bool FullPrecision = false;
        public bool DynamicScaling = true;
        public int PolyCount = 1;

        public enum BokehShapes
        {
            Pentagon,
            Hexagon,
            Circle,
            Star,
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
            _shapeHexagon = content.Load<Texture2D>("shaders/bokeh/ShapeHexagon");
        }

        public void Draw(GameTime gameTime, RenderTarget2D outputRT)
        {
            Texture2D input = CheckRenderChanges();

            _graphics.Clear(Color.Black);

            //_spriteBatch.Begin();
            
            //_spriteBatch.Draw(input, new Rectangle(0, 0, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), Color.White);
            
            //_spriteBatch.End();

            //Dynamic
            
            _bokehShader.Draw(input, GetBokehTex(), outputRT, Brightness , BokehSize,
                    1 << Downsize, FullPrecision, DynamicScaling);
            

            PolyCount = _bokehShader.PrimitiveCount;
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
                case BokehShapes.Hexagon:
                    return _shapeHexagon;
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
