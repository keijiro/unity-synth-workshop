// Basic delay line class.
// by Keijiro Takahashi, 2013.

namespace Stk
{
    public class DelayLine
    {
        float[] buffer;
        float lastOut;
        int inPoint;
        int outPoint;
        int delay;

        public int Delay {
            get { return delay; }
            set {
                if (delay + 1 > buffer.Length)
                    throw new System.ArgumentOutOfRangeException ("Must be less than maxDelay.");

                delay = value;

                outPoint = inPoint - delay;
                if (outPoint < 0)
                    outPoint += buffer.Length;
            }
        }
    
        public float LastOut {
            get { return lastOut; }
        }

        public float NextOut {
            get { return buffer [outPoint]; }
        }
    
        public DelayLine (int delay, int maxDelay = 4095)
        {
            buffer = new float[maxDelay + 1];
            Delay = delay;
        }
    
        public float Tick (float input)
        {
            buffer [inPoint++] = input;
            if (inPoint == buffer.Length) {
                inPoint = 0;
            }
            
            lastOut = buffer [outPoint++];
            if (outPoint == buffer.Length) {
                outPoint = 0;
            }
            
            return lastOut;
        }
    }
}
