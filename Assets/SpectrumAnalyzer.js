#pragma strict

private var resolution = 1024;
private var meshFilter : MeshFilter;
private var sampleBuffer : float[];

function Awake () {
	meshFilter = GetComponent.<MeshFilter>();
	meshFilter.mesh = new Mesh();
	sampleBuffer = new float[resolution];
}

function Start () {
	var vertices = new Vector3[resolution];
	for (var i = 0; i < resolution; i++) {
		vertices[i] = Vector3(2.0 * Mathf.Log10(8.0 * i / resolution + 1) - 1.0, 0, 0);
	}
	meshFilter.mesh.vertices = vertices;
	
	var indices = new int[resolution];
	for (i = 0; i < indices.Length; i++) {
		indices[i] = i;
	}
	meshFilter.mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
}

function Update () {
	AudioListener.GetSpectrumData(sampleBuffer, 0, FFTWindow.BlackmanHarris);

	var vertices = meshFilter.mesh.vertices;
	for (var i = 0; i < resolution; i++) {
		vertices[i].y = Mathf.Sqrt(sampleBuffer[i]);
	}
	meshFilter.mesh.vertices = vertices;
}