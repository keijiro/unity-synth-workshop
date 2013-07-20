using UnityEngine;
using System.Collections;

public class StkChorusFilter : MonoBehaviour
{
    // Base delay time.
    [Range(0.0f, 0.3f)]
    public float
        baseDelay = 0.1f;

    // Modulation depth.
    [Range(0.0f, 1.0f)]
    public float
        depth = 0.1f;

    // Modulation frequency.
    [Range(0.0f, 10.0f)]
    public float
        frequency = 0.2f;

    // Wet signal ratio.
    [Range(0.0f, 1.0f)]
    public float
        wetMix = 0.5f;

    // STK chorus filter.
    Stk.Chorus chorus;

    // Used for detecting parameter changes.
    private float prevBaseDelay;

    // Used for error handling.
    string error;

    void UpdateParameters ()
    {
        if (baseDelay != prevBaseDelay) {
            chorus.ResetBaseDelay (baseDelay);
            prevBaseDelay = baseDelay;
        }
        chorus.Depth = depth;
        chorus.Frequency = frequency;
        chorus.WetMix = wetMix;
    }

    void Awake ()
    {
        chorus = new Stk.Chorus (baseDelay);
        prevBaseDelay = baseDelay;
        UpdateParameters ();
    }

    void Update ()
    {
        if (error == null) {
            UpdateParameters ();
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
            var output = chorus.Tick (new Stk.StereoFrame (data [i], data [i + 1]));
            data [i] = output.left;
            data [i + 1] = output.right;
        }
    }
}
