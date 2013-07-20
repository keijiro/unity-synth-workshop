// Not-so-memory-efficient sine wave class.
// by Keijiro Takahashi, 2013.

namespace Stk
{
    public class SineWave
    {
        const int TableSize = 1024; // must be a power of 2.
        const float Pi2 = 6.28318530718f;
        static float[] table;
        float time01;
        float delta;

        static void InitializeTable ()
        {
            table = new float[TableSize];
            for (var i = 0; i < TableSize; i++) {
                table [i] = (float)System.Math.Sin (i * Pi2 / TableSize);
            }
        }

        static float LookUpTable (float time01)
        {
            time01 *= TableSize;
            var i1 = (int)time01;
            var i2 = (i1 + 1) & (TableSize - 1);
            float fraction = time01 - i1;
            return 0.5f * (table [i1] * (1.0f - fraction) + table [i2] * fraction);
        }
        
        public SineWave ()
        {
            if (table == null)
                InitializeTable ();
        }

        public SineWave (float frequency)
        {
            if (table == null)
                InitializeTable ();
            Frequency = frequency;
        }

        public float Frequency {
            set { delta = value / Config.SampleRate; }
        }

        public float Tick ()
        {
            float output = LookUpTable (time01);
            time01 += delta;
            time01 -= (int)time01;
            return output;
        }
    }
}
