using System.Numerics;
using HelperSuite.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Screens.DefaultPreset.ShaderModules;
using ShaderPlayground.Screens.FourierTransform.FourierTransforms;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.FourierTransform
{
    public class FTRenderer
    {
        private GraphicsDevice _graphics;

        public GuiTextBlockLoadDialog _backgroundTextureLoader;

        public SpriteBatch _spriteBatch;
        
        private Texture2D DefaultTexture2D;
        private Texture2D DFTTexture2D;

        private DFT dftFunction;


        public void Initialize(GraphicsDevice graphics)
        {
            _graphics = graphics;
            
            _spriteBatch = new SpriteBatch(graphics);

            dftFunction = new DFT();
        }

        public void Load(ContentManager content)
        {
            DefaultTexture2D = content.Load<Texture2D>("carred");

        }

        public void Draw(GameTime gameTime, RenderTarget2D outputRT)
        {
            Texture2D input = CheckRenderChanges();

            _graphics.Clear(Color.Black);

            _spriteBatch.Begin();
            
            _spriteBatch.Draw(input, new Rectangle(0, 0, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), Color.White);

            if (DFTTexture2D != null)
                _spriteBatch.Draw(DFTTexture2D, new Rectangle(0, 0, GameSettings.g_ScreenWidth, GameSettings.g_ScreenHeight), Color.White);

            _spriteBatch.End();


            //Dynamic
        }

        public void CreateDFT()
        {
            //Create an array of complex numbers

            Texture2D input = CheckRenderChanges();

            DFTTexture2D = new Texture2D(_graphics, input.Width, input.Height);

            Complex[,] data = TextureTo2DArray(input);

            data = dftFunction.CreateDFT(data);

            ArrayToTexture(DFTTexture2D, data);
        }

        Complex[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colorsOne = new Color[texture.Width * texture.Height]; //The hard to read,1D array
            texture.GetData(colorsOne); //Get the colors and add them to the array

            Complex[,] colorsTwo = new Complex[texture.Width, texture.Height]; //The new, easy to read 2D array
            for (int x = 0; x < texture.Width; x++) //Convert!
            for (int y = 0; y < texture.Height; y++)
                colorsTwo[x, y] = new Complex(colorsOne[x + y * texture.Width].R, 0);

            return colorsTwo; //Done!
        }

        void ArrayToTexture(Texture2D texture, Complex[,] data)
        {
            Color[] colorsOne = new Color[texture.Width * texture.Height];

            for (int x = 0; x < texture.Width; x++) //Convert!
            for (int y = 0; y < texture.Height; y++)
            {
                float value = (float) data[x, y].Real;
                colorsOne[x + y * texture.Width] = new Color(Vector3.One * value);
            }

            texture.SetData(colorsOne); //Get the colors and add them to the array
        }

        private Texture2D CheckRenderChanges()
        {
            //What image are we using?
            if (_backgroundTextureLoader.LoadedObject != null)
            {
                return _backgroundTextureLoader.LoadedObject as Texture2D;
            }
            else
            {
                return DefaultTexture2D;
            }
        }
    }
}
