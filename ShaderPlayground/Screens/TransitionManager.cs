using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Helpers;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens
{
    public class TransitionManager
    {
        private GraphicsDevice _graphics;

        private ScreenManager.ScreenStates _capturedScreen;
        private RenderTarget2D _capturedRenderTarget2D;

        private ScreenManager _screenManager;

        private Effect _shader;

        private QuadRenderer _quadRenderer;

        public double Timer = 100;

        private double MaxTimer = 10;

        private bool startFlag = false;
        
        //

        private int lastTransition;
        private TransitionType currentTransitionType;
        private const int transitionAmount = 3;
        public enum TransitionType
        {
            Shear,
            Spiral, 
            Circle
        };
        private EffectPass _passShear;
        private EffectPass _passSpiral;
        private EffectPass _passCircle;

        public void Initialize(GraphicsDevice graphics, ScreenManager screenManager)
        {
            _graphics = graphics;
            _screenManager = screenManager;
            UpdateResolution();
            lastTransition = 1; //FastRand.NextInteger(0, transitionAmount-1);
        }

        public void Load(ContentManager content)
        {
            _shader = content.Load<Effect>("Shaders/Transition/TransitionShader");

            _passShear = _shader.Techniques["Shear"].Passes[0];
            _passSpiral = _shader.Techniques["Spiral"].Passes[0];
            _passCircle = _shader.Techniques["Circle"].Passes[0];
            _quadRenderer = new QuadRenderer();
        }

        public void Draw(GameTime gameTime)
        {
            if (startFlag)
            {
                //Capture the last screen!
                _screenManager.Draw(gameTime, _capturedRenderTarget2D, _capturedScreen);
                startFlag = false;
            }
            //Draw the transition

            if (Timer <= MaxTimer && Timer >= 0)
            {
                _graphics.SetRenderTarget(null);
                _graphics.BlendState = BlendState.NonPremultiplied;
                TransitionShader();
                _quadRenderer.RenderQuad(_graphics, -Vector2.One, Vector2.One);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Timer > MaxTimer) return;

            Timer += gameTime.ElapsedGameTime.TotalMilliseconds;


            _shader.Parameters["Timer"].SetValue((float) (Timer/MaxTimer));
        }

        private void TransitionShader()
        {
            switch (currentTransitionType)
            {
                case TransitionType.Shear:
                    _passShear.Apply();
                    break;
                case TransitionType.Spiral:
                    _passSpiral.Apply();
                    break;
                case TransitionType.Circle:
                    _passCircle.Apply();
                    break;
            }
        }


        public void Begin(double milliseconds, ScreenManager.ScreenStates captureScreen)
        {
            _capturedScreen = captureScreen;
            MaxTimer = milliseconds;
            Timer = 0;
            startFlag = true;

            int transition = lastTransition;

            while (transition == lastTransition)
            {
                transition = FastRand.NextInteger(0, transitionAmount - 1);
            }

            currentTransitionType = (TransitionType) transition;
            lastTransition = transition;
        }

        public void UpdateResolution()
        {
            if(_capturedRenderTarget2D != null) _capturedRenderTarget2D.Dispose();

            _capturedRenderTarget2D = new RenderTarget2D(_graphics, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight, false, SurfaceFormat.Color, DepthFormat.None);

            _shader.Parameters["Screen"].SetValue(_capturedRenderTarget2D);
            _shader.Parameters["AspectRatio"].SetValue((float)GameSettings.g_ScreenWidth / GameSettings.g_ScreenHeight);
        }
    }
}
