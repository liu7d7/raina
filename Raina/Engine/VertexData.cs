using OpenTK.Mathematics;

namespace Raina.Engine
{
    public class VertexData
    {
        public Vector3 pos;
        public Vector3 normal;
        public Vector2 uv;
        public uint color = 0xffffffff;
        
        public VertexData(Vector3 pos, Vector2 uv)
        {
            this.pos = pos;
            this.uv = uv;
        }
    }
}