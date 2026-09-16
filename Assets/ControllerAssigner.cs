using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerAssigner : MonoBehaviour
{
    public PlayerInput player1;
    public PlayerInput player2;

    void Start()
    {
        if (Gamepad.all.Count < 2)
        {
            Debug.Log("Need 2 controllers");
            return;
        }

        player1.DeactivateInput();
        player2.DeactivateInput();

        player1.SwitchCurrentControlScheme(
            Gamepad.all[0]
        );

        player2.SwitchCurrentControlScheme(
            Gamepad.all[1]
        );

        player1.ActivateInput();
        player2.ActivateInput();

        Debug.Log("Player 1: " + Gamepad.all[0].displayName);
        Debug.Log("Player 2: " + Gamepad.all[1].displayName);
    }
}