using System;
using System.Threading.Tasks;
using HelperSuite.ContentLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Helpers;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.Pixelizer.ShaderModules
{
    public class PixelizerShader
    {
        private GraphicsDevice _graphicsDevice;
        private ThreadSafeContentManager _contentManager;

        private bool _effectLoaded = false;

        private QuadRenderer _quadRenderer;

        private RenderTarget2D _renderTarget0;
        private RenderTarget2D _renderTarget1;

        //Shader Effect
        private Effect shaderEffect;
        private Effect shaderEffectLoad;
        private Task loadTaskOut;

        //Parameters
        private EffectParameter _screenParameter;
        private EffectParameter _resolutionParameter;
        private EffectParameter _mousePositionParameter;
        private EffectParameter _lineColorParameter;
        private EffectParameter _splitChanceParameter;
        private EffectParameter _endChanceParameter;
        private EffectParameter _timeParameter;
        
        private EffectPass _texturePass;
        private EffectPass _animatePass;

        private Texture2D _screenTexture;

        private bool evenFrame = false;

        private Color _lineColor;

        private float _splitChance;
        private float _endChance;
        private float _time;
        
        public Texture2D ScreenTexture
        {
            get
            {
                return _screenTexture;
            }

            set
            {
                if (value != _screenTexture)
                {
                    _screenTexture = value;

                    _screenParameter?.SetValue(_screenTexture);
                }
            }
        }
        
        public Color LineColor
        {
            set
            {
                if (value != _lineColor)
                {
                    _lineColor = value;
                    _lineColorParameter.SetValue(_lineColor.ToVector3());
                }
            }
            get { return _lineColor; }
        }

        public float SplitChance
        {
            get
            {
                return _splitChance;
            }

            set
            {
                if (Math.Abs(_splitChance - value) > 0.00001f)
                {
                    _splitChance = value;
                    _splitChanceParameter.SetValue(_splitChance);
                }
            }
        }
        public float EndChance
        {
            get
            {
                return _endChance;
            }

            set
            {
                if (Math.Abs(_endChance - value) > 0.00001f)
                {
                    _endChance = value;
                    _endChanceParameter.SetValue(_endChance);
                }
            }
        }

        public float Time
        {
            get
            {
                return _time;
            }

            set
            {
                if (Math.Abs(_time - value) > 0.00001f)
                {
                    _time = value;
                    _timeParameter.SetValue(_time);
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////
        /// Functions

        /// <summary>
        /// Initialize quad renderer
        /// </summary>
        /// <param name="graphics"></param>

        public void Initialize(GraphicsDevice graphics)
        {
            _graphicsDevice = graphics;
            _quadRenderer = new QuadRenderer();
        }
        
        public void Load(ContentManager content, string path)
        {
            //Hot swapping
            _contentManager = new ThreadSafeContentManager(content.ServiceProvider) { RootDirectory = "Content" };

            ReloadShader(path);
        }

        private void ReloadShader(string path)
        {
            //If already have one -> dispose
            shaderEffectLoad?.Dispose();

            _effectLoaded = false;
            loadTaskOut = Task.Factory.StartNew(() =>
            {
                shaderEffectLoad = _contentManager.Load<Effect>(path);
            });
        }
        

        public void Draw(RenderTarget2D target, Color lineColor, float splitChance, float endChance, bool random, GameTime gameTime, int scale)
        {
            //Check for changes
            InitializeParameters();

            if(scale<1 || scale > 8)
                throw new NotImplementedException("Scale values from 1 to 8 are supported");

            if(_renderTarget1==null || _renderTarget0.Width*scale != GameSettings.g_ScreenWidth || _renderTarget0.Height * scale != GameSettings.g_ScreenHeight)
                CreateRT(scale);

            //Don't draw if the effect is not loaded yet
            if (!_effectLoaded) return;

            //If mouse was clicked draw!
            if (Input.IsLMBPressed() && !GameStats.UIWasUsed)
            {
                _mousePositionParameter.SetValue(new Vector2( Input.GetMousePosition().X / scale, Input.GetMousePosition().Y / scale));
            }
            else
            {
                _mousePositionParameter.SetValue(-Vector2.One);
            }

            LineColor = lineColor;
            EndChance = endChance;
            SplitChance = splitChance;

            if (random)
            {
                Time = (float) gameTime.TotalGameTime.TotalMilliseconds;
            }

            RenderTarget2D t0 = evenFrame ? _renderTarget0 : _renderTarget1;
            RenderTarget2D t1 = evenFrame ? _renderTarget1 : _renderTarget0;

            _graphicsDevice.BlendState = BlendState.Opaque;

            DrawPass(t0, t1, _animatePass );
            DrawPass(t1, target, _texturePass);
            
            evenFrame = !evenFrame;
        }

        public void DrawPass(Texture2D tex, RenderTarget2D target, EffectPass pass)
        {
            _graphicsDevice.SetRenderTarget(target);

            ScreenTexture = tex;

            pass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);
        }

        public void CreateRT(int scale)
        {
            if (_renderTarget1 != null)
            {
                _renderTarget1.Dispose();
                _renderTarget0.Dispose();
            }

            int w = GameSettings.g_ScreenWidth / scale;
            int h = GameSettings.g_ScreenHeight / scale;

            _renderTarget0 = new RenderTarget2D(_graphicsDevice, w,h, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _renderTarget1 = new RenderTarget2D(_graphicsDevice, w,h, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            _resolutionParameter.SetValue(new Vector2(w,h));

            //Clear to black

            _graphicsDevice.SetRenderTarget(_renderTarget0);
            _graphicsDevice.Clear(Color.TransparentBlack);

            _graphicsDevice.SetRenderTarget(_renderTarget1);
            _graphicsDevice.Clear(Color.TransparentBlack);

        }

        private void InitializeParameters()
        {
            if (_effectLoaded) return;

            if (loadTaskOut == null) return;

            //Is still loading
            if (loadTaskOut.Status == TaskStatus.Running) return;

            //Has finished loading -> initialize parameters

            //Swap shader effects
            shaderEffect?.Dispose();
            shaderEffect = shaderEffectLoad;

            _screenParameter = shaderEffect.Parameters["Screen"];
            _resolutionParameter = shaderEffect.Parameters["Resolution"];
            _mousePositionParameter = shaderEffect.Parameters["MousePosition"];
            _lineColorParameter = shaderEffect.Parameters["LineColor"];
            _splitChanceParameter = shaderEffect.Parameters["SplitChance"];
            _endChanceParameter = shaderEffect.Parameters["EndChance"];
            _timeParameter = shaderEffect.Parameters["Time"];

            _texturePass = shaderEffect.Techniques["Texture"].Passes[0];
            _animatePass = shaderEffect.Techniques["Animate"].Passes[0];

            //Signal we have loaded the effect
            _effectLoaded = true;
        }
        
    }
}
