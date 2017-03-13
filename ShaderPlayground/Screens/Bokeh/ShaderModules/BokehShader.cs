using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Helpers;
using ShaderPlayground.HelperSuite.ContentLoader;
using ShaderPlayground.Settings;

namespace ShaderPlayground.Screens.Bokeh.ShaderModules
{
    public class BokehShader
    {
        private GraphicsDevice _graphicsDevice;
        private ThreadSafeContentManager _contentManager;

        private bool _effectLoaded = false;

        private QuadRenderer _quadRenderer;
        
        //Shader Effect
        private Effect shaderEffect;
        private Effect shaderEffectLoad;
        private Task loadTaskOut;

        //Bokeh QUad
        private Vector2 v1 = Vector2.One;
        private Vector2 v2 = Vector2.One;
        private Vector2 vcenter = Vector2.One;
        private Vector2 texCoord = Vector2.One;
        private int primitiveCount;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private BokehVertex[] _vertices;
        private int[] _indices;

        //Parameters
        private EffectParameter _screenParameter;
        private EffectParameter _shapeParameter;
        private EffectParameter _imageTexCoordParameter;
        private EffectParameter _brighnessParameter;

        private EffectPass _defaultPass;
        private EffectPass _texPass;

        private Texture2D _screenTexture;
        private RenderTarget2D _renderTarget0;
        private Texture2D _shapeTexture;

        private float _brightness = 0.01f;
        private float _bokehSize = -1f;
        private float _bokehSizeBrightness = 1;
        private int _downsize = -1;
        private bool _fullPrecision = false;
        private bool _downsizeChanged = false;
        private RasterizerState WireFrameRasterizer;

        public Texture2D ScreenTexture
        {
            get
            {
                return _screenTexture;
            }

            set
            {
                if (value != _screenTexture)
                {
                    _screenTexture = value;

                    _screenParameter?.SetValue(_screenTexture);
                }
            }
        }


        public Texture2D ShapeTexture
        {
            get
            {
                return _shapeTexture;
            }

            set
            {
                if (value != _shapeTexture)
                {
                    _shapeTexture = value;

                    _shapeParameter?.SetValue(_shapeTexture);
                }
            }
        }

        public float Brightness
        {
            get { return _brightness; }
            set
            {
                if (Math.Abs(_brightness - value) > 0.0001f)
                {
                    _brightness = value;
                    _brighnessParameter.SetValue(_brightness * _bokehSizeBrightness);
                }
            }
        }

        public void Initialize(GraphicsDevice graphics)
        {
            _graphicsDevice = graphics;
            _quadRenderer = new QuadRenderer();
        }
        
        public void Load(ContentManager content, string path)
        {
            //Hot swapping
            _contentManager = new ThreadSafeContentManager(content.ServiceProvider) { RootDirectory = "Content" };

            WireFrameRasterizer = new RasterizerState();
            WireFrameRasterizer.FillMode = FillMode.WireFrame;
            WireFrameRasterizer.CullMode = CullMode.None;
            ReloadShader(path);
        }

        private void ReloadShader(string path)
        {
            //If already have one -> dispose
            shaderEffectLoad?.Dispose();

            _effectLoaded = false;
            loadTaskOut = Task.Factory.StartNew(() =>
            {
                shaderEffectLoad = _contentManager.Load<Effect>(path);
            });
        }

        public void Draw(Texture2D tex, Texture2D shape, RenderTarget2D rt, float brightness, float bokehSize, int downsize, bool fullPrecision)
        {
            InitializeParameters();
            
            if (!_effectLoaded) return;

            //Create Quads
            if (Math.Abs(bokehSize - _bokehSize) > 0.01f || downsize!=_downsize)
            {
                _bokehSize = bokehSize;

                if (_downsize != downsize)
                {
                    _downsize = downsize;
                    _downsizeChanged = true;
                    _fullPrecision = fullPrecision;
                    CreateRT(downsize, fullPrecision);
                }

                CreateQuads(_bokehSize, downsize);
            }

            if (_fullPrecision != fullPrecision)
            {
                _fullPrecision = fullPrecision;
                CreateRT(downsize, fullPrecision);
            }

            ScreenTexture = tex;
            ShapeTexture = shape;
            Brightness = brightness;
            
            _graphicsDevice.SetRenderTarget(_renderTarget0);
            //1280 800

            _defaultPass.Apply();

            _graphicsDevice.RasterizerState = RasterizerState.CullNone;
            _graphicsDevice.BlendState = BlendState.Additive;

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitiveCount);

            //Draw to RT
            _graphicsDevice.SetRenderTarget(rt);
            _graphicsDevice.Clear(Color.Black);
            
            ScreenTexture = _renderTarget0;
            _texPass.Apply();
            _quadRenderer.RenderQuad(_graphicsDevice, -Vector2.One, Vector2.One);
        }

