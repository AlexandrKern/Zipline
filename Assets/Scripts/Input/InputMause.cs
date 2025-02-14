using UnityEngine;

public class InputMause : IInput
{
    public Vector2 InputPosition()
    {
        return Input.mousePosition;
    }

    public bool MoveCats()
    {
        return Input.GetMouseButton(0);
    }
}
