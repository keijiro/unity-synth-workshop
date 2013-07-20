namespace Stk
{
    public class Config
    {
        static int sampleRate = 44100;

        static public int SampleRate {
            get { return sampleRate; }
        }

        static public void Initialize (int sampleRate)
        {
            Config.sampleRate = sampleRate;
        }
    }
}
