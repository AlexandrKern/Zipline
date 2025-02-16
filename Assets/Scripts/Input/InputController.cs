using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }
    public IInput input;
    public bool IsDesktop { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeInput();
    }
    private void InitializeInput()
    {
        switch (SystemInfo.deviceType)
        {
            case DeviceType.Handheld:
                input = new InputTouch();
                IsDesktop = false;
                break;
            case DeviceType.Desktop:
                input = new InputMause();
                IsDesktop = true;
                break;
        }
    }
}
