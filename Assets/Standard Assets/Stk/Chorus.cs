// STK chorus effect class, based on CCRMA STK library.
// https://ccrma.stanford.edu/software/stk/
//
// The Synthesis ToolKit in C++ (STK)
// Copyright (c) 1995-2012 Perry R. Cook and Gary P. Scavone
//
// Ported by Keijiro Takahashi, 2013.

namespace Stk
{
    public class Chorus
    {
        // Delay lines.
        Stk.DelayLine2 delay1;
        Stk.DelayLine2 delay2;
        float baseDelay;

        // Modulations.
        Stk.SineWave mod1;
        Stk.SineWave mod2;
        float modDepth;
        
        // Wet/Dry mix ratio.
        float wetMix;
        float dryMix;

        // Modulation frequency.
        public float Frequency {
            set {
                mod1.Frequency = value;
                mod2.Frequency = value * 1.1111f;
            }
        }

        // Modulation depth.
        public float Depth {
            set { modDepth = value; }
        }

        // Wet signal ratio.
        public float WetMix {
            set {
                wetMix = value;
                dryMix = 1.0f - value;
            }
        }

        // Constructor.
        public Chorus (float baseDelay = 0.1f)
        {
            ResetBaseDelay (baseDelay);
            mod1 = new Stk.SineWave ();
            mod2 = new Stk.SineWave ();
            Frequency = 0.2f;
            wetMix = 0.5f;
        }

        // Reset the base delay time.
        public void ResetBaseDelay (float baseDelay)
        {
            this.baseDelay = baseDelay;
            float maxDelay = baseDelay * 1.414f;
            delay1 = new Stk.DelayLine2 (baseDelay, maxDelay);
            delay2 = new Stk.DelayLine2 (baseDelay, maxDelay);
        }

        // Tick function.
        public StereoFrame Tick (StereoFrame input)
        {
            delay1.Delay = baseDelay * (1.0f + modDepth * mod1.Tick ()) * 0.707f;
            delay2.Delay = baseDelay * (1.0f + modDepth * mod2.Tick ()) * 0.5f;
            var mono = input.Mono;
            return new StereoFrame (
                wetMix * delay1.Tick (mono) + dryMix * input.left,
                wetMix * delay2.Tick (mono) + dryMix * input.right
            );
        }
    }
}