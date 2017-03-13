using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.HelperSuite.GUI;
using ShaderPlayground.HelperSuite.GUIHelper;
using ShaderPlayground.HelperSuite.GUIRenderer;
using ShaderPlayground.HelperSuite.GUIRenderer.Helper;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.RadialBlur
{
    public class PixelizerGUILogic
    {
        private ScreenManager _screenManager;

        private GUICanvas screenCanvas;

        private GUIContentLoader _guiContentLoader;

        private GUIList baseList;
        private GUITextBlock _splitChance;
        private GUITextBlock _endChance;
        private GUITextBlock _scale;

        private PixelizerRenderer _renderer;

        public void Initialize(ScreenManager screenManager, PixelizerRenderer renderer)
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

            baseList.AddElement(new GUITextBlockToggle(Vector2.Zero, new Vector2(200, 35), "Random points", GUIRenderer.MonospaceFont, Color.Gray, Color.White)
            {
                ToggleObject = renderer,
                ToggleField = renderer.GetType().GetField("Random"),
                Toggle = (bool) renderer.GetType().GetField("Random").GetValue(renderer)
            });

            baseList.AddElement(_splitChance = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Split Chance: " + renderer.SplitChance, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 35), 0, 1, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = renderer.GetType().GetField("SplitChance"),
                SliderValue = (float) renderer.GetType().GetField("SplitChance").GetValue(renderer)
            });

            baseList.AddElement(_endChance = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "End Chance: " + renderer.EndChance, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderFloat(Vector2.Zero, new Vector2(200, 35), 0, 1, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = renderer.GetType().GetField("EndChance"),
                SliderValue = (float)renderer.GetType().GetField("EndChance").GetValue(renderer)
            });

            baseList.AddElement(_scale = new GUITextBlock(Vector2.Zero, new Vector2(200, 25), "Scale: " + renderer.Scale, GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            baseList.AddElement(new GuiSliderInt(Vector2.Zero, new Vector2(200, 35), 0, 3, 1, Color.Gray, Color.White)
            {
                SliderObject = renderer,
                SliderField = renderer.GetType().GetField("Scale"),
                SliderValue = (int)renderer.GetType().GetField("Scale").GetValue(renderer)
            });

            baseList.AddElement(new GUIColorPicker(Vector2.Zero, new Vector2(200,200), Color.Gray, GUIRenderer.MonospaceFont)
            {
                ReferenceObject = renderer,
                ReferenceField = renderer.GetType().GetField("LineColor")
            });
            
            baseList.AddElement(new GUITextBlock(Vector2.Zero, new Vector2(200, 55), "Click to add effect ", GUIRenderer.MonospaceFont, Color.Gray, Color.White));
            
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

                _splitChance.Text.Clear();
                _splitChance.Text.Append("Split Chance: ");
                _splitChance.Text.Concat(_renderer.SplitChance, 2);

                _endChance.Text.Clear();
                _endChance.Text.Append("End Chance: ");
                _endChance.Text.Concat(_renderer.EndChance, 2);

                _scale.Text.Clear();
                _scale.Text.Append("Scale: 1 / ");
                _scale.Text.Concat(1 << _renderer.Scale);
                //_samplesBlock.Text.Clear();
                //_samplesBlock.Text.Append("Samples: ");
                //_samplesBlock.Text.Concat(_renderer.BlurSamples);

                //_passBlock.Text.Clear();
                //_passBlock.Text.Append("Passes: ");
                //_passBlock.Text.Concat(_renderer.BlurPasses);
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
