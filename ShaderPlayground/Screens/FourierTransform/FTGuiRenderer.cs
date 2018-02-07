using HelperSuite.GUI;
using HelperSuite.GUIHelper;
using HelperSuite.GUIRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Screens.DefaultPreset;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.FourierTransform
{
    public class FTGuiRenderer
    {
        private ScreenManager _screenManager;

        private GUICanvas screenCanvas;

        private GUIContentLoader _guiContentLoader;
        
        private FTRenderer _renderer;

        public void Initialize(ScreenManager screenManager, FTRenderer renderer)
        {
            _renderer = renderer;
            _screenManager = screenManager;

            screenCanvas = new GUICanvas(Vector2.Zero, new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), 0, GUIStyle.GUIAlignment.None);

            var baseList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.TopRight, screenCanvas.Dimensions);
            screenCanvas.AddElement(baseList);
            
            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "return", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = this.GetType().GetMethod("MainMenu")
            });

            
            GuiTextBlockLoadDialog textureLoader;
            baseList.AddElement(textureLoader = new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "image: ", _guiContentLoader, GuiTextBlockLoadDialog.ContentType.Texture2D,  GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            );

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "CreateDFT", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = renderer,
                ButtonMethod = renderer.GetType().GetMethod("CreateDFT")
            });

            //baseList.AddElement(_sizeBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Intensity: ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            //baseList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 35), 2, 20, Color.Gray, Color.White)
            //{
            //    SliderObject = renderer,
            //    SliderField = typeof(FTRenderer).GetField("BokehSize"),
            //    SliderValue = (float)typeof(FTRenderer).GetField("BokehSize").GetValue(renderer)
            //});

            renderer._backgroundTextureLoader = textureLoader;
            
            baseList.ParentResized(screenCanvas.Dimensions);
        }

        //public void SwitchShape(FTRenderer.BokehShapes bokehShape)
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
