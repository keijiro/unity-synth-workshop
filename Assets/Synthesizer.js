#pragma strict

var freq = 440.0;
var semitone = 0;
var modulation = 0.2;
var octave = 0;

private var sampleRate = 44100.0;
private var phase = 0.0;

var attack = 0.1;
var release = 0.5;

private var envelope = 0.0;
private var noteOn = false;

function UpdateSynth() {
	freq = 440.0 * Mathf.Pow(2.0, (semitone - 9 + octave * 12) / 12.0);
}

function TickSynth() {
	if (noteOn) {
		envelope += 1.0 / (sampleRate * attack);
	} else {
		envelope -= 1.0 / (sampleRate * release);
	}
	envelope = Mathf.Clamp01(envelope);
	
	// Sine wave
	// return Mathf.Sin(phase * Mathf.PI * 2) * envelope;

	// Sawtooth wave
	// return (phase * 2 - 1) * envelope;

	// Rectangle wave
	// return (phase < modulation ? -1 : 1) * envelope;
	
	// Triangular wave
	// return (phase < 0.25 ? phase * 4 : (phase < 0.75 ? 2.0 - phase * 4.0 : phase * 4 - 4)) * envelope;
	
	// 4-bit Sine wave
	// return Mathf.Floor(Mathf.Sin(phase * Mathf.PI * 2) * 8) / 8 * envelope;
	
	// FM Synthesis
	var carrier = Mathf.Sin(phase * Mathf.PI * 2 * 8) * modulation;
	return Mathf.Sin(phase * Mathf.PI * 2 + carrier) * envelope;
}

////////////////

function Start() {
	sampleRate = AudioSettings.outputSampleRate;
}

function Update() {
	ProcessKeyboard();
}

function OnAudioFilterRead(data : float[], channels : int) {
	UpdateSynth();
	var delta = freq / sampleRate;
    for (var i = 0; i < data.Length; i += 2) {
    	phase += delta;
    	phase -= Mathf.Floor(phase);
        data[i] = data[i + 1] = TickSynth();
    }
}

////////////////

private var keyboardArray = [
	'a', 'w', 's', 'e', 'd',
	'f', 't', 'g', 'y', 'h', 'u', 'j', 
	'k', 'o', 'l', 'p', ';'
];

private var lastPressedKey = '';

function ProcessKeyboard() {
	for (var i = 0; i < keyboardArray.Length; i++) {
		var key = keyboardArray[i];
		if (Input.GetKeyDown(key)) {
			semitone = i;
			lastPressedKey = key;
			noteOn = true;
		} else if (Input.GetKeyUp(key) && lastPressedKey == key) {
			lastPressedKey = '';
			noteOn = false;
		}
	}
	
	if (Input.GetKeyDown('z')) octave--;
	if (Input.GetKeyDown('x')) octave++;
}
