#pragma strict

var freq = 440.0;
var resolution = 256;

@HideInInspector
var buffer : float[];

private var index = 0;
private var tick = 0.0;
private var interval = 0.0;

function UpdateInterval() {
	interval = AudioSettings.outputSampleRate / freq;
	while (interval < resolution) interval *= 2;
}

function Awake() {
	buffer = new float[resolution];
	UpdateInterval();
}

function Update() {
	UpdateInterval();
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
