using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerInputDispatcher : IInputDispatcher
{
    private Player player;
    /// <summary>
    /// Take the input event and dispatch it to the appropriate handler
    /// </summary>
    /// <param name="input"></param>
    void IInputDispatcher.OnInput(InputCarrier input)
    {
        if (player == null)
        {
            player = GameObject.FindObjectOfType<Player>();
        }
        switch (input)
        {
            case InputCarrier.LowVoice: player.playerState = PlayerState.Moving;
                break;
            case InputCarrier.HighVoice: player.playerState = PlayerState.Jumping;
                break;
            case InputCarrier.NoVoice: player.playerState = PlayerState.Idle;
                break;
            default: throw new InvalidEnumArgumentException();
        }
        throw new NotImplementedException();
    }
}


