namespace Raina.Shared
{
    public sealed class Formatting
    {
        
        public static readonly Dictionary<char, Formatting> values = new();

        public static readonly Formatting black = new(0, '0');
        public static readonly Formatting darkblue = new(0xff0000aa, '1');
        public static readonly Formatting darkgreen = new(0xff00aa00, '2');
        public static readonly Formatting darkcyan = new(0xff00aaaa, '3');
        public static readonly Formatting darkred = new(0xffaa0000, '4');
        public static readonly Formatting darkpurple = new(0xffaa00aa, '5');
        public static readonly Formatting gold = new(0xffffaa00, '6');
        public static readonly Formatting gray = new(0xffaaaaaa, '7');
        public static readonly Formatting darkgray = new(0xff555555, '8');
        public static readonly Formatting blue = new(0xff5555ff, '9');
        public static readonly Formatting green = new(0xff55ff55, 'a');
        public static readonly Formatting cyan = new(0xff55ffff, 'b');
        public static readonly Formatting red = new(0xffff5555, 'c');
        public static readonly Formatting purple = new(0xffff55ff, 'd');
        public static readonly Formatting yellow = new(0xffffff55, 'e');
        public static readonly Formatting white = new(0xffffffff, 'f');
        public static readonly Formatting reset = new(0, 'r');

        public readonly uint color;
        private readonly uint _code;

        private Formatting(uint color, char code)
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