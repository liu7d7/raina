using System.ComponentModel;
using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Raina.Engine;
using Raina.Shared;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Raina.Shared.Components;

namespace Raina
{
    public class Raina : GameWindow
    {
        private static int _ticks;
        public static Raina instance;
        public static Model3d model;
        public static RainaObj camera;

        public Raina(GameWindowSettings windowSettings, NativeWindowSettings nativeWindowSettings) : base(windowSettings, nativeWindowSettings)
        {
            instance = this;
            model = Model3d.read("body", new Dictionary<string, uint>());
            camera = new RainaObj();
            camera.add(new Camera());
            camera.add(new FloatPos());
            FloatPos pos = camera.get<FloatPos>();
            pos.yaw = 25;
            pos.x = pos.prevX = pos.z = pos.prevZ = -1;
            pos.y = pos.prevY = 25;
            camera.update();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.DepthFunc(DepthFunction.Lequal);
            GlStateManager.enable_blend();

            Ticker.init();
            
            CursorState = CursorState.Grabbed;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            
            if (e.Size == Vector2i.Zero)
                return;

            RenderSystem.update_projection();
            GL.Viewport(new Rectangle(0, 0, Size.X, Size.Y));
            Fbo.resize(Size.X, Size.Y);
        }

        private float _lastFrame = Environment.TickCount;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            RenderSystem.frame.clear_color();
            RenderSystem.frame.clear_depth();
            RenderSystem.frame.bind();
            
            RenderSystem.update_look_at(camera);
            RenderSystem.update_projection();
            
            RenderSystem.playerTex.bind(TextureUnit.Texture0);
            RenderSystem.mesh.begin();
            model.render(RenderSystem.mesh, Vector3.Zero);
            RenderSystem.mesh.render();
            Texture.unbind();

            RenderSystem.frame.bind();

            RenderSystem.frame.blit(RenderSystem.swap.handle);
            RenderSystem.render_outline();
            
            RenderSystem.frame.bind();
            render_skybox();
            
            Fbo.unbind();

            RenderSystem.frame.blit();

            RenderSystem.update_look_at(camera, false);
            RenderSystem.update_projection();
            RenderSystem.font.bind();
            RenderSystem.renderingRed = true;
            RenderSystem.mesh.begin();
            RenderSystem.font.draw(RenderSystem.mesh, "mspf: " + (Environment.TickCount - _lastFrame), -Size.X / 2f + 11, Size.Y / 2f - 8, Color4.HotPink.to_uint(), false);
            RenderSystem.mesh.render();
            RenderSystem.renderingRed = false;
            Font.unbind();
            
            _lastFrame = Environment.TickCount;

            SwapBuffers();
        }

        private static void render_skybox()
        {
            GlStateManager.depth_mask(false);
            for (int i = 0; i < 6; i++)
            {
                Texture.bind_cubemap(RenderSystem.skyboxTex.ids[i]);
                int start = i * 4; 
                Vector3[] verts = RenderSystem.skyboxVertices;
                RenderSystem.skybox.begin();
                int i1 = RenderSystem.skybox.float3(verts[start + 0] * 10).float2(0, 0).next();
                int i2 = RenderSystem.skybox.float3(verts[start + 1] * 10).float2(1, 0).next();
                int i3 = RenderSystem.skybox.float3(verts[start + 2] * 10).float2(1, 1).next();
                int i4 = RenderSystem.skybox.float3(verts[start + 3] * 10).float2(0, 1).next();
                RenderSystem.skybox.quad(i1, i2, i3, i4);
                RenderSystem.skybox.render();
            }
            GlStateManager.depth_mask(true);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            camera.get<Camera>().on_mouse_move(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            int i = Ticker.update();

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                camera.get<Camera>().firstMouse = true;
                CursorState = CursorState.Normal;
            }

            if (MouseState.WasButtonDown(MouseButton.Left))
            {
                CursorState = CursorState.Grabbed;
            }

            for (int j = 0; j < Math.Min(10, i); j++)
            {
                _ticks++;

                camera.update();
            }
        }
    }
}