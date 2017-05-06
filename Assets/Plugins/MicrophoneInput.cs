using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    public float sensitivity = 100;
    public float loudness = 0;

	AudioSource audio;
	
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = Microphone.Start(null, true, 10, 44100);
        audio.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        audio.mute = false;
        audio.Play();
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0f;
        audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }

    void Update()
    {
        loudness = GetAveragedVolume() * sensitivity;
    }

    
}