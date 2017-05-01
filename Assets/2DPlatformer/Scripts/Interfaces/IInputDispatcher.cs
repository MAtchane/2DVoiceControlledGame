using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Concracts on the Input Controllers which are used to make Gameobjects input-independant
/// </summary>
public interface IInputDispatcher {
    //TODO Remove the generic type in favor of an universal event feedback
    void OnInput(InputCarrier input);
}
