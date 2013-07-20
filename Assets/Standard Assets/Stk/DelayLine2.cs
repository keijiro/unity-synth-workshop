// Time-based interpolated delay line class.
// by Keijiro Takahashi, 2013.

namespace Stk
{
    public class DelayLine2
    {
        float[] buffer;
        int inPoint;
        float outPoint;

        public float Delay {
            set {
                outPoint = inPoint - value * Config.SampleRate;
                if (outPoint < 0.0f) {
                    outPoint += buffer.Length;
                    if (outPoint < 0.0f)
                        throw new System.ArgumentOutOfRangeException ("Must be less than maxDelay.");
                }
            }
        }
    
        public DelayLine2 (float delay, float maxDelay = 0.2f)
        {
            buffer = new float[(int)(maxDelay * Config.SampleRate) + 2];
            Delay = delay;
        }
    
        public float Tick (float input)
        {
            buffer [inPoint++] = input;
            if (inPoint == buffer.Length) {
                inPoint = 0;
            }

            int index1 = (int)outPoint;
            float frac = outPoint - index1;

            outPoint += 1.0f;
            if (outPoint >= buffer.Length) {
                outPoint -= buffer.Length;
            }

            int index2 = (int)outPoint;

            return buffer [index1] * (1.0f - frac) + buffer [index2] * frac;
        }
    }
}
