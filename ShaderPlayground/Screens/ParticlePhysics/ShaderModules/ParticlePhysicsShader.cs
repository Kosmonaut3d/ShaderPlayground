using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderPlayground.Helpers;
using ShaderPlayground.HelperSuite.ContentLoader;
using ShaderPlayground.Screens.Debug;

namespace ShaderPlayground.Screens.ParticlePhysics.ShaderModules
{
    public class ParticlePhysicsShader
    {
        private GraphicsDevice _graphicsDevice;
        private ThreadSafeContentManager _contentManager;
        private FullScreenQuadRenderer _fsqRenderer;

        private bool _effectLoaded = false;
        
        //Shader Effect
        private Effect shaderEffect;
        private Effect shaderEffectLoad;
        private Task loadTaskOut;

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private Vector2 position = Vector2.One;
        
        //Parameters
        private EffectParameter _texBufferParameter;
        private EffectParameter _texBufferSizeParameter;
        private EffectParameter _mousePositionParameter;
        private EffectParameter _spriteParameter;
        private EffectParameter _sizeParameter;
        private EffectParameter _springConstantParameter;
        private EffectParameter _dampeningConstantParameter;

        private EffectPass _defaultPass;
        private EffectPass _physicsPass;
        private EffectPass _physicsInitializePass;
        private RenderTarget2D _textureBufferA;
        private RenderTarget2D _textureBufferB;
        private int _primitiveCount;

        private bool offframe = false;

        private Texture2D _sprite;

        private int _scale = 1;

        private float _springConstant;
        private float _dampeningConstant;

        public Texture2D Sprite
        {
            get { return _sprite; }
            set {
                if (_sprite != value)
                {
                    _sprite = value;
                    _spriteParameter.SetValue(_sprite);
                } }
        }

        public float SpringConstant
        {
            get { return _springConstant; }
            set {
                if (System.Math.Abs(_springConstant - value) > 0.001f)
                {
                    _springConstant = value;
                    _springConstantParameter.SetValue(value);
                }
            }
        }

        public float DampeningConstant
        {
            get { return _dampeningConstant; }
            set
            {
                if (System.Math.Abs(_dampeningConstant - value) > 0.001f)
                {
                    _dampeningConstant = value;
                    _dampeningConstantParameter.SetValue(value);
                }
            }
        }

        public void Initialize(GraphicsDevice graphics)
        {
            _graphicsDevice = graphics;

            _fsqRenderer = new FullScreenQuadRenderer(graphics);
        }

        public void Load(ContentManager content, string path)
        {
            //Hot swapping
            _contentManager = new ThreadSafeContentManager(content.ServiceProvider) { RootDirectory = "Content" };
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

        public void Draw(RenderTarget2D rt, Texture2D spriteTexture, Vector2 MousePosition, int Scale, float Spring, float Dampening)
        {
            InitializeParameters();
            
            if (!_effectLoaded) return;

            if (_scale != Scale)
            {
                _scale = Scale;
                CreateParticles(Scale);
            }

            SpringConstant = Spring;
            DampeningConstant = Dampening;

            _graphicsDevice.BlendState = BlendState.Opaque;

            _mousePositionParameter.SetValue(MousePosition);

            Sprite = spriteTexture;

            //Physics

            _graphicsDevice.SetRenderTarget(offframe ? _textureBufferB :  _textureBufferA);
            _texBufferParameter.SetValue(offframe ? _textureBufferA : _textureBufferB);
            _physicsPass.Apply();
            _fsqRenderer.RenderFullscreenQuad(_graphicsDevice);

            //Vector4[] v4 = new Vector4[1];
            //_textureBufferA.GetData(v4);

            //DebugScreen.AddString(v4[0].ToString());
            
            _graphicsDevice.SetRenderTarget(rt);
            //1280 800

            _graphicsDevice.Clear(Color.Black);
            _graphicsDevice.BlendState = BlendState.Additive;

            _texBufferParameter.SetValue(offframe ? _textureBufferB : _textureBufferA);

            _graphicsDevice.SetVertexBuffer(_vertexBuffer);
            _graphicsDevice.Indices = _indexBuffer;
            
            _defaultPass.Apply();

            _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primitiveCount);

            offframe = !offframe;
        }

