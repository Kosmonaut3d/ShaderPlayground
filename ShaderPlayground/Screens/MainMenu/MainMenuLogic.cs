using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ShaderPlayground.Controls;
using ShaderPlayground.HelperSuite.GUI;
using ShaderPlayground.HelperSuite.GUIRenderer;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.MainMenu
{
    public class MainMenuLogic
    {
        private ScreenManager _screenManager;

        private GUICanvas screenCanvas;

        private GUIList baseList;
        private GUITextBlock _sizeBlock;

        public void Initialize(ScreenManager screenManager)
        {
            _screenManager = screenManager;

            screenCanvas = new GUICanvas(Vector2.Zero, new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), 0, GUIStyle.GUIAlignment.None);
            
            baseList = new GUIList(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.Center, screenCanvas.Dimensions);
            screenCanvas.AddElement(baseList);

            //baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 35), "debug info", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            //{
            //    ToggleField = typeof(GameSettings).GetField("ui_debug"),
            //    Toggle = (bool)typeof(GameSettings).GetField("ui_debug").GetValue(null)
            //});

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "Radial Blur", GUIRenderer.MonospaceFont, Color.Gray, Color.White )
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("SwitchState"),
                ButtonMethodArgs = new object[] { ScreenManager.ScreenStates.RadialBlur }
            });

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "Pixelizer", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("SwitchState"),
                ButtonMethodArgs = new object[] { ScreenManager.ScreenStates.Pixelizer }
            });

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "Bokeh", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("SwitchState"),
                ButtonMethodArgs = new object[] { ScreenManager.ScreenStates.Bokeh }
            });

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "Particle Physics", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("SwitchState"),
                ButtonMethodArgs = new object[] { ScreenManager.ScreenStates.ParticlePhysics }
            });

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "Exit", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("ExitProgram")
            });

            baseList.ParentResized(screenCanvas.Dimensions);
        }
        
        public void SwitchState(ScreenManager.ScreenStates newstate)
        {
            _screenManager.NextState = newstate;
        }

        public void ExitProgram()
        {
            GameStats.NextState = ScreenManager.ScreenStates.Exit;
        }

        public void Load(ContentManager content)
        {

        }

        public void UpdateResolution()
        {
            screenCanvas.Dimensions = new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight);
            screenCanvas.ParentResized(screenCanvas.Dimensions);
        }
        
        public void Update(GameTime gameTime)
        {
            GameStats.UIWasUsed = false;
            if (GameSettings.ui_DrawUI)
            {
                //_sizeBlock.Text.Clear();
                //_sizeBlock.Text.Append("Model Size: ");
                //_sizeBlock.Text.Concat((float)Math.Pow(10, GameSettings.m_size), 2);
                
                screenCanvas.Update(gameTime, Input.GetMousePosition().ToVector2(), Vector2.Zero);
            }

            //Safety
            if (!Input.IsLMBPressed() && GameStats.UIElementEngaged)
                GameStats.UIElementEngaged = false;
        }

        public GUICanvas getCanvas()
        {
            return screenCanvas;
        }

    }
}
