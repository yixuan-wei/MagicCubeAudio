using UnityEngine;
using System.Collections;
using System.IO;
using System;

public static class AudioClipExtension
{
    public static byte[] GetData(this AudioClip clip)
    {
        var data = new float[clip.samples * clip.channels];

        clip.GetData(data, 0);

        byte[] bytes = new byte[data.Length * 4];
        Buffer.BlockCopy(data, 0, bytes, 0, bytes.Length);

        return bytes;
    }

    public static void SetData(this AudioClip clip, byte[] bytes)
    {
        float[] data = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);

        clip.SetData(data, 0);
    }

    public static byte[] GetData16(this AudioClip clip)
    {
        var data = new float[clip.samples * clip.channels];

        clip.GetData(data, 0);

        byte[] bytes = new byte[data.Length * 2];

        int rescaleFactor = 32767;

        for (int i = 0; i < data.Length; i++)
        {
            short value = (short)(data[i] * rescaleFactor);
            BitConverter.GetBytes(value).CopyTo(bytes, i * 2);
        }

        return bytes;
    }

    public static byte[] EncodeToWAV(this AudioClip clip)
    {
        byte[] bytes = null;

        using (var memoryStream = new MemoryStream())
        {
            memoryStream.Write(new byte[44], 0, 44);//预留44字节头部信息

            byte[] bytesData = clip.GetData16();

            memoryStream.Write(bytesData, 0, bytesData.Length);

            memoryStream.Seek(0, SeekOrigin.Begin);

            byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            memoryStream.Write(riff, 0, 4);

            byte[] chunkSize = BitConverter.GetBytes(memoryStream.Length - 8);
            memoryStream.Write(chunkSize, 0, 4);

            byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            memoryStream.Write(wave, 0, 4);

            byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            memoryStream.Write(fmt, 0, 4);

            byte[] subChunk1 = BitConverter.GetBytes(16);
            memoryStream.Write(subChunk1, 0, 4);

            UInt16 two = 2;
            UInt16 one = 1;

            byte[] audioFormat = BitConverter.GetBytes(one);
            memoryStream.Write(audioFormat, 0, 2);

            byte[] numChannels = BitConverter.GetBytes(clip.channels);
            memoryStream.Write(numChannels, 0, 2);

            byte[] sampleRate = BitConverter.GetBytes(clip.frequency);
            memoryStream.Write(sampleRate, 0, 4);

            byte[] byteRate = BitConverter.GetBytes(clip.frequency * clip.channels * 2); // sampleRate * bytesPerSample*number of channels
            memoryStream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(clip.channels * 2);
            memoryStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            byte[] bitsPerSample = BitConverter.GetBytes(bps);
            memoryStream.Write(bitsPerSample, 0, 2);

            byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
            memoryStream.Write(datastring, 0, 4);

            byte[] subChunk2 = BitConverter.GetBytes(clip.samples * clip.channels * 2);
            memoryStream.Write(subChunk2, 0, 4);

            bytes = memoryStream.ToArray();
        }

        return bytes;
    }
}

public class SpawnByLoudness : MonoBehaviour
{

    DateTimeKind a;


    public GameObject audioInputObject;
    public float threshold = 1.0f;
    public GameObject objectToSpawn;
    public byte []_Wav;
    MicrophoneInput micIn;
    void Start()
    {
        a = DateTimeKind.Local;
        if (objectToSpawn == null)
            Debug.LogError("You need to set a prefab to Object To Spawn -parameter in the editor!");
        //if (audioInputObject == null)
          //  audioInputObject = objectToSpawn;
        micIn = (MicrophoneInput)audioInputObject.GetComponent("MicrophoneInput");
    }

    static string ToHexString(byte[] bytes)
    {
        string HexString = string.Empty;
        if (bytes != null)
        {

        }
    }

    void Update()
    {
        if (a - DateTimeKind.Local > 10) {
            char[] eoc = {0x45,0x4f,0x53 };
            Microphone.End(null);

        }
        float l = micIn.loudness;
        //Debug.Log(l);
       // print(l);
        if (l > 0.5)
        {
           // print(l);
            //Vector3 scale = new Vector3(l, l, l);
            //GameObject newObject = (GameObject)Instantiate(objectToSpawn, this.transform.position, Quaternion.identity);
            //newObject.transform.localScale += scale;


            _Wav = AudioClipExtension.EncodeToWAV(micIn.audio.clip);
            
            Debug.Log("Requirments Hit");
        }
    }
}