using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using ShaderPlayground.Controls;
using ShaderPlayground.Settings;

namespace ShaderPlayground.HelperSuite.GUI
{
    class GuiSliderInt : GuiSliderFloat
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
            }
        }

        public GuiSliderInt(GUIStyle guiStyle, int min, int max, int stepSize) : this(
            position: Vector2.Zero, 
            dimensions: new Vector2(guiStyle.DimensionsStyle.X, 35),
            min: min,
            max: max,
            stepSize: stepSize,
            color: guiStyle.BlockColorStyle,
            sliderColor: guiStyle.SliderColorStyle,
            layer: 0,
            alignment: guiStyle.GuiAlignmentStyle,
            ParentDimensions: guiStyle.ParentDimensionsStyle
            )
        { }

        public GuiSliderInt(Vector2 position, Vector2 dimensions, int min, int max, int stepSize, Color color, Color sliderColor, int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 ParentDimensions = new Vector2()) : base(position, dimensions, min, max, color, sliderColor, layer, alignment, ParentDimensions)
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

            Vector2 bound1 = Position + parentPosition /*+ SliderIndicatorBorder*Vector2.UnitX*/;
            Vector2 bound2 = bound1 + Dimensions/* - 2*SliderIndicatorBorder * Vector2.UnitX*/;

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

                _sliderValue =  (int) Math.Round(_sliderPercent * (float)(MaxValue - MinValue) + MinValue) / StepSize * StepSize;

                _sliderPercent = (float)(_sliderValue - MinValueInt)/( MaxValueInt - MinValueInt);

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