        private void CreateRT(int scale, bool fullPrecision)
        {
            if (_renderTarget0 != null)
            {
                _renderTarget0.Dispose();
            }

            int w = GameSettings.g_ScreenWidth / scale;
            int h = GameSettings.g_ScreenHeight / scale;

            _renderTarget0 = new RenderTarget2D(_graphicsDevice, w, h, false, fullPrecision ?  SurfaceFormat.Vector4 : SurfaceFormat.HalfVector4, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            
        }

        private void CreateQuads(float size, int downsample)
        {
            //Create one giant vertexbuffer that holds all the quads for the pixels
            //The quads are created with the final size already
            //Alternatively one could just pass the center position of the quad to all vertices and expand inside the vertex shader (see: GPU particle sample for xna)
            //This would make scaling trivial, since nothing has to be recomputed on the CPU
            //But it would increase the runtime cost per quad slightly.

            int xsize = 1280;
            int ysize = 800;

            int col = xsize / downsample;
            int row = ysize / downsample;

            int verticeCount = col*row*4;
            int indiceCount = col*row*6;

            primitiveCount = verticeCount/2;

            if (_vertices == null || _downsizeChanged)
            {
                _vertices = new BokehVertex[verticeCount];
                _indices = new int[indiceCount];
            }

            float sizex = 1.0f / col ;
            float sizey = 1.0f / row ;

            float sizeMultiplier = size;

            _bokehSizeBrightness = 1.0f/(size*size);

            //Apply
            _brighnessParameter.SetValue(_brightness * _bokehSizeBrightness);

            int vertexIndex = 0;
            int indexIndex = 0;

            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    CreateQuad((i + 0.5f) * sizex, (j + 0.5f) * sizey, sizex * sizeMultiplier, sizey * sizeMultiplier, vertexIndex, indexIndex);

                    vertexIndex += 4;
                    indexIndex += 6;
                }
            }

            if (_vertexBuffer == null || _downsizeChanged)
            {
                if (_vertexBuffer != null)
                {
                    _vertexBuffer.Dispose();
                    _indexBuffer.Dispose();
                }

                _vertexBuffer = new VertexBuffer(_graphicsDevice, BokehVertex.VertexDeclaration, verticeCount, BufferUsage.None);
                _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits, indiceCount, BufferUsage.None);
            }
            
            _vertexBuffer.SetData(_vertices);
            _indexBuffer.SetData(_indices);
        }

        private void CreateQuad(float x, float y, float sizex, float sizey, int vertexIndex, int indexIndex)
        {
            texCoord.X = x;
            texCoord.Y = 1-y;

            x = x*2 - 1;
            y = (y*2 - 1);

            v1.X = x - sizex*4;
            v1.Y = y - sizey*4;
            v2.X = x + sizex*4;
            v2.Y = y + sizey*4;
            
            _vertices[0 + vertexIndex] = new BokehVertex(new Vector3(v1.X, v2.Y, 1), new Vector2(0,0), texCoord);
            _vertices[1 + vertexIndex] = new BokehVertex(new Vector3(v2.X, v2.Y, 1), new Vector2(1, 0), texCoord);
            _vertices[2 + vertexIndex] = new BokehVertex(new Vector3(v1.X, v1.Y, 1), new Vector2(0, 1), texCoord);
            _vertices[3 + vertexIndex] = new BokehVertex(new Vector3(v2.X, v1.Y, 1), new Vector2(1, 1), texCoord);

            _indices[0 + indexIndex] = 0 + vertexIndex;
            _indices[1 + indexIndex] = 3 + vertexIndex;
            _indices[2 + indexIndex] = 2 + vertexIndex;
            _indices[3 + indexIndex] = 0 + vertexIndex;
            _indices[4 + indexIndex] = 1 + vertexIndex;
            _indices[5 + indexIndex] = 3 + vertexIndex;

            //_vertexBuffer = new VertexPositionTexture[4];
            //_vertexBuffer[0] = new VertexPositionTexture(new Vector3(-1, 1, 1), new Vector2(0, 0));
            //_vertexBuffer[1] = new VertexPositionTexture(new Vector3(1, 1, 1), new Vector2(1, 0));
            //_vertexBuffer[2] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1));
            //_vertexBuffer[3] = new VertexPositionTexture(new Vector3(1, -1, 1), new Vector2(1, 1));

            //_vertexBuffer[0].Position.X = v1.X;
            //_vertexBuffer[0].Position.Y = v2.Y;

            //_vertexBuffer[1].Position.X = v2.X;
            //_vertexBuffer[1].Position.Y = v2.Y;

            //_vertexBuffer[2].Position.X = v1.X;
            //_vertexBuffer[2].Position.Y = v1.Y;

            //_vertexBuffer[3].Position.X = v2.X;
            //_vertexBuffer[3].Position.Y = v1.Y;

            //_indexBuffer = new short[] { 0, 3, 2, 0, 1, 3 };
        }


        private void InitializeParameters()
        {
            if (_effectLoaded) return;

            if (loadTaskOut == null) return;

            //Is still loading
            if (loadTaskOut.Status == TaskStatus.Running) return;

            //Has finished loading -> initialize parameters

            //Swap shader effects
            shaderEffect?.Dispose();
            shaderEffect = shaderEffectLoad;

            _screenParameter = shaderEffect.Parameters["Screen"];
            _shapeParameter = shaderEffect.Parameters["Shape"];
            _imageTexCoordParameter = shaderEffect.Parameters["ImageTexCoord"];
            _brighnessParameter = shaderEffect.Parameters["Brightness"];

            _defaultPass = shaderEffect.Techniques["Default"].Passes[0];
            _texPass = shaderEffect.Techniques["Texture"].Passes[0];

            //Signal we have loaded the effect
            _effectLoaded = true;

        }
        
    }

    public struct BokehVertex
    {
        // Stores the starting position of the particle.
        public Vector3 Position;
        
        public Vector2 TexCoord;
        
        public Vector2 TexCoord2;


        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
          new VertexElement(0, VertexElementFormat.Vector3,
                                 VertexElementUsage.Position, 0),
          new VertexElement(12, VertexElementFormat.Vector2,
                                 VertexElementUsage.TextureCoordinate, 0),
          new VertexElement(20, VertexElementFormat.Vector2,
                                 VertexElementUsage.TextureCoordinate, 1)
        );

        public BokehVertex(Vector3 position, Vector2 texCoord, Vector2 texCoord2)
        {
            Position = position;
            TexCoord = texCoord;
            TexCoord2 = texCoord2;
        }

        public const int SizeInBytes = 28;
    }
}
