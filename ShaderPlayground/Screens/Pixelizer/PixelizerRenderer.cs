using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Screens.Pixelizer.ShaderModules;
using ShaderPlayground.Screens.RadialBlur.ShaderModules;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.Pixelizer
{
    public class PixelizerRenderer
    {
        private GraphicsDevice _graphics;

        public GuiTextBlockLoadDialog _backgroundTextureLoader;

        public SpriteBatch _spriteBatch;

        public PixelizerShader _PixelizerShader;

        public RadialBlurShader _RadialBlurShader;
        
        public RenderTarget2D DefaultTexture2D;

        public float BlurIntensity = 0.01f;

        public int BlurSamples = 10;

        public int BlurPasses = 3;

        private bool _mouseUpdate = true;

        public Color LineColor = new Color(0,255,0,1);

        public float SplitChance = 0.12f;
        public float EndChance = 0.06f;
        public bool Random = true;
        public int Scale = 0;

        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;

            _PixelizerShader.Initialize(graphics);
            _RadialBlurShader.Initialize(graphics);
            _spriteBatch = new SpriteBatch(graphics);

            DefaultTexture2D = new RenderTarget2D(graphics, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        }

        public void Load(ContentManager content)
        {
            _PixelizerShader = new PixelizerShader();
            _PixelizerShader.Load(content, "shaders/Pixelizer/Pixelizer");

            _RadialBlurShader = new RadialBlurShader();
            _RadialBlurShader.Load(content, "shaders/RadialBlur/RadialBlur");
            
            
        }

        public void Draw(GameTime gameTime, RenderTarget2D outputRT)
        {
            _graphics.Clear(Color.Black);

            if (Input.IsRMBPressed())
                _RadialBlurShader.MousePosition = Input.GetMousePositionNormalized();

            _PixelizerShader.Draw(DefaultTexture2D, LineColor, SplitChance, 1 - EndChance, Random, gameTime, 1 << Scale);

            _RadialBlurShader.Draw(DefaultTexture2D, outputRT, BlurSamples, BlurIntensity, BlurPasses);
        }
        
    }
}
