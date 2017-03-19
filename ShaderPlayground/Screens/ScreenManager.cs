using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShaderPlayground.Controls;
using ShaderPlayground.Helpers;
using ShaderPlayground.HelperSuite.GUIRenderer;
using ShaderPlayground.Screens.Bokeh;
using ShaderPlayground.Screens.Debug;
using ShaderPlayground.Screens.MainMenu;
using ShaderPlayground.Screens.ParticlePhysics;
using ShaderPlayground.Screens.RadialBlur;

namespace ShaderPlayground.Screens
{
    public class ScreenManager
    {
        private GraphicsDevice _graphics;

        //dbg
        private DebugScreen _debugScreen;

        //GUI
        private GUIRenderer _guiRenderer;

        //Main menu
        private MainMenuLogic _mainMenuLogic;

        //Editor
        private RadialBlurLogic _radialBlurLogic;
        private RadialBlurRenderer _radialBlurRenderer;

        private PixelizerLogic _pixelizerLogic;
        private PixelizerRenderer _pixelizerRenderer;

        private BokehLogic _bokehLogic;
        private BokehRenderer _bokehRenderer;

        private ParticlePhysicsLogic _particlePhysicsLogic;
        private ParticlePhysicsRenderer _particlePhysicsRenderer;

        ///////

        private MeshLoader _meshLoader;

        //Transitions
        private TransitionManager _transitionManager;

        private ScreenStates _currentState;
        public ScreenStates NextState;

        public enum ScreenStates
        {
            MainMenu,
            RadialBlur,
            Pixelizer,
            Exit,
            Bokeh,
            ParticlePhysics
        }

        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;

            _debugScreen.Initialize(graphics);

            _currentState = ScreenStates.MainMenu;
            NextState = ScreenStates.MainMenu;

            _guiRenderer.Initialize(graphics);

            _mainMenuLogic.Initialize(this);

            _radialBlurLogic.Initialize(this, _radialBlurRenderer);
            _radialBlurRenderer.Initialize(graphics);

            _pixelizerLogic.Initialize(this, _pixelizerRenderer);
            _pixelizerRenderer.Initialize(graphics);

            _bokehLogic.Initialize(this, _bokehRenderer);
            _bokehRenderer.Initialize(graphics);

            _particlePhysicsLogic.Initialize(this, _particlePhysicsRenderer);
            _particlePhysicsRenderer.Initialize(graphics);

            _transitionManager.Initialize(graphics, this);

        }

        public void Load(ContentManager content)
        {
            _debugScreen = new DebugScreen();
            _debugScreen.LoadContent(content);

            _guiRenderer = new GUIRenderer();
            _guiRenderer.Load(content);

            _mainMenuLogic = new MainMenuLogic();
            _mainMenuLogic.Load(content);

            _radialBlurLogic = new RadialBlurLogic();
            _radialBlurLogic.Load(content);
            
            _radialBlurRenderer = new RadialBlurRenderer();
            _radialBlurRenderer.Load(content);

            _pixelizerLogic = new PixelizerLogic();
            _pixelizerLogic.Load(content);
            _pixelizerRenderer = new PixelizerRenderer();
            _pixelizerRenderer.Load(content);

            _bokehLogic = new BokehLogic();
            _bokehLogic.Load(content);
            _bokehRenderer = new BokehRenderer();
            _bokehRenderer.Load(content);

            _particlePhysicsLogic = new ParticlePhysicsLogic();
            _particlePhysicsLogic.Load(content);
            _particlePhysicsRenderer = new ParticlePhysicsRenderer();
            _particlePhysicsRenderer.Load(content);

            _transitionManager = new TransitionManager();
            _transitionManager.Load(content);

            _meshLoader = new MeshLoader();
            _meshLoader.Load(content);
        }

        public void Update(GameTime gameTime)
        {
            //Switch States!
            if (_currentState != NextState)
            {
                _transitionManager.Begin(800, _currentState);
                _currentState = NextState;
            }
           
            _transitionManager.Update(gameTime);
            
            Input.Update(gameTime);
            switch (_currentState)
            {
                case ScreenStates.MainMenu:
                    {
                        _mainMenuLogic.Update(gameTime);
                        break;
                    }
                case ScreenStates.RadialBlur:
                    {
                        _radialBlurLogic.Update(gameTime);
                        break;
                    }
                case ScreenStates.Pixelizer:
                    {
                        _pixelizerLogic.Update(gameTime);
                        break;
                    }
                case ScreenStates.Bokeh:
                    {
                        _bokehLogic.Update(gameTime);
                        break;
                    }
                case ScreenStates.ParticlePhysics:
                    {
                        _particlePhysicsLogic.Update(gameTime);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }

           _debugScreen.Update(gameTime);

            //Reload
        }

        public void Draw(GameTime gameTime)
        {
            //Default
            Draw(gameTime, null, _currentState);
            
            _transitionManager.Draw(gameTime);

            _debugScreen.Draw(gameTime);
        }

        public void Draw(GameTime gameTime, RenderTarget2D renderTarget, ScreenStates screenState)
        {
            _graphics.SetRenderTarget(renderTarget);
            switch (screenState)
            {
                case ScreenStates.MainMenu:
                    {
                        _graphics.Clear(Color.CornflowerBlue);
                        _guiRenderer.Draw(_mainMenuLogic.getCanvas());
                        break;
                    }
                case ScreenStates.RadialBlur:
                    {
                        _radialBlurRenderer.Draw(gameTime, renderTarget);
                        _guiRenderer.Draw(_radialBlurLogic.GetCanvas());
                        break;
                    }
                case ScreenStates.Pixelizer:
                    {
                        _pixelizerRenderer.Draw(gameTime, renderTarget);
                        _guiRenderer.Draw(_pixelizerLogic.GetCanvas());
                        break;
                    }
                case ScreenStates.Bokeh:
                    {
                        _bokehRenderer.Draw(gameTime, renderTarget);
                        _guiRenderer.Draw(_bokehLogic.GetCanvas());
                        break;
                    }
                case ScreenStates.ParticlePhysics:
                    {
                        _particlePhysicsRenderer.Draw(gameTime, renderTarget);
                        _guiRenderer.Draw(_particlePhysicsLogic.GetCanvas());
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }


        }

        public void UpdateResolution()
        {
            _mainMenuLogic.UpdateResolution();
        }
    }
}
