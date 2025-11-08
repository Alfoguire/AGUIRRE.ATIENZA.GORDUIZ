using UnityEngine;

[RequireComponent(typeof(SimpleFPC))]
public class MicrophoneInput : MonoBehaviour
{
    [Header("Microphone Settings")]
    [Tooltip("How much to boost the mic signal. 100 is a good start.")]
    public float sensitivity = 100f;
    [Tooltip("Noise below this level will be ignored.")]
    public float noiseThreshold = 0.01f;

    private SimpleFPC playerController;
    private AudioClip micClip;
    private string micDevice;
    private float[] samples = new float[128];

    void Start()
    {
        playerController = GetComponent<SimpleFPC>();
    }
    
    void OnEnable()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("MicrophoneInput: No microphone devices found!");
            return;
        }
        
        micDevice = Microphone.devices[0];
        micClip = Microphone.Start(micDevice, true, 1, AudioSettings.outputSampleRate);
    }
    
    void OnDisable()
    {
        if (Microphone.IsRecording(micDevice))
        {
            Microphone.End(micDevice);
        }
        if (playerController != null)
        {
            playerController.currentMicrophoneNoise = 0f;
        }
    }

    void Update()
    {
        if (micClip == null) return;

        float micLoudness = GetMicLoudness();

        if (micLoudness < noiseThreshold)
        {
            micLoudness = 0f;
        }

        Debug.Log("Mic Noise: " + (micLoudness * sensitivity));
        playerController.currentMicrophoneNoise = micLoudness * sensitivity;
    }
    
    float GetMicLoudness()
    {
        int micPosition = Microphone.GetPosition(micDevice);
        
        int sampleStart = micPosition - samples.Length;
        if (sampleStart < 0)
            sampleStart = 0; 

        micClip.GetData(samples, sampleStart);

        float totalLoudness = 0;
        foreach (float s in samples)
        {
            totalLoudness += Mathf.Abs(s);
        }

        return totalLoudness / samples.Length;
    }
}