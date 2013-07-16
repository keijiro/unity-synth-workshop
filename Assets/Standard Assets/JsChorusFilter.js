#pragma strict

@Range(0.0, 100.0)
var delayMs = 20.0;

@Range(0.0, 5.0)
var frequencyHz = 0.2;

@Range(0.0, 100.0)
var depthMs = 10.0;

@Range(0.0, 1.0)
var amplifier = 0.666666;

@Range(0.0, 0.5)
var feedback = 0.1;

@Range(0.0, 1.0)
var stereo = 0.5;

private var bufferSize = 32 * 1024;

private var buffer1 : float[];
private var buffer2 : float[];

private var position = 0;

private var baseDelay = 100.0;
private var depth = 100.0;

private var phi = 0.0;
private var deltaPhi = 2.0 / 44800;

private var error = "";

private function UpdateParameters() {	
	var sampleRate = AudioSettings.outputSampleRate;
	baseDelay = sampleRate * delayMs / 1000;
	depth = sampleRate * depthMs / 2000;
	deltaPhi = frequencyHz / sampleRate;
}

function Awake() {
	buffer1 = new float[bufferSize];
	buffer2 = new float[bufferSize];
	UpdateParameters();
}

function Update() {
	if (error) {
		Debug.LogError(error);
		Destroy(this);
	} else {
		UpdateParameters();
	}
}

function OnAudioFilterRead(data:float[], channels:int) {
	if (channels != 2) {
		error = "This filter only supports stereo audio (given:" + channels + ")";
		return;
	}
	
	var pi2 = Mathf.PI * 2;
	var amp1 = (1.0 + stereo) * 0.5;
	var amp2 = (1.0 - stereo) * 0.5;
	var bufferSizeBits = bufferSize - 1;
	
	for (var i = 0; i < data.Length; i += 2) {
		var m1 = Mathf.Sin(pi2 * phi);
		var m2 = Mathf.Sin(pi2 * (phi + 0.25));

		var d1 = baseDelay + (1.0 + m1) * depth;
		var d2 = baseDelay + (1.0 + m2) * depth;
		var d3 = baseDelay + (1.0 - m1) * depth;
		
		var d1i : int = d1;
		var d2i : int = d2;
		var d3i : int = d3;

		var d1f = d1 - d1i;
		var d2f = d2 - d2i;
		var d3f = d3 - d3i;
		
		var offsetPosition = position + bufferSize;

		var c1a = buffer1[(offsetPosition - d1i    ) & bufferSizeBits];
		var c1b = buffer1[(offsetPosition - d1i - 1) & bufferSizeBits];
		
		var c2a_idx = (offsetPosition - d2i    ) & bufferSizeBits;
		var c2b_idx = (offsetPosition - d2i - 1) & bufferSizeBits;
		var c2a = (buffer1[c2a_idx] + buffer2[c2a_idx]) * 0.5;
		var c2b = (buffer1[c2b_idx] + buffer2[c2b_idx]) * 0.5;

		var c3a = buffer2[(offsetPosition - d3i    ) & bufferSizeBits];
		var c3b = buffer2[(offsetPosition - d3i - 1) & bufferSizeBits];
		
		var c1 = c1a * (1 - d1f) + c1b * d1f;
		var c2 = c2a * (1 - d2f) + c2b * d2f;
		var c3 = c3a * (1 - d3f) + c3b * d3f;
		
		var o1 = c1 * amp1 + c2 + c3 * amp2;
		var o2 = c1 * amp2 + c2 + c3 * amp1;

		var i1 = data[i    ] * amplifier;
		var i2 = data[i + 1] * amplifier;

		data[i    ] = i1 + o1;
		data[i + 1] = i2 + o2;
		
		buffer1[position] = i1 + o1 * feedback;
		buffer2[position] = i2 + o2 * feedback;
		position = (position + 1) & bufferSizeBits;

		phi += deltaPhi;
	}
	
	phi -= Mathf.Floor(phi);
}
