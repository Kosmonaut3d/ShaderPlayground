using System;
using System.Threading.Tasks;
using HelperSuite.ContentLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Helpers;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.DefaultPreset.ShaderModules
{
    public class DefaultShader
    {
        private GraphicsDevice _graphicsDevice;
        private ThreadSafeContentManager _contentManager;

        private bool _effectLoaded = false;
        
        //Shader Effect
        private Effect shaderEffect;
        private Effect shaderEffectLoad;
        private Task loadTaskOut;
        
        //Parameters
        private EffectParameter _defaultParameter;

        private EffectPass _defaultPass;

        public void Initialize(GraphicsDevice graphics)
        {
            _graphicsDevice = graphics;
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

        public void Draw(RenderTarget2D rt)
        {
            InitializeParameters();
            
            if (!_effectLoaded) return;
            
            _graphicsDevice.SetRenderTarget(rt);
            //1280 800

            _defaultPass.Apply();
        }

        private void CreateRT(int scale, bool fullPrecision)
        {
            //if (_renderTarget0 != null)
            //{
            //    _renderTarget0.Dispose();
            //}

            //int w = GameSettings.g_ScreenWidth / scale;
            //int h = GameSettings.g_ScreenHeight / scale;

            //_renderTarget0 = new RenderTarget2D(_graphicsDevice, w, h, false, fullPrecision ?  SurfaceFormat.Vector4 : SurfaceFormat.HalfVector4, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            
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

            _defaultParameter = shaderEffect.Parameters["Default"];

            _defaultPass = shaderEffect.Techniques["Default"].Passes[0];

            //Signal we have loaded the effect
            _effectLoaded = true;

        }
        
    }
    
}
