#pragma strict

var freq = 440.0;
var resolution = 256;

@HideInInspector
var buffer : float[];

private var index = 0;
private var tick = 0.0;
private var interval = 0.0;

function Awake() {
	buffer = new float[resolution];
	interval = AudioSettings.outputSampleRate / freq;
}

function OnAudioFilterRead(data : float[], channels : int) {
    for (var i = 0; i < data.Length; i += channels) {
    	if (index < resolution) {
    		buffer[index++] = data[i];
    	}
    	tick += 1.0;
    	if (tick >= interval) {
    		tick -= interval;
    		index = 0;
    	}
    }
}
