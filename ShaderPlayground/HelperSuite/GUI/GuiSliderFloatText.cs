using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Controls;
using ShaderPlayground.Settings;
using ShaderPlayground.HelperSuite.GUIRenderer.Helper;

namespace ShaderPlayground.HelperSuite.GUI
{
    class GuiSliderFloatText : GUIBlock
    {
        protected bool IsEngaged = false;

        protected const float SliderIndicatorSize = 15;
        protected const float SliderIndicatorBorder = 10;
        protected const float SliderBaseHeight = 5;

        private Vector2 SliderPosition;
        private Vector2 _tempPosition = Vector2.One;

        protected Vector2 SliderDimensions;

        protected float _sliderPercent;

        private float _sliderValue;
        public float SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                _sliderPercent = (_sliderValue - MinValue)/(MaxValue - MinValue);
                UpdateText();
            }
        }

        public float MaxValue = 1; 
        public float MinValue;

        protected Color _sliderColor;
        
        //TextBlock associated
        protected GUITextBlock _textBlock;
        private uint roundDecimals = 1;
        protected String baseText;

        //Associated reference
        public PropertyInfo SliderProperty;
        public FieldInfo SliderField;
        public Object SliderObject;


        public GuiSliderFloatText(Vector2 position, Vector2 sliderDimensions, Vector2 textdimensions, float min, float max, uint decimals, String text, SpriteFont font, Color color, Color sliderColor, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, GUIStyle.TextAlignment textAlignment = GUIStyle.TextAlignment.Left, Vector2 textBorder = default(Vector2), Vector2 ParentDimensions = new Vector2()) : base(position, sliderDimensions, color, layer, alignment, ParentDimensions)
        {
            _textBlock = new GUITextBlock(position, textdimensions, text, font, color, sliderColor, textAlignment, textBorder, layer, alignment, ParentDimensions);

            Dimensions = sliderDimensions + _textBlock.Dimensions*Vector2.UnitY;
            SliderDimensions = sliderDimensions;
            _sliderColor = sliderColor;
            MinValue = min;
            MaxValue = max;
            _sliderValue = min;
            roundDecimals = decimals;
            baseText = text;

            UpdateText();
        }

        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            if (GameStats.UIElementEngaged && !IsEngaged) return;

            //Break Engagement
            if (IsEngaged && !Input.IsLMBPressed())
            {
                GameStats.UIElementEngaged = false;
                IsEngaged = false;
            }

            if (!Input.IsLMBPressed()) return;

            Vector2 bound1 = Position + parentPosition + _textBlock.Dimensions * Vector2.UnitY /*+ SliderIndicatorBorder*Vector2.UnitX*/;
            Vector2 bound2 = bound1 + SliderDimensions/* - 2*SliderIndicatorBorder * Vector2.UnitX*/;

            if (mousePosition.X >= bound1.X && mousePosition.Y >= bound1.Y && mousePosition.X < bound2.X &&
                mousePosition.Y < bound2.Y + 1)
            {
                GameStats.UIElementEngaged = true;
                IsEngaged = true;
            }

            if (IsEngaged)
            {
                GameStats.UIWasUsed = true;

                float lowerx = bound1.X + SliderIndicatorBorder;
                float upperx = bound2.X - SliderIndicatorBorder;

                _sliderPercent = MathHelper.Clamp((mousePosition.X - lowerx)/(upperx - lowerx), 0, 1);

                _sliderValue = _sliderPercent*(MaxValue - MinValue) + MinValue;

                UpdateText();

                if (SliderObject != null)
                {
                    if (SliderField != null)
                        SliderField.SetValue(SliderObject, SliderValue, BindingFlags.Public, null, null);
                    else if (SliderProperty != null) SliderProperty.SetValue(SliderObject, SliderValue);
                }
                else
                {
                    if (SliderField != null)
                        SliderField.SetValue(null, SliderValue, BindingFlags.Static | BindingFlags.Public, null, null);
                    else if (SliderProperty != null) SliderProperty.SetValue(null, SliderValue);
                }
            }
        }

        private void UpdateText()
        {
            _textBlock.Text.Clear();
            _textBlock.Text.Append(baseText);
            _textBlock.Text.Concat(_sliderValue, roundDecimals);
        }

        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition, Vector2 mousePosition)
        {
            _textBlock.Draw(guiRenderer, parentPosition, mousePosition);

            _tempPosition = parentPosition + Position + _textBlock.Dimensions*Vector2.UnitY;
            guiRenderer.DrawQuad(_tempPosition, SliderDimensions, Color);
            
            Vector2 slideDimensions = new Vector2(SliderDimensions.X - SliderIndicatorBorder*2, SliderBaseHeight);
            guiRenderer.DrawQuad(_tempPosition + new Vector2(SliderIndicatorBorder,
                SliderDimensions.Y* 0.5f - SliderBaseHeight * 0.5f), slideDimensions, Color.DarkGray);

            //slideDimensions = new Vector2(slideDimensions.X + SliderIndicatorSize* 0.5f, slideDimensions.Y);
            guiRenderer.DrawQuad(_tempPosition + new Vector2(SliderIndicatorBorder - SliderIndicatorSize* 0.5f,
                 SliderDimensions.Y * 0.5f - SliderIndicatorSize * 0.5f) + _sliderPercent*slideDimensions * Vector2.UnitX, new Vector2(SliderIndicatorSize, SliderIndicatorSize), _sliderColor);
        }
    }

    class GuiSliderIntText : GuiSliderFloatText
    {
        public int MaxValueInt = 1;
        public int MinValueInt = 0;
        public int StepSize = 1;

        public int _sliderValue;
        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                _sliderPercent = (float)(_sliderValue - MinValue) / (MaxValue - MinValue);

                UpdateText();
            }
        }

        private void UpdateText()
        {
            _textBlock.Text.Clear();
            _textBlock.Text.Append(baseText);
            _textBlock.Text.Concat(_sliderValue);
        }

        public GuiSliderIntText(Vector2 position, Vector2 sliderDimensions, Vector2 textdimensions, int min, int max, int stepSize, String text, SpriteFont font, Color color, Color sliderColor, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, GUIStyle.TextAlignment textAlignment = GUIStyle.TextAlignment.Left, Vector2 textBorder = default(Vector2), Vector2 ParentDimensions = new Vector2()) : base(position, sliderDimensions, textdimensions, min, max, 0, text, font, color, sliderColor, layer, alignment, textAlignment, textBorder, ParentDimensions)
        {
            MaxValueInt = max;
            MinValueInt = min;
            StepSize = stepSize;
        }


        public override void Update(GameTime gameTime, Vector2 mousePosition, Vector2 parentPosition)
        {
            if (GameStats.UIElementEngaged && !IsEngaged) return;

            //Break Engagement
            if (IsEngaged && !Input.IsLMBPressed())
            {
                GameStats.UIElementEngaged = false;
                IsEngaged = false;
            }

            if (!Input.IsLMBPressed()) return;

            Vector2 bound1 = Position + parentPosition + _textBlock.Dimensions * Vector2.UnitY /*+ SliderIndicatorBorder*Vector2.UnitX*/;
            Vector2 bound2 = bound1 + SliderDimensions/* - 2*SliderIndicatorBorder * Vector2.UnitX*/;

            if (mousePosition.X >= bound1.X && mousePosition.Y >= bound1.Y && mousePosition.X < bound2.X &&
                mousePosition.Y < bound2.Y + 1)
            {
                GameStats.UIElementEngaged = true;
                IsEngaged = true;
            }

            if (IsEngaged)
            {
                GameStats.UIWasUsed = true;

                float lowerx = bound1.X + SliderIndicatorBorder;
                float upperx = bound2.X - SliderIndicatorBorder;

                _sliderPercent = MathHelper.Clamp((mousePosition.X - lowerx) / (upperx - lowerx), 0, 1);

                _sliderValue = (int)Math.Round(_sliderPercent * (float)(MaxValue - MinValue) + MinValue) / StepSize * StepSize;

                UpdateText();

                _sliderPercent = (float)(_sliderValue - MinValueInt) / (MaxValueInt - MinValueInt);

                if (SliderObject != null)
                {
                    if (SliderField != null) SliderField.SetValue(SliderObject, SliderValue, BindingFlags.Public, null, null);
                    else if (SliderProperty != null) SliderProperty.SetValue(SliderObject, SliderValue);
                }
                else
                {
                    if (SliderField != null) SliderField.SetValue(null, SliderValue, BindingFlags.Static | BindingFlags.Public, null, null);
                    else if (SliderProperty != null) SliderProperty.SetValue(null, SliderValue);
                }
            }
        }
    }

}