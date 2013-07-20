// Perry's simple reverberator class, based on CCRMA STK library.
// https://ccrma.stanford.edu/software/stk/
//
// The Synthesis ToolKit in C++ (STK)
// Copyright (c) 1995-2012 Perry R. Cook and Gary P. Scavone
//
// Ported by Keijiro Takahashi, 2013.

namespace Stk
{
    public class PRCReverb
    {
        // All-pass filters.
        DelayLine allpassLine1;
        DelayLine allpassLine2;

        // Comb filters.
        DelayLine combLine1;
        DelayLine combLine2;
        float combCoeff1;
        float combCoeff2;

        // T60 decay time.
        public float DecayTime {
            set {
                float scaler = -3.0f / (value * Config.SampleRate);
                combCoeff1 = (float)System.Math.Pow (10.0, scaler * combLine1.Delay);
                combCoeff2 = (float)System.Math.Pow (10.0, scaler * combLine2.Delay);
            }
        }

        // Constructor.
        public PRCReverb (float decayTime)
        {
            // Delay length for 44100 Hz sample rate.
            int[] delays = { 341, 613, 1557, 2137 };
            
            // Scale the delay lengths if necessary.
            if (Config.SampleRate != 44100) {
                var scaler = Config.SampleRate / 44100.0f;
                for (var i = 0; i < delays.Length; i++) {
                    var delay = (int)(scaler * delays [i]);
                    if ((delay & 1) == 0)
                        delay++;
                    while (!Math.IsPrime(delay))
                        delay += 2;
                    delays [i] = delay;
                }
            }
            
            allpassLine1 = new DelayLine (delays [0]);
            allpassLine2 = new DelayLine (delays [1]);
            combLine1 = new DelayLine (delays [2]);
            combLine2 = new DelayLine (delays [3]);

            DecayTime = decayTime;
        }

        // Tick function.
        public StereoFrame Tick (float input)
        {
            const float AllpassCoeff = 0.7f;

            var temp0 = AllpassCoeff * allpassLine1.NextOut;
            temp0 += input;
            temp0 = allpassLine1.Tick (temp0) - AllpassCoeff * temp0;
            
            var temp1 = AllpassCoeff * allpassLine2.NextOut;
            temp1 += temp0;
            temp1 = allpassLine2.Tick (temp1) - AllpassCoeff * temp1;
            
            var out1 = combLine1.Tick (temp1 + combCoeff1 * combLine1.NextOut);
            var out2 = combLine2.Tick (temp1 + combCoeff2 * combLine2.NextOut);

            return new StereoFrame (out1, out2);
        }
    }
}