        private void CreateBuffers(int w, int h, bool fullPrecision)
        {
            if (_textureBufferA != null)
            {
                _textureBufferA.Dispose();
                _textureBufferB.Dispose();
            }
            
            _textureBufferA = new RenderTarget2D(_graphicsDevice, w, h, false, fullPrecision ?  SurfaceFormat.Vector4 : SurfaceFormat.HalfVector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _textureBufferB = new RenderTarget2D(_graphicsDevice, w, h, false, fullPrecision ? SurfaceFormat.Vector4 : SurfaceFormat.HalfVector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            _graphicsDevice.SetRenderTarget(_textureBufferA);
            _physicsInitializePass.Apply();
            _fsqRenderer.RenderFullscreenQuad(_graphicsDevice);
            _graphicsDevice.SetRenderTarget(_textureBufferB);
            _physicsInitializePass.Apply();
            _fsqRenderer.RenderFullscreenQuad(_graphicsDevice);

            _texBufferSizeParameter.SetValue(new Vector2(w,h));
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

            _texBufferParameter = shaderEffect.Parameters["TexBuffer"];
            _spriteParameter = shaderEffect.Parameters["Sprite"];
            _texBufferSizeParameter = shaderEffect.Parameters["TexBufferSize"];
            _mousePositionParameter = shaderEffect.Parameters["MousePosition"];
            _sizeParameter = shaderEffect.Parameters["Size"];
            _springConstantParameter = shaderEffect.Parameters["SpringConstant"];
            _dampeningConstantParameter = shaderEffect.Parameters["DampeningConstant"];

            _defaultPass = shaderEffect.Techniques["Default"].Passes[0];
            _physicsPass = shaderEffect.Techniques["Physics"].Passes[0];
            _physicsInitializePass = shaderEffect.Techniques["PhysicsInitialize"].Passes[0];


            CreateParticles(1);
            //Signal we have loaded the effect
            _effectLoaded = true;

        }

        private void CreateParticles(int scale)
        {
            int w = 1280 / scale;
            int h = 800 / scale;
            CreateVertexBuffer(w, h);

            CreateBuffers(w, h, false);

            _texBufferParameter.SetValue(_textureBufferA);
            _sizeParameter.SetValue(0.0006f * scale);
        }

        private void CreateVertexBuffer(int w, int h)
        {
            float x = w;
            float y = h;

            int amount = (int) (x * y);

            ParticleVertex[] vBuffer = new ParticleVertex[amount * 4];
            int[] indices = new int[amount*6];

            if (_vertexBuffer != null)
            {
                _vertexBuffer.Dispose();
                _indexBuffer.Dispose();
            }

            _vertexBuffer = new VertexBuffer(_graphicsDevice, ParticleVertex.VertexDeclaration, vBuffer.Length, BufferUsage.WriteOnly);
            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);

            int indexIndex = 0;
            int vertexIndex = 0;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++, indexIndex+=6, vertexIndex+=4)
                {
                    float xpos = (float)i/x + 0.5f/x;
                    float ypos = (float)j / y + 0.5f / y;

                    position.X = xpos * 2 - 1;
                    position.Y = ypos*2 - 1;

                    for (int z = 0; z < 4; z++)
                    {
                        vBuffer[z + vertexIndex] = new ParticleVertex(position, z);
                    }

                    indices[0 + indexIndex] = 0 + vertexIndex;
                    indices[1 + indexIndex] = 3 + vertexIndex;
                    indices[2 + indexIndex] = 2 + vertexIndex;
                    indices[3 + indexIndex] = 0 + vertexIndex;
                    indices[4 + indexIndex] = 1 + vertexIndex;
                    indices[5 + indexIndex] = 3 + vertexIndex;
                    
                }
            }

            _primitiveCount = amount*2;

            _vertexBuffer.SetData(vBuffer);
            _indexBuffer.SetData(indices);
        }

        public Texture2D GetTexBuffer()
        {
            return _textureBufferA;
            
        }
    }
    public struct ParticleVertex
    {
        // Stores the starting position of the particle.
        public Vector3 Position;
        
        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
          new VertexElement(0, VertexElementFormat.Vector3,
                                 VertexElementUsage.Position, 0)
        );

        public ParticleVertex(Vector2 position, float edge)
        {
            Position = new Vector3(position, edge);
        }
        
        public const int SizeInBytes = 12;
    }


}
