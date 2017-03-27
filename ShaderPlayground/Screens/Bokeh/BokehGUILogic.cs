using HelperSuite.GUI;
using HelperSuite.GUIHelper;
using HelperSuite.GUIRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.Bokeh
{
    public class BokehGUILogic
    {
        private ScreenManager _screenManager;

        private GUICanvas screenCanvas;

        private GUIContentLoader _guiContentLoader;

        private GUIList baseList;
        private GUITextBlock _sizeBlock;
        private GUITextBlock _brightnessBlock;
        private GUITextBlock _downsizeBlock;

        private GUITextBlockButton _pentagonToggle;
        private GUITextBlockButton _circleToggle;
        private GUITextBlockButton _starToggle;

        private GUITextBlock _polyCount;

        private BokehRenderer _renderer;

        public void Initialize(ScreenManager screenManager, BokehRenderer renderer)
        {
            _renderer = renderer;
            _screenManager = screenManager;

            screenCanvas = new GUICanvas(Vector2.Zero, new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), 0, GUIStyle.GUIAlignment.None);

            baseList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.TopRight, screenCanvas.Dimensions);
            screenCanvas.AddElement(baseList);
            
            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "return", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = this.GetType().GetMethod("MainMenu")
            });


            Texture2D reference = null;
            GuiTextBlockLoadDialog textureLoader;
            baseList.AddElement(textureLoader = new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "image: ", _guiContentLoader, GuiTextBlockLoadDialog.ContentType.Texture2D,  GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            );


            baseList.AddElement(_sizeBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Intensity: " + renderer.BokehSize, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 35), 2, 20, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = typeof(BokehRenderer).GetField("BokehSize"),
                SliderValue = (float)typeof(BokehRenderer).GetField("BokehSize").GetValue(renderer)
            });

            baseList.AddElement(_brightnessBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Brightness: " + renderer.Brightness, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 35), 0.1f, 3, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = typeof(BokehRenderer).GetField("Brightness"),
                SliderValue = (float)typeof(BokehRenderer).GetField("Brightness").GetValue(renderer)
            });

            baseList.AddElement(_downsizeBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Scale: " + renderer.Downsize, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderInt(Vector2.Zero, new Vector2(200, 35), 1, 3, 1, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = renderer.GetType().GetField("Downsize"),
                SliderValue = (int)renderer.GetType().GetField("Downsize").GetValue(renderer)
            });

            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 25), "32-bit precision", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ToggleObject = renderer,
                ToggleField = renderer.GetType().GetField("FullPrecision"),
                Toggle = renderer.FullPrecision,
            });

            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 25), "Dynamic Scaling", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ToggleObject = renderer,
                ToggleField = renderer.GetType().GetField("DynamicScaling"),
                Toggle = renderer.DynamicScaling,
            });

            baseList.AddElement(new GUIBlock(Vector2.Zero, new Vector2(200, 25), Color.DimGray));

            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Bokeh Shape: ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));

            //Radio list
            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 25), "Pentagon", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("SwitchShape"),
                ButtonMethodArgs = new object[] { BokehRenderer.BokehShapes.Pentagon }
            });

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 25), "Hexagon", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("SwitchShape"),
                ButtonMethodArgs = new object[] { BokehRenderer.BokehShapes.Hexagon }
            });

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 25), "Circle", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("SwitchShape"),
                ButtonMethodArgs = new object[] { BokehRenderer.BokehShapes.Circle }
            });

            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 25), "Star", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = GetType().GetMethod("SwitchShape"),
                ButtonMethodArgs = new object[] { BokehRenderer.BokehShapes.Star }
            });
            
            baseList.AddElement(new GUIBlock(Vector2.Zero, new Vector2(200, 25), Color.DimGray));

            baseList.AddElement(_polyCount = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "PolyCount: ", GUIRenderer.MonospaceFont, Color.DimGray, Color.White));


            renderer._backgroundTextureLoader = textureLoader;
            
            baseList.ParentResized(screenCanvas.Dimensions);
        }

        public void SwitchShape(BokehRenderer.BokehShapes bokehShape)
        {
            _renderer.BokehShape = bokehShape;
        }

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

                _sizeBlock.Text.Clear();
                _sizeBlock.Text.Append("Bokeh Size: ");
                _sizeBlock.Text.Concat(_renderer.BokehSize, 2);


                _brightnessBlock.Text.Clear();
                _brightnessBlock.Text.Append("Brightness: ");
                _brightnessBlock.Text.Concat(_renderer.Brightness, 2);


                _downsizeBlock.Text.Clear();
                _downsizeBlock.Text.Append("Scale: 1 / ");
                _downsizeBlock.Text.Concat(1 << _renderer.Downsize);

                _polyCount.Text.Clear();
                _polyCount.Text.Append("PolyCount: ");
                _polyCount.Text.Concat(_renderer.PolyCount);
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
