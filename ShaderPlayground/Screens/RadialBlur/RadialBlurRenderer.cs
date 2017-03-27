using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Screens.RadialBlur.ShaderModules;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.RadialBlur
{
    public class RadialBlurRenderer
    {
        private GraphicsDevice _graphics;

        public GuiTextBlockLoadDialog _backgroundTextureLoader;

        public SpriteBatch _spriteBatch;

        public RadialBlurShader _RadialBlurShader;

        public Texture2D DefaultTexture2D;

        public float BlurIntensity = 0.2f;

        public int BlurSamples = 10;

        public int BlurPasses = 3;

        private bool _mouseUpdate = true;

        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;

            _RadialBlurShader.Initialize(graphics);

            _spriteBatch = new SpriteBatch(graphics);
        }

        public void Load(ContentManager content)
        {
            _RadialBlurShader = new RadialBlurShader();
            _RadialBlurShader.Load(content, "shaders/RadialBlur/RadialBlur");

            DefaultTexture2D = content.Load<Texture2D>("carred");
        }

        public void Draw(GameTime gameTime, RenderTarget2D outputRT)
        {
            Texture2D input = CheckRenderChanges();

            _graphics.Clear(Color.Black);

            //_spriteBatch.Begin();

            //if (_backgroundTextureLoader.LoadedObject != null)
            //{
            //    Texture2D tex = _backgroundTextureLoader.LoadedObject as Texture2D;

            //    _spriteBatch.Draw(tex, new Rectangle(0,0, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), Color.White);
            //}

            //_spriteBatch.End();

            _RadialBlurShader.Draw(input, outputRT, BlurSamples, BlurIntensity, BlurPasses);

        }

        private Texture2D CheckRenderChanges()
        {
            

            //This should be in logic, but who cares
            if (Input.WasLMBClicked() && !GameStats.UIWasUsed) _mouseUpdate = !_mouseUpdate;

            if(_mouseUpdate)
            _RadialBlurShader.MousePosition = Input.GetMousePositionNormalized();

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
