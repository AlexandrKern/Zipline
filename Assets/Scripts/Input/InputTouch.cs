using UnityEngine;

public class InputTouch : IInput
{
    public Vector2 InputPosition()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            return touch.position;
        }
        return Vector2.zero;
    }

    public bool MoveCats()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                return true;
            }
        }
        return false;
    }
}
