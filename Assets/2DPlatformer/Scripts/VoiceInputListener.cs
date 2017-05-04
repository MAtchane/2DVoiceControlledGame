using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// Listener for Voice Input Events
/// </summary>
public class VoiceInputListener : MonoBehaviour {

    public IInputDispatcher inputDispatcher;
    public float micSensitivity;
    public float highLowSoundThreshold;
    private float sensitivityFactor = 0.01f;

    private AudioClip recordedData;
    private AudioClip recordedDataDouble;
    private int minFreq;
    private int maxFreq;
    private bool micConnected;
    private bool validRecording;
    private VoiceStrenght voiceStrenght;
    private int listeningTime = 60;
    private int previousFrameSamplesLenght;
    private float previousFrameSoundData;


    // Use this for initialization
    void Start () {
        voiceStrenght = VoiceStrenght.NoVoice;
        inputDispatcher = new PlayerInputDispatcher();
        if (Microphone.devices.Length <= 0)
        {
            throw new NotSupportedException();
        }
        else 
        {
            micConnected = true;

            //Get the default microphone recording capabilities  
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

            //According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...  
            if (minFreq == 0 && maxFreq == 0)
            {
                maxFreq = 44100;
            }

            //Start recording
            ListenForFixedTime();

        }


    }
	
	void Update () {
        var recordingLooped = recordedData.samples < previousFrameSamplesLenght;
        if (recordingLooped)
        {
            ResetPeriodicRecordingsData();
        }

        validRecording = ProcessRecordedClip();
        if (validRecording)
        {
            switch (voiceStrenght)
            {
                case VoiceStrenght.HighVoice: OnHighVoice();
                    break;
                case VoiceStrenght.LowVoice: OnLowVoice();
                    break;
                case VoiceStrenght.NoVoice: OnNoVoice();
                    break;
                default: throw new InvalidEnumArgumentException();
            }
        }

        previousFrameSamplesLenght = Microphone.GetPosition(null);
    }

    void OnHighVoice()
    {
        inputDispatcher.OnInput(InputCarrier.HighVoice);
    }

    void OnLowVoice()
    {
        inputDispatcher.OnInput(InputCarrier.LowVoice);
    }

    void OnNoVoice()
    {
        inputDispatcher.OnInput(InputCarrier.NoVoice);
    }

    bool ProcessRecordedClip() //todo why return a bool? why return anything? maybe exceptions much!
    {
        float[] samples = new float[recordedData.samples * recordedData.channels];
        recordedData.GetData(samples, 0);
        int i = 0;
        float samplesSum = 0;
        while (i < samples.Length)
        {
            samplesSum += Math.Abs(samples[i]);
            ++i;
        }
        float sensitivityThreshold = (Microphone.GetPosition(null) - previousFrameSamplesLenght) * micSensitivity * sensitivityFactor;
        if ((samplesSum - previousFrameSoundData) > sensitivityThreshold)
        {
            var highLowSoundthresholdData = (Microphone.GetPosition(null) - previousFrameSamplesLenght) * highLowSoundThreshold * sensitivityFactor;
            if ((samplesSum - previousFrameSoundData) > highLowSoundthresholdData)
            {
                voiceStrenght = VoiceStrenght.HighVoice;
            }
            else
            {
                voiceStrenght = VoiceStrenght.LowVoice ;
            }
        }
        else
        {
            voiceStrenght = VoiceStrenght.NoVoice;
        }
        previousFrameSoundData = samplesSum;
        return true;
    }

    public void SetMicSensitivity(float sensitivity)
    {
        micSensitivity = sensitivity;
    }

    /// <summary>
    /// Set up the microphone to listen in between Unity updates
    /// (stops and starts again on every update) Very laggy performance
    /// </summary>
    void ListenForFixedTime()
    {
        if (micConnected)
        {
            var notRecording = !(Microphone.IsRecording(null));

            if(notRecording)
            {
                recordedData = Microphone.Start(null, true, listeningTime, maxFreq);
            }
        }
        else
        {
            throw new NotSupportedException();
        }  
    }

    void ResetPeriodicRecordingsData()
    {
        previousFrameSamplesLenght = 0;
        previousFrameSoundData = 0f;
    }

}
