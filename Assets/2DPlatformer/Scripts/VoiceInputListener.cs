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
    private AudioClip deltaRecorded;
    private int minFreq;
    private int maxFreq;
    private bool micConnected;
    private bool validRecording;
    private VoiceStrenght voiceStrenght;


	// Use this for initialization
	void Start () {

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
            ListenTillNextUpdate();

        }


    }
	
	// Update is called once per frame
	void Update () {
        StopRecording();
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

        ListenTillNextUpdate();

    }

    void StopRecording()
    {
        Microphone.End(null);
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
        voiceStrenght = VoiceStrenght.HighVoice;
        return true;
    }

    public void SetMicSensitivity(float sensitivity)
    {
        micSensitivity = sensitivity;
    }

    /// <summary>
    /// Set up the microphone to listen in between Unity updates
    /// (stops and starts again on every update) TODO check performance
    /// </summary>
    void ListenTillNextUpdate()
    {
        if (Microphone.devices.Length > 0)
        {
            if (Microphone.IsRecording(null))
            {
                Microphone.End(null);
                deltaRecorded = Microphone.Start(null, true, 1, maxFreq);
            }
            else
            {
                deltaRecorded = Microphone.Start(null, true, 1, maxFreq);
            }
        }
        else
        {
            throw new NotSupportedException();
        }  
    }


}
