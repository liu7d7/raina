using OpenTK.Mathematics;
using Raina.Shared;
using Raina.Shared.Components;
using OpenTK.Graphics.OpenGL4;

namespace Raina.Engine
{
    public static class RenderSystem
    {
        private static readonly Shader john = new("Resource/Shader/john.vert", "Resource/Shader/john.frag");
        private static readonly Shader cubemap = new("Resource/Shader/cubemap.vert", "Resource/Shader/cubemap.frag");
        private static readonly Shader pixel = new("Resource/Shader/postprocess.vert", "Resource/Shader/pixelate.frag");
        private static readonly Shader fxaa = new("Resource/Shader/fxaa.vert", "Resource/Shader/fxaa.frag");

        private static readonly Shader
            outline = new("Resource/Shader/postprocess.vert", "Resource/Shader/outline.frag");

        private static Matrix4 _projection;
        private static Matrix4 _lookAt;
        private static readonly Matrix4[] _model = new Matrix4[7];
        public static Matrix4 model;
        private static int _modelIdx;
        public static bool renderingRed;
        public static readonly Font font = new(File.ReadAllBytes("Resource/Font/Dank Mono Italic.otf"), 32);
        public static readonly Texture playerTex = Texture.load_from_file("Resource/Texture/texture1.png");
        public static readonly Texture rect = Texture.load_from_file("Resource/Texture/rect.png");

        public static readonly Mesh mesh = new(Mesh.DrawMode.triangle, john, Vao.Attrib.float3, Vao.Attrib.float3,
            Vao.Attrib.float2, Vao.Attrib.float4);

        public static readonly Mesh skybox = new(Mesh.DrawMode.triangle, cubemap, Vao.Attrib.float3, Vao.Attrib.float2);

        public static readonly Mesh line = new(Mesh.DrawMode.line, john, Vao.Attrib.float3, Vao.Attrib.float3,
            Vao.Attrib.float2, Vao.Attrib.float4);

        private static readonly Mesh post = new(Mesh.DrawMode.triangle, null, Vao.Attrib.float2);
        public static readonly Fbo frame = new(size.X, size.Y, true);
        public static readonly Fbo swap = new(size.X, size.Y, true);
        public static bool rendering3d;
        private static FloatPos _camera;
        
        static RenderSystem()
        {
            Array.Fill(_model, Matrix4.Identity);
            model = _model[0];
        }

        public static void push()
        {
            _model[_modelIdx + 1] = model;
            _modelIdx++;
            model = _model[_modelIdx];
        }

        public static void pop()
        {
            _modelIdx--;
            model = _model[_modelIdx];
        }

        public static Vector2i size => Raina.instance.Size;

        public static Vector4 to_vector4(this Color4 color)
        {
            return (color.R, color.G, color.B, color.A);
        }

        public static uint to_uint(this Color4 color)
        {
            return (uint)color.ToArgb();
        }

        public static void set_defaults(this Shader shader)
        {
            shader.set_matrix4("_proj", _projection);
            shader.set_matrix4("_lookAt", _lookAt);
            shader.set_vector2("_screenSize", (size.X, size.Y));
            shader.set_int("_rendering3d", rendering3d ? 1 : 0);
            shader.set_int("doLighting", 0);
            shader.set_int("_renderingRed", renderingRed ? 1 : 0);
            shader.set_vector3("lightPos", (_camera.x + 5, _camera.y + 12, _camera.z + 5));
        }

        public static void render_pixelation(float pixWidth, float pixHeight)
        {
            frame.clear_color();
            frame.clear_depth();
            frame.bind();
            pixel.bind();
            swap.bind_color(TextureUnit.Texture0);
            pixel.set_int("_tex0", 0);
            pixel.set_vector2("_screenSize", (size.X, size.Y));
            pixel.set_vector2("_pixSize", (pixWidth, pixHeight));
            post.begin();
            int i1 = post.float2(0, 0).next();
            int i2 = post.float2(size.X, 0).next();
            int i3 = post.float2(size.X, size.Y).next();
            int i4 = post.float2(0, size.Y).next();
            post.quad(i1, i2, i3, i4);
            post.render();
            Shader.unbind();
        }

        public static void render_fxaa(Fbo fbo)
        {
            frame.clear_color();
            frame.clear_depth();
            frame.bind();
            fxaa.bind();
            swap.bind_color(TextureUnit.Texture0);
            fxaa.set_int("_tex0", 0);
            fxaa.set_vector2("_screenSize", (size.X, size.Y));
            fxaa.set_float("SpanMax", 8);
            fxaa.set_float("ReduceMul", 0.125f);
            fxaa.set_float("SubPixelShift", 0.25f);
            post.begin();
            int i1 = post.float2(0, 0).next();
            int i2 = post.float2(1, 0).next();
            int i3 = post.float2(1, 1).next();
            int i4 = post.float2(0, 1).next();
            post.quad(i1, i2, i3, i4);
            post.render();
            Shader.unbind();
        }

        private const float threshold = 0.033f;
        private const float depthThreshold = 0.8f;

        public static void render_outline()
        {
            frame.clear_color();
            frame.clear_depth();
            frame.bind();
            outline.bind();
            swap.bind_color(TextureUnit.Texture0);
            outline.set_int("_tex0", 0);
            swap.bind_depth(TextureUnit.Texture1);
            outline.set_int("_tex1", 1);
            outline.set_vector2("_screenSize", (size.X, size.Y));
            outline.set_float("_width", 0.75f);
            outline.set_int("_glow", 0);
            outline.set_int("_abs", 1);
            outline.set_float("_threshold", threshold);
            outline.set_float("_depthThreshold", depthThreshold);
            outline.set_vector4("_outlineColor", Color4.HotPink.to_vector4());
            outline.set_int("_diffDepthCol", 0);
            outline.set_int("_blackAndWhite", 1);
            outline.set_vector4("_otherColor", Color4.White.to_vector4());
            post.begin();
            int i1 = post.float2(0, 0).next();
            int i2 = post.float2(size.X, 0).next();
            int i3 = post.float2(size.X, size.Y).next();
            int i4 = post.float2(0, size.Y).next();
            post.quad(i1, i2, i3, i4);
            post.render();
            Shader.unbind();
        }

        public static void update_projection()
        {
            if (rendering3d)
            {
                Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), size.X / (float)size.Y, 0.1f, 1000,
                    out _projection);
                return;
            }
            Matrix4.CreateOrthographic(size.X, size.Y, -1000, 3000, out _projection);
        }

        public static void update_look_at(RainaObj cameraObj, bool rendering3d = true)
        {
            if (!cameraObj.has<FloatPos>())
            {
                return;
            }

            _camera = cameraObj.get<FloatPos>();
            RenderSystem.rendering3d = rendering3d;
            if (!RenderSystem.rendering3d)
            {
                _lookAt = Matrix4.Identity;
                return;
            }

            Camera comp = cameraObj.get<Camera>();
            _lookAt = comp.get_camera_matrix();
        }
    }
}