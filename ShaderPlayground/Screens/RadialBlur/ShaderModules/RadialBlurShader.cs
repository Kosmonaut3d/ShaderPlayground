using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Helpers;
using ShaderPlayground.HelperSuite.ContentLoader;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.RadialBlur.ShaderModules
{
    public class RadialBlurShader
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
        private EffectParameter _mouseParameter;
        private EffectParameter _blurIntensityParameter;
        private EffectParameter _blurSamplesParameter;

        private EffectPass _blurPass;
        private EffectPass _texPass;

        private Texture2D _screenTexture;

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

        private float _blurIntensity = 0.2f;
        public float BlurIntensity
        {
            get { return _blurIntensity; }
            set
            {
                _blurIntensity = value; 
                _blurIntensityParameter.SetValue(_blurIntensity);
            }
        }

        private int _blurSamples = 10;
        public int BlurSamples
        {
            get { return _blurSamples; }
            set
            {
                _blurSamples = value;
                _blurSamplesParameter.SetValue((float)_blurSamples);
            }
        }

        public Vector2 MousePosition
        {
            set
            {
                _mouseParameter?.SetValue(value);
            }
        }

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

        public void DrawTexture(RenderTarget2D tex, RenderTarget2D rt)
        {
            _graphicsDevice.SetRenderTarget(rt);
            ScreenTexture = tex;
            _texPass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);
        }


        public void Draw(Texture2D tex, RenderTarget2D target, int sampleCount, float intensity, int passes)
        {
            //Check for changes
            InitializeParameters();

            if(_renderTarget1==null || _renderTarget1.Width!=GameSettings.g_ScreenWidth || _renderTarget1.Height != GameSettings.g_ScreenHeight)
                CreateRT();

            //Don't draw if the effect is not loaded yet
            if (!_effectLoaded) return;

            //First Pass

            ScreenTexture = tex;

            BlurSamples = sampleCount;

            _graphicsDevice.SetRenderTarget(_renderTarget0);

            BlurIntensity = intensity;

            _blurPass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);

            if (passes <= 1)
            {
                DrawTexture(_renderTarget0, target);
                return;
            }

            //Second pass
            
            _graphicsDevice.SetRenderTarget(_renderTarget1);

            ScreenTexture = _renderTarget0;

            BlurIntensity = intensity/sampleCount*2;
            
            _blurPass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);

            if (passes <= 2)
            {
                DrawTexture(_renderTarget1, target);
                return;
            }

            //Third pass

            _graphicsDevice.SetRenderTarget(_renderTarget0);

            ScreenTexture = _renderTarget1;

            BlurIntensity = intensity/sampleCount;

            _blurPass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);

            DrawTexture(_renderTarget0, target);

        }

        public void CreateRT()
        {
            if (_renderTarget1 != null)
            {
                _renderTarget1.Dispose();
                _renderTarget0.Dispose();
            }

            _renderTarget0 = new RenderTarget2D(_graphicsDevice, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            _renderTarget1 = new RenderTarget2D(_graphicsDevice, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
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
            _mouseParameter = shaderEffect.Parameters["MousePosition"];
            _blurIntensityParameter = shaderEffect.Parameters["BlurIntensity"];
            _blurSamplesParameter = shaderEffect.Parameters["BlurSamples"];

            _blurPass = shaderEffect.Techniques["Blur"].Passes[0];
            _texPass = shaderEffect.Techniques["Texture"].Passes[0];

            //Signal we have loaded the effect
            _effectLoaded = true;
        }
        
    }
}
