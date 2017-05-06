using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    public float sensitivity = 100;
    public float loudness = 0;

	AudioSource _audio;
	AudioSource audio{
		get{
			if (_audio == null){
				_audio = gameObject.AddComponent<AudioSource>();
			}
			return _audio;
		}
	}
	
    void Start()
    {
        audio.clip = Microphone.Start(null, true, 10, 44100);
        audio.loop = true;
        audio.mute = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        audio.Play();
    }

    void Update()
    {
        loudness = GetAveragedVolume() * sensitivity;
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256];
        float a = 0;
        audio.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }
}