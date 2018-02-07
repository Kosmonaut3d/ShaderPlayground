using System;
using HelperSuite.GUIHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShaderPlayground.Screens;
using ShaderPlayground.Settings;

namespace ShaderPlayground
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ShaderPlayground : Game
    {
        GraphicsDeviceManager graphics;

        private ScreenManager _screenManager;
        private bool _isActive = true;

        private bool _fixedFrameRate = true;

        public ShaderPlayground()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.PreferredBackBufferWidth = GameSettings.g_ScreenWidth;
            graphics.PreferredBackBufferHeight = GameSettings.g_ScreenHeight;

            graphics.PreferMultiSampling = false;
            
            //graphics.SynchronizeWithVerticalRetrace = false;
            //IsFixedTimeStep = false;
            //TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f/ 200);

            IsMouseVisible = true;

            //Update framerate etc. when not the active window
            Activated += IsActivated;
            Deactivated += IsDeactivated;

            _screenManager = new ScreenManager();
        }


        private void IsActivated(object sender, EventArgs e)
        {
            _isActive = true;
        }

        private void IsDeactivated(object sender, EventArgs e)
        {
            _isActive = false;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        //protected override void Initialize()
        //{
        //    // TODO: Add your initialization logic here

        //    base.Initialize();
        //}

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            GUIControl.Initialize(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight);
            _screenManager.Load(Content);
            _screenManager.Initialize(GraphicsDevice);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || GameStats.NextState == ScreenManager.ScreenStates.Exit)
                Exit();

            _screenManager.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!IsActive) return;

            CheckChanges();

            _screenManager.Draw(gameTime);
        }

        private void CheckChanges()
        {
            if (GameSettings.g_fixFPS != _fixedFrameRate)
            {
                _fixedFrameRate = GameSettings.g_fixFPS;

                if (!_fixedFrameRate)
                {
                    graphics.SynchronizeWithVerticalRetrace = false;
                    IsFixedTimeStep = false;
                }
                else
                {
                    graphics.SynchronizeWithVerticalRetrace = true;
                    IsFixedTimeStep = true;
                }
                graphics.ApplyChanges();
            }
        }
    }
}
