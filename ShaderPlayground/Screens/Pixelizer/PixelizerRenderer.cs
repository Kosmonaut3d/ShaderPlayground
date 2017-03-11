using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.HelperSuite.GUI;
using ShaderPlayground.Screens.RadialBlur.ShaderModules;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.RadialBlur
{
    public class PixelizerRenderer
    {
        private GraphicsDevice _graphics;

        public GuiTextBlockLoadDialog _backgroundTextureLoader;

        public SpriteBatch _spriteBatch;

        public PixelizerShader _PixelizerShader;
        
        public Texture2D DefaultTexture2D;

        public float BlurIntensity = 0.2f;

        public int BlurSamples = 10;

        public int BlurPasses = 3;

        private bool _mouseUpdate = true;

        public Color LineColor = Color.Green;

        public float SplitChance = 0.12f;
        public float EndChance = 0.06f;
        public bool Random = true;

        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;

            _PixelizerShader.Initialize(graphics);

            _spriteBatch = new SpriteBatch(graphics);
        }

        public void Load(ContentManager content)
        {
            _PixelizerShader = new PixelizerShader();
            _PixelizerShader.Load(content, "shaders/Pixelizer/Pixelizer");

            DefaultTexture2D = content.Load<Texture2D>("carred");
        }

        public void Draw(GameTime gameTime, RenderTarget2D outputRT)
        {
            _graphics.Clear(Color.Black);
            
            _PixelizerShader.Draw(outputRT, LineColor, SplitChance, 1 - EndChance, Random, gameTime);

        }
        
    }
}
