using UnityEngine;

public class PlayerInput : IPlayerInput
{
    public float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }

    public bool GetJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool GetDashInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public bool GetCrouchInput()
    {
        return Input.GetKey(KeyCode.C);
    }

    public bool GetDropDownInput()
    {
        return Input.GetKeyDown(KeyCode.S);
    }
}
