namespace Raina.Shared.Tweens
{
    public class StaticTween : BaseTween
    {
        private float _output;

        public StaticTween(float output, float duration)
        {
            _output = output;
            base.duration = duration;
        }
        
        public override float output()
        {
            return _output;
        }

        public override float output_at(float time)
        {
            return _output;
        }

        public override bool done()
        {
            return Environment.TickCount - lastActivation > duration;
        }
    }
}