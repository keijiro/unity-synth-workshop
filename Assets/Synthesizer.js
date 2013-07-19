#pragma strict

private var tick = 0;

function OnAudioFilterRead(data : float[], channels : int) {
    for (var i = 0; i < data.Length; i += 2) {
        data[i] = data[i + 1] = 0;
        tick++;
    }
}
