using OpenTK.Mathematics;

namespace Raina.Shared.Tweens
{
    public class FromToTween : BaseTween
    {
        public Animations.Animation animation;
        public float from;
        public float to;

        public FromToTween(Animations.Animation animation, float from, float to, float duration)
        {
            this.animation = animation;
            lastActivation = Environment.TickCount;
            this.from = from;
            this.to = to;
            base.duration = duration;
        }

        public override float output()
        {
            if (Environment.TickCount < lastActivation)
            {
                return from;
            }

            if (Environment.TickCount > lastActivation + duration)
            {
                return to;
            }
            
            return MathHelper.Lerp(from, to, animation(duration, Environment.TickCount - lastActivation));
        }

        public override float output_at(float time)
        {
            if (time < lastActivation)
            {
                return from;
            }

            if (time > lastActivation + duration)
            {
                return to;
            }

            return MathHelper.Lerp(from, to, animation(duration, time - lastActivation));
        }

        public override bool done()
        {
            return Environment.TickCount - lastActivation > duration;
        }
    }
}