public class MockPlayerInput : IPlayerInput
{
    public float horizontalInput = 0f;
    public bool jumpInput = false;
    public bool dashInput = false;
    public bool crouchInput = false;
    public bool fallingThroughPlatfrom = false;

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }

    public bool GetDashInput()
    {
        return dashInput;
    }

    public bool GetCrouchInput()
    {
        return crouchInput;
    }

    public bool GetJumpInput()
    {
        return jumpInput;
    }

    public bool GetDropDownInput()
    {
        return fallingThroughPlatfrom;
    }
}
