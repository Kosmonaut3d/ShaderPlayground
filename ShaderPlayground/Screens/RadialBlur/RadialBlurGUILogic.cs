using System;
using HelperSuite.GUI;
using HelperSuite.GUIHelper;
using HelperSuite.GUIRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.RadialBlur
{
    public class RadialBlurGUILogic
    {
        private ScreenManager _screenManager;

        private GUICanvas screenCanvas;

        private GUIContentLoader _guiContentLoader;

        private GUIList baseList;
        private GUITextBlock _intensityBlock;
        private GUITextBlock _samplesBlock;
        private GUITextBlock _passBlock;

        private RadialBlurRenderer _renderer;
        
        public void Initialize(ScreenManager screenManager, RadialBlurRenderer renderer)
        {
            _renderer = renderer;
            _screenManager = screenManager;

            screenCanvas = new GUICanvas(Vector2.Zero, new Vector2(GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), 0, GUIStyle.GUIAlignment.None);

            baseList = new GuiListToggle(Vector2.Zero, new Vector2(200, 30), 0, GUIStyle.GUIAlignment.TopRight, screenCanvas.Dimensions);
            screenCanvas.AddElement(baseList);
            
            baseList.AddElement(new GUITextBlockButton(Vector2.Zero, new Vector2(200, 35), "return", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ButtonObject = this,
                ButtonMethod = typeof(RadialBlurGUILogic).GetMethod("MainMenu")
            });
            

            Texture2D reference = null;
            GuiTextBlockLoadDialog textureLoader;
            baseList.AddElement(textureLoader = new GuiTextBlockLoadDialog(Vector2.Zero, new Vector2(200, 35), "image: ", _guiContentLoader, GuiTextBlockLoadDialog.ContentType.Texture2D,  GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            );


            baseList.AddElement(_intensityBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Intensity: " + renderer.BlurIntensity, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 35), 0, 1, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = typeof(RadialBlurRenderer).GetField("BlurIntensity"),
                SliderValue = (float) typeof(RadialBlurRenderer).GetField("BlurIntensity").GetValue(renderer)
            });

            baseList.AddElement(_samplesBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Samples: " + renderer.BlurSamples, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderInt(Vector2.Zero, new Vector2(200, 35), 1, 64, 1, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = typeof(RadialBlurRenderer).GetField("BlurSamples"),
                SliderValue = (int)typeof(RadialBlurRenderer).GetField("BlurSamples").GetValue(renderer)
            });

            baseList.AddElement(_passBlock = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Passes: " + renderer.BlurPasses, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderInt(Vector2.Zero, new Vector2(200, 35), 1, 3, 1, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = typeof(RadialBlurRenderer).GetField("BlurPasses"),
                SliderValue = (int)typeof(RadialBlurRenderer).GetField("BlurPasses").GetValue(renderer)
            });

            baseList.AddElement(new GUIBlock(Vector2.Zero, new Vector2(200, 25), Color.DimGray));

            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 55), "Click to freeze center point ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));

            renderer._backgroundTextureLoader = textureLoader;
            
            baseList.ParentResized(screenCanvas.Dimensions);
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

                _intensityBlock.Text.Clear();
                _intensityBlock.Text.Append("Intensity: ");
                _intensityBlock.Text.Concat(_renderer.BlurIntensity, 2);

                _samplesBlock.Text.Clear();
                _samplesBlock.Text.Append("Samples: ");
                _samplesBlock.Text.Concat(_renderer.BlurSamples);

                _passBlock.Text.Clear();
                _passBlock.Text.Append("Passes: ");
                _passBlock.Text.Concat(_renderer.BlurPasses);
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
