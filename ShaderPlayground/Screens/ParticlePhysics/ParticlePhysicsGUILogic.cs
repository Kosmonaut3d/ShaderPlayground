using HelperSuite.GUI;
using HelperSuite.GUIHelper;
using HelperSuite.GUIRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.ParticlePhysics
{
    public class ParticlePhysicsGUILogic
    {
        private ScreenManager _screenManager;

        private GUICanvas screenCanvas;

        private GUIContentLoader _guiContentLoader;
        
        private GUITextBlock _sizeBlock;

        private ParticlePhysicsRenderer _renderer;

        public void Initialize(ScreenManager screenManager, ParticlePhysicsRenderer renderer)
        {
            _renderer = renderer;
            _screenManager = screenManager;

            screenCanvas = new GUICanvas(Vector2.Zero, new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), 0, GUIStyle.GUIAlignment.None);

            GUIStyle style = new GUIStyle(new Vector2(200, 35), GUIRenderer.MonospaceFont, Color.Gray, Color.White, Color.White, GUIStyle.GUIAlignment.None, GUIStyle.TextAlignment.Left, GUIStyle.TextAlignment.Center, Vector2.Zero, screenCanvas.Dimensions);

            var baseList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.TopRight, screenCanvas.Dimensions);
            screenCanvas.AddElement(baseList);
            
            baseList.AddElement(new GUITextBlockButton(style, "return")
            {
                ButtonObject = this,
                ButtonMethod = this.GetType().GetMethod("MainMenu")
            });


            Texture2D reference = null;
            GuiTextBlockLoadDialog textureLoader;
            baseList.AddElement(textureLoader = new GuiTextBlockLoadDialog(style, "image: ", _guiContentLoader, GuiTextBlockLoadDialog.ContentType.Texture2D)
            );

            baseList.AddElement(new GuiSliderFloatText(style, 0, 1, 2, "K: ")
            {
                SliderObject = renderer,
                SliderField = renderer.GetType().GetField("SpringConstant"),
                SliderValue = (float)renderer.GetType().GetField("SpringConstant").GetValue(renderer)
            });

            baseList.AddElement(new GuiSliderFloatText(style, 0, 1, 2, "Dampening: ")
            {
                SliderObject = renderer,
                SliderField = renderer.GetType().GetField("DampeningConstant"),
                SliderValue = (float)renderer.GetType().GetField("DampeningConstant").GetValue(renderer)
            });

            baseList.AddElement(new GuiSliderIntText(style, 1, 10, 1, "Scale: ")
            {
                SliderObject = renderer,
                SliderField = renderer.GetType().GetField("TestInt"),
                SliderValue = (int)renderer.GetType().GetField("TestInt").GetValue(renderer)
            });

            //baseList.AddElement(new GuiSliderFloatText(Vector2.Zero, new Vector2(200, 55), new Vector2(200, 20), 0, 1, "test", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            //{
            //    SliderObject = renderer,
            //    SliderField = renderer.GetType().GetField("TestFloat"),
            //    SliderValue = (float)renderer.GetType().GetField("TestFloat").GetValue(renderer)
            //});

            //baseList.AddElement(_sizeBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Intensity: ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            //baseList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 35), 2, 20, Color.Gray, Color.White)
            //{
            //    SliderObject = renderer,
            //    SliderField = typeof(DefaultRenderer).GetField("BokehSize"),
            //    SliderValue = (float)typeof(DefaultRenderer).GetField("BokehSize").GetValue(renderer)
            //});

            renderer._backgroundTextureLoader = textureLoader;
            
            baseList.ParentResized(screenCanvas.Dimensions);
        }

        //public void SwitchShape(DefaultRenderer.BokehShapes bokehShape)
        //{
        //    _renderer.BokehShape = bokehShape;
        //}

        public void Load(ContentManager content)
        {
            _guiContentLoader = new GUIContentLoader();
            _guiContentLoader.Load(content);
        }

        public void MainMenu()
        {
            _screenManager.NextState = ScreenManager.ScreenStates.MainMenu;
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
                screenCanvas.Update(gameTime, Input.GetMousePosition().ToVector2(), Vector2.Zero);

                //_sizeBlock.Text.Clear();
                //_sizeBlock.Text.Append("Bokeh Size: ");
                //_sizeBlock.Text.Concat(_renderer.BokehSize, 2);
                
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
