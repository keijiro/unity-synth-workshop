#pragma strict

private var input : OscilloscopeInput;
private var meshFilter : MeshFilter;

function Awake () {
	input = FindObjectOfType(OscilloscopeInput);
	meshFilter = GetComponent.<MeshFilter>();
	meshFilter.mesh = new Mesh();
}

function Start () {
	var vertices = new Vector3[input.resolution];
	for (var i = 0; i < input.resolution; i++) {
		vertices[i] = Vector3(2.0 / input.resolution * i - 1, 0, 0);
	}
	meshFilter.mesh.vertices = vertices;
	
	var indices = new int[input.resolution];
	for (i = 0; i < input.resolution; i++) {
		indices[i] = i;
	}
	meshFilter.mesh.SetIndices(indices, MeshTopology.LineStrip, 0);
}

function Update () {
	var vertices = meshFilter.mesh.vertices;
	for (var i = 0; i < input.resolution; i++) {
		vertices[i].y = input.buffer[i];
	}
	meshFilter.mesh.vertices = vertices;
}
