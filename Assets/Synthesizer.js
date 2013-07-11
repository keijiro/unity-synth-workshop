#pragma strict

var freq = 440.0;

private var tick = 0;

function OnAudioFilterRead(data : float[], channels : int) {
    for (var i = 0; i < data.Length; i += 2) {
    	tick++;
    	var value = Mathf.Sin(freq * Mathf.PI * 2 * 12 * tick / 44100.0);
    	value = Mathf.Sin(freq * Mathf.PI * 2 * tick / 44100.0 + 0.3 * value);
        data[i  ] = value;
        data[i+1] = value;
    }
}
