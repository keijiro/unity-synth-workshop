#pragma strict

var freq = 440.0;
var semitone = 0;

var attack = 0.1;
var release = 0.5;

var volume = 0.0;
var keyDown = false;

private var tick = 0;

function OnAudioFilterRead(data : float[], channels : int) {
	freq = 440.0 * Mathf.Pow(2.0, (semitone + 3) / 12.0);

    for (var i = 0; i < data.Length; i += 2) {
    	if (keyDown) {
    		volume = Mathf.Clamp01(volume + 1.0 / 44100.0 / attack);
    	} else {
    		volume = Mathf.Clamp01(volume - 1.0 / 44100.0 / release);
    	}
    
    	tick++;
    	var value = Mathf.Sin(freq * Mathf.PI * 2 * 12 * tick / 44100.0);
    	value = Mathf.Sin(freq * Mathf.PI * 2 * tick / 44100.0 + 0.3 * value) * volume * 0.9;
        data[i  ] = value;
        data[i+1] = value;
    }
}

private var keys = ['a', 'w', 's', 'e', 'd', 'f', 't', 'g', 'y', 'h', 'u', 'j', 'k', 'o', 'l', 'p', ';'];

function Update() {
	keyDown = false;
	for (var i = 0; i < keys.Length; i++) {
		if (Input.GetKey(keys[i])) {
			if (semitone != i) volume = 0.0;
			semitone = i;
			keyDown = true;
			break;
		}
	}
}
