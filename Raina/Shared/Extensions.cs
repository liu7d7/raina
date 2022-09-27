using System.Reflection;
using System.Text;
using OpenTK.Mathematics;

namespace Raina.Shared
{
    public static class Maps
    {
        public static Dictionary<Tk, Tv> create<Tk, Tv>(params KeyValuePair<Tk, Tv>[] pairs)
        {
            return pairs.ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }

    public static class Colors
    {
        private static bool _initialized;
        private static readonly Dictionary<string, Color4> values = new();

        public static Color4 get_color4(string color)
        {
            if (_initialized) return values[color.ToLower()];
            foreach (PropertyInfo john in typeof(Color4).GetProperties())
            {
                object val = john.GetValue(null);
                if (val == null)
                {
                    continue;
                }
                    
                if (val.get_type() == typeof(Color4))
                {
                    values.Add(john.Name.ToLower(), (Color4) val);
                }
            }

            _initialized = true;
            return values[color.ToLower()];
        }
        
        public static Color4 get_random_color4()
        {
            if (!_initialized) get_color4("white");
            return values.Values.ElementAt(new Random().Next(values.Count));
        }
    }

    public static class Extensions
    {
        public static string content_to_string<T>(this T[] arr)
        {
            string o = arr.to_string() ?? string.Empty;
            o = o[..^1];
            foreach (T item in arr)
            {
                o += item + ", ";
            }
            o = o[..^2];
            o += "]";
            return o;
        }

        public static string content_to_string<T, Tv>(this Dictionary<T, Tv> arr)
        {
            StringBuilder o = new("Map<");
            o.Append(typeof(T));
            o.Append(", ");
            o.Append(typeof(Tv));
            o.Append(">(");
            foreach (var item in arr)
            {
                o.Append('(');
                o.Append(item.Key);
                o.Append(", ");
                o.Append(item.Value);
                o.Append("), ");
            }
            if (arr.Count > 0)
            {
                o.Remove(o.Length - 3, 3);
            }
            o.Append(')');
            return o.to_string();
        }

        public static void transform(this ref Vector4 vec, Matrix4 m4F)
        {
            float f = vec.X;
            float g = vec.Y;
            float h = vec.Z;
            float i = vec.W;
            vec.X = MathF.FusedMultiplyAdd(m4F.M11, f, MathF.FusedMultiplyAdd(m4F.M21, g, MathF.FusedMultiplyAdd(m4F.M31, h, m4F.M41 + i)));
            vec.Y = MathF.FusedMultiplyAdd(m4F.M12, f, MathF.FusedMultiplyAdd(m4F.M22, g, MathF.FusedMultiplyAdd(m4F.M32, h, m4F.M42 + i)));
            vec.Z = MathF.FusedMultiplyAdd(m4F.M13, f, MathF.FusedMultiplyAdd(m4F.M23, g, MathF.FusedMultiplyAdd(m4F.M33, h, m4F.M43 + i)));
            vec.W = MathF.FusedMultiplyAdd(m4F.M14, f, MathF.FusedMultiplyAdd(m4F.M24, g, MathF.FusedMultiplyAdd(m4F.M34, h, m4F.M44 + i)));
        }

        public static Vector2 normalized_fast(this ref Vector2 vec)
        {
            if (vec == Vector2.Zero)
            {
                return vec;
            }
            var length = MathHelper.InverseSqrtFast((float) (vec.X * (double) vec.X + vec.Y * (double) vec.Y));
            vec.X *= length;
            vec.Y *= length;
            return vec;
        }

        public static Vector3 normalized_fast(this ref Vector3 vec)
        {
            if (vec == Vector3.Zero)
            {
                return vec;
            }
            var length = MathHelper.InverseSqrtFast((float) (vec.X * (double) vec.X + vec.Y * (double) vec.Y + vec.Z * (double) vec.Z));
            vec.X *= length;
            vec.Y *= length;
            vec.Z *= length;
            return vec;
        }
        
        public static float to_radians(this float degrees)
        {
            return (float) (degrees * Math.PI / 180.0);
        }
        
        public static float to_degrees(this float radians)
        {
            return (float) (radians * 180 / Math.PI);
        }

        public static void scale(this ref Matrix4 matrix4, float scalar)
        {
            matrix4 *= Matrix4.CreateScale(scalar);
        }
        
        public static void scale(this ref Matrix4 matrix4, Vector3 scalar)
        {
            matrix4 *= Matrix4.CreateScale(scalar);
        }
        
        public static void scale(this ref Matrix4 matrix4, float x, float y, float z)
        {
            matrix4 *= Matrix4.CreateScale(x, y, z);
        }
        
        public static void translate(this ref Matrix4 matrix4, Vector3 translation)
        {
            matrix4 *= Matrix4.CreateTranslation(translation);
        }
        
        public static void translate(this ref Matrix4 matrix4, float x, float y, float z)
        {
            matrix4 *= Matrix4.CreateTranslation(x, y, z);
        }
        
        public static void rotate(this ref Matrix4 matrix4, float angle, Vector3 axis)
        {
            matrix4 *= Matrix4.CreateFromAxisAngle(axis, angle / 180f * MathF.PI);
        }
        
        public static void rotate(this ref Matrix4 matrix4, float angle, float x, float y, float z)
        {
            matrix4 *= Matrix4.CreateFromAxisAngle(new Vector3(x, y, z), angle / 180f * MathF.PI);
        }

        public static void set(this ref Matrix4 mat, Matrix4 other)
        {
            mat.M11 = other.M11;
            mat.M12 = other.M12;
            mat.M13 = other.M13;
            mat.M14 = other.M14;
            mat.M21 = other.M21;
            mat.M22 = other.M22;
            mat.M23 = other.M23;
            mat.M24 = other.M24;
            mat.M31 = other.M31;
            mat.M32 = other.M32;
            mat.M33 = other.M33;
            mat.M34 = other.M34;
            mat.M41 = other.M41;
            mat.M42 = other.M42;
            mat.M43 = other.M43;
            mat.M44 = other.M44;
        }

        public static string to_string(this object obj)
        {
            return obj.ToString();
        }
        
        public static Type get_type(this object obj)
        {
            return obj.GetType();
        }
        
        public static int get_hash_code(this object obj)
        {
            return obj.GetHashCode();
        }
        
        public static bool equals(this object obj, object other)
        {
            return obj.Equals(other);
        }

        public static int distance_sq(this Vector2i vec, Vector2i other)
        {
            (int x, int y) = vec;
            (int i, int i1) = other;
            return (x - i) * (x - i) + (y - i1) * (y - i1);
        }

        public static float next_float(this Random random)
        {
            return (float)random.NextDouble();
        }
    }
}