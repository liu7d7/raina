using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Raina.Shared.Components
{
    public class Camera : RainaObj.Component
    {
        public Vector3 front;
        public Vector3 right;
        public Vector3 up;
        private FloatPos _pos;

        public Camera()
        {
            front = Vector3.Zero;
            right = Vector3.Zero;
            up = Vector3.UnitY;
            _lastX = 0;
        }

        private void update_camera_vectors()
        {
            front = new Vector3(MathF.Cos(_pos.pitch.to_radians()) * MathF.Cos(_pos.yaw.to_radians()), MathF.Sin(_pos.pitch.to_radians()), MathF.Cos(_pos.pitch.to_radians()) * MathF.Sin(_pos.yaw.to_radians())).Normalized();
            right = Vector3.Cross(front, up).Normalized();
        }

        public override void update(RainaObj objIn)
        {
            base.update(objIn);

            _pos ??= objIn.get<FloatPos>();

            update_camera_vectors();

            _pos.set_prev();

            int forwards = 0;
            int rightwards = 0;
            int upwards = 0;
            KeyboardState kb = Raina.instance.KeyboardState;
            if (kb.IsKeyDown(Keys.W)) forwards++;
            if (kb.IsKeyDown(Keys.S)) forwards--;
            if (kb.IsKeyDown(Keys.A)) rightwards--;
            if (kb.IsKeyDown(Keys.D)) rightwards++;
            if (kb.IsKeyDown(Keys.Space)) upwards++;
            if (kb.IsKeyDown(Keys.LeftShift)) upwards--;
            Vector3 current = _pos.to_vector3();
            current += front * forwards;
            current += right * rightwards;
            current += up * upwards;
            _pos.set_vector3(current);
        }

        public bool firstMouse = true;
        private float _lastX;
        private float _lastY;

        public void on_mouse_move(MouseMoveEventArgs e)
        {
            if (Raina.instance.CursorState != CursorState.Grabbed || !Raina.instance.IsFocused) return;
            float xPos = e.X;
            float yPos = e.Y;

            if (firstMouse)
            {
                _lastX = xPos;
                _lastY = yPos;
                firstMouse = false;
            }
            
            float xOffset = xPos - _lastX;
            float yOffset = _lastY - yPos;
            _lastX = xPos;
            _lastY = yPos;
            
            const float sensitivity = 0.1f;
            xOffset *= sensitivity;
            yOffset *= sensitivity;
            
            _pos.yaw += xOffset;
            _pos.pitch += yOffset;
            
            if (_pos.pitch > 89.0f)
                _pos.pitch = 89.0f;
            if (_pos.pitch < -89.0f)
                _pos.pitch = -89.0f;
            
            update_camera_vectors();
        }

        public Matrix4 get_camera_matrix() 
        {
            if (_pos == null)
            {
                return Matrix4.Identity;
            }
            Vector3 pos = new(_pos.lerped_x, _pos.lerped_y, _pos.lerped_z);
            Matrix4 lookAt = Matrix4.LookAt(pos - front, pos, up);
            return lookAt;
        }
    }
}