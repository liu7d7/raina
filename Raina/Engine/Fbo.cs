using OpenTK.Graphics.OpenGL4;

namespace Raina.Engine
{
    public class Fbo
    {

        private static int _active;
        private static readonly Dictionary<int, Fbo> frames = new();
        private readonly bool _useDepth;
        public int handle;
        private int _colorAttachment;
        private int _depthAttachment;
        private int _width;
        private int _height;

        public Fbo(int width, int height)
        {
            _width = width;
            _height = height;
            _useDepth = false;
            handle = -1;
            init();
            frames[handle] = this;
        }

        public Fbo(int width, int height, bool useDepth)
        {
            _width = width;
            _height = height;
            _useDepth = useDepth;
            handle = -1;
            init();
            frames[handle] = this;
        }

        private void dispose()
        {
            GL.DeleteFramebuffer(handle);
            GL.DeleteTexture(_colorAttachment);
            if (_useDepth)
            {
                GL.DeleteTexture(_depthAttachment);
            }
        }

        private void init()
        {
            if (handle != -1)
            {
                dispose();
            }

            handle = GL.GenFramebuffer();
            bind();
            _colorAttachment = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _colorAttachment);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToBorder);
            GL.TexStorage2D(TextureTarget2d.Texture2D, 1, SizedInternalFormat.Rgba8, _width, _height);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _colorAttachment, 0);
            if (_useDepth)
            {
                _depthAttachment = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, _depthAttachment);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int) TextureCompareMode.None);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, _width, _height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, _depthAttachment, 0);
            }

            FramebufferErrorCode status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception($"Incomplete Framebuffer! {status} should be {FramebufferErrorCode.FramebufferComplete}");
            }

            unbind();
        }

        private void _resize(int width, int height)
        {
            _width = width;
            _height = height;
            init();
        }

        public static void resize(int width, int height)
        {
            foreach (KeyValuePair<int, Fbo> frame in frames)
            {
                frame.Value._resize(width, height);
            }
        }

        public void bind_color(TextureUnit unit)
        {
            Texture.bind(_colorAttachment, unit);
        }
        
        public int bind_depth(TextureUnit unit)
        {
            if (!_useDepth)
            {
                throw new Exception("Trying to bind depth texture of a framebuffer without depth!");
            }
            Texture.bind(_depthAttachment, unit);
            return _depthAttachment;
        }

        public void clear_color()
        {
            bind();
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Fbo.unbind();
        }
        
        public void clear_depth()
        {
            bind();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            Fbo.unbind();
        }

        public void bind()
        {
            if (handle == _active)
            {
                return;
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
            _active = handle;
        }
        
        public void blit(int other = 0)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, handle);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, other);
            GL.BlitFramebuffer(0, 0, _width, _height, 0, 0, _width, _height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            unbind();
        }

        public static void unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            _active = 0;
        }
        
    }
}
