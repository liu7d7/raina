namespace Raina.Shared
{
    public sealed class Fmt
    {
        
        public static readonly Dictionary<char, Fmt> values = new();

        public static readonly Fmt black = new(0, '0');
        public static readonly Fmt darkblue = new(0xff0000aa, '1');
        public static readonly Fmt darkgreen = new(0xff00aa00, '2');
        public static readonly Fmt darkcyan = new(0xff00aaaa, '3');
        public static readonly Fmt darkred = new(0xffaa0000, '4');
        public static readonly Fmt darkpurple = new(0xffaa00aa, '5');
        public static readonly Fmt gold = new(0xffffaa00, '6');
        public static readonly Fmt gray = new(0xffaaaaaa, '7');
        public static readonly Fmt darkgray = new(0xff555555, '8');
        public static readonly Fmt blue = new(0xff5555ff, '9');
        public static readonly Fmt green = new(0xff55ff55, 'a');
        public static readonly Fmt cyan = new(0xff55ffff, 'b');
        public static readonly Fmt red = new(0xffff5555, 'c');
        public static readonly Fmt purple = new(0xffff55ff, 'd');
        public static readonly Fmt yellow = new(0xffffff55, 'e');
        public static readonly Fmt white = new(0xffffffff, 'f');
        public static readonly Fmt reset = new(0, 'r');

        public readonly uint color;
        private readonly uint _code;

        private Fmt(uint color, char code)
        {
            this.color = color;
            _code = code;
            values[code] = this;
        }

        public override string ToString()
        {
            return $"\u00a7{_code}";
        }
    }
}