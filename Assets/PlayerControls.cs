using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public bool JumpPressed { get; private set; }
    public bool SlidePressed { get; private set; }
    public bool KickPressed { get; private set; }

    public bool LeftBargePressed { get; private set; }
    public bool RightBargePressed { get; private set; }

    public bool PausePressed { get; private set; }
    public bool LimpPressed { get; private set; }

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
        Debug.Log("Move: " + MoveInput);
    }

    public void OnLook(InputValue value)
    {
        LookInput = value.Get<Vector2>();
        Debug.Log("Look: " + LookInput);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            JumpPressed = true;
            Debug.Log("Jump");
        }
    }

    public void OnSlide(InputValue value)
    {
        if (value.isPressed)
        {
            SlidePressed = true;
            Debug.Log("Slide");
        }
    }

    public void OnKick(InputValue value)
    {
        if (value.isPressed)
        {
            KickPressed = true;
            Debug.Log("Kick");
        }
    }

    public void OnLeftBarge(InputValue value)
    {
        if (value.isPressed)
        {
            LeftBargePressed = true;
            Debug.Log("Left Barge");
        }
    }

    public void OnRightBarge(InputValue value)
    {
        if (value.isPressed)
        {
            RightBargePressed = true;
            Debug.Log("Right Barge");
        }
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            PausePressed = true;
            Debug.Log("Pause");
        }
    }

    public void OnLimpUnlimp(InputValue value)
    {
        if (value.isPressed)
        {
            LimpPressed = true;
            Debug.Log("Limp / Unlimp");
        }
    }

    void LateUpdate()
    {
        JumpPressed = false;
        SlidePressed = false;
        KickPressed = false;

        LeftBargePressed = false;
        RightBargePressed = false;

        PausePressed = false;
        LimpPressed = false;
    }
}