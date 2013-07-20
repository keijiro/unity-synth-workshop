using UnityEngine;
using System.Collections;

public class PRCReverbFilter : MonoBehaviour
{
    // T60 decay time.
    [Range(0.0f, 4.0f)]
    public float
        decayTime = 1.0f;
    
    // Send/return level.
    [Range(0.0f, 1.0f)]
    public float
        sendLevel = 0.1f;

    // STK PRCReverb filter.
    Stk.PRCReverb reverb;

    // Used for detecting parameter changes.
    float prevDecayTime;

    // Used for error handling.
    string error;

    void Awake ()
    {
        reverb = new Stk.PRCReverb (decayTime);
        prevDecayTime = decayTime;
    }

    void Update ()
    {
        if (error == null) {
            if (decayTime != prevDecayTime) {
                reverb.DecayTime = decayTime;
                prevDecayTime = decayTime;
            }
        } else {
            Debug.LogError (error);
            Destroy (this);
        }
    }

    void OnAudioFilterRead (float[] data, int channels)
    {
        if (channels != 2) {
            error = "This filter only supports stereo audio (given:" + channels + ")";
            return;
        }
        for (var i = 0; i < data.Length; i += 2) {
            var output = reverb.Tick (0.2f * (data [i] + data [i + 1]));
            data [i] += output.left * sendLevel;
            data [i + 1] += output.right * sendLevel;
        }
    }
}
