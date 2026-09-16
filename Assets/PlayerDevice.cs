using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDevice : MonoBehaviour
{
    public int controllerNumber;

    PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        if (Gamepad.all.Count <= controllerNumber)
        {
            Debug.Log("Controller not found for " + gameObject.name);
            return;
        }

        playerInput.DeactivateInput();

        playerInput.neverAutoSwitchControlSchemes = true;

        playerInput.actions.devices =
            new InputDevice[]
            {
                Gamepad.all[controllerNumber]
            };

        playerInput.ActivateInput();

        Debug.Log(
            gameObject.name +
            " using " +
            Gamepad.all[controllerNumber].displayName
        );
    }
}