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
    private float micSensitivity;
    private AudioClip nextDeltaRecorded;
    private AudioClip nowDeltaRecorded;
    private float[] spectrumData = new float[256];
    private int minFreq;
    private int maxFreq;
    private bool micConnected;
    private bool validRecording;
    private VoiceStrenght voiceStrenght;
    private int listeningTime = 60;


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

        nowDeltaRecorded = nextDeltaRecorded;
        ListenForFixedTime();

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

    bool ProcessRecordedClip()
    {
        nowDeltaRecorded.GetData(spectrumData, 0);
        voiceStrenght = VoiceStrenght.NoVoice;
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
            //if (Microphone.IsRecording(null))
            //{
            //    Microphone.End(null);
            //    nextDeltaRecorded = Microphone.Start(null, false, 1, maxFreq);
            //}
            if(notRecording)
            {
                nextDeltaRecorded = Microphone.Start(null, false, listeningTime, maxFreq);
            }
        }
        else
        {
            throw new NotSupportedException();
        }  
    }




}
