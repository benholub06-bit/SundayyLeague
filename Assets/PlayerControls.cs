using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 FlingInput { get; private set; }


    public void OnMove(InputValue value)
    {
        MoveInput =
            value.Get<Vector2>();
    }


    public void OnFling(InputValue value)
    {
        FlingInput =
            value.Get<Vector2>();
    }
}