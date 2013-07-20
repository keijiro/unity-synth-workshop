// CCRMA's NRev reverberator class, based on CCRMA STK library.
// https://ccrma.stanford.edu/software/stk/
//
// The Synthesis ToolKit in C++ (STK)
// Copyright (c) 1995-2012 Perry R. Cook and Gary P. Scavone
//
// Ported by Keijiro Takahashi, 2013.

namespace Stk
{
    public class NReverb
    {
        // All-pass filters.
        DelayLine[] allpassLines;

        // Comb filters.
        DelayLine[] combLines;
        float[] combCoeffs;
        
        // One-pole low-pass filter.
        float lowpassState;

        // T60 decay time.
        public float DecayTime {
            set {
                float scaler = -3.0f / (value * Config.SampleRate);
                for (var i = 0; i < 6; i++) {
                    combCoeffs [i] = (float)System.Math.Pow (10.0, scaler * combLines [i].Delay);
                }
            }
        }

        // Constructor.
        public NReverb (float decayTime)
        {
            allpassLines = new DelayLine[6];
            combLines = new DelayLine[6];
            combCoeffs = new float[6];
            
            int[] delays = {
                1433, 1601, 1867, 2053, 2251, 2399,
                347, 113, 37, 59, 53, 43
            };
            
            var scaler = Config.SampleRate / 25641.0f;
            for (var i = 0; i < delays.Length; i++) {
                var delay = (int)(scaler * delays [i]);
                if ((delay & 1) == 0)
                    delay++;
                while (!Math.IsPrime(delay))
                    delay += 2;
                delays [i] = delay;
            }
            
            for (var i = 0; i < 6; i++) {
                combLines [i] = new DelayLine (delays [i], delays [i]);
            }
            
            for (var i = 0; i < 6; i++) {
                allpassLines [i] = new DelayLine (delays [i + 6], delays [i + 6]);
            }

            DecayTime = decayTime;
        }

        // Tick function.
        public StereoFrame Tick (float input)
        {
            const float AllpassCoeff = 0.7f;

            var temp0 = 0.0f;
            for (var i = 0; i < 6; i++) {
                temp0 += combLines [i].Tick (input + combCoeffs [i] * combLines [i].NextOut);
            }
            
            for (var i = 0; i < 3; i++) {
                var temp1 = temp0 + AllpassCoeff * allpassLines [i].NextOut;
                temp0 = allpassLines [i].Tick (temp1) - AllpassCoeff * temp1;
            }
            
            // One-pole lowpass filter.
            lowpassState = 0.7f * lowpassState + 0.3f * temp0;
            var temp2 = lowpassState + AllpassCoeff * allpassLines [3].NextOut;
            temp2 = allpassLines [3].Tick (temp2) - AllpassCoeff * temp2;
            
            var out1 = temp2 + AllpassCoeff * allpassLines [4].NextOut;
            var out2 = temp2 + AllpassCoeff * allpassLines [5].NextOut;
            
            out1 = allpassLines [4].Tick (out1) - AllpassCoeff * out1;
            out2 = allpassLines [5].Tick (out2) - AllpassCoeff * out2;

            return new StereoFrame (out1, out2);
        }
    }
}