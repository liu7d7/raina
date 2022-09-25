using OpenTK.Mathematics;

namespace Raina.Shared.Components
{
    public class FloatPos : RainaObj.Component
    {
        public float x;
        public float prevX;
        public float y;
        public float prevY;
        public float z;
        public float prevZ;
        public float yaw;
        public float prevYaw;
        public float pitch;
        public float prevPitch;
        public float lerped_x => Util.lerp(prevX, x, Ticker.tickDelta);
        public float lerped_y => Util.lerp(prevY, y, Ticker.tickDelta);
        public float lerped_z => Util.lerp(prevZ, z, Ticker.tickDelta);

        public FloatPos()
        {
            x = prevX = y = prevY = z = prevZ = yaw = prevYaw = pitch = prevPitch = 0;
        }

        public Vector3 to_vector3()
        {
            return (x, y, z);
        }
        
        public void set_vector3(Vector3 pos)
        {
            (x, y, z) = pos;
        }

        public Vector3 to_lerped_vector3(float xOff, float yOff, float zOff)
        {
            return (lerped_x + xOff, lerped_y + yOff, lerped_z + zOff);
        }

        public void set_prev()
        {
            prevX = x;
            prevY = y;
            prevZ = z;
            prevYaw = yaw;
            prevPitch = pitch;
        }
    }
}