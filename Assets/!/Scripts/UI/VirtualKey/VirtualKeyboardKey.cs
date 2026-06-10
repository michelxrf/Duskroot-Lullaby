using UnityEngine;

public class VirtualKeyboardKey :
    MonoBehaviour
{
    [SerializeField]
    private VirtualKeyType keyType;

    [SerializeField]
    private string character;

    public void PressKey()
    {
        switch (keyType)
        {
            case VirtualKeyType.Character:
                VirtualKeyboardManager.Instance.AddCharacter(character);
                AudioUI.instance.PressKey();
                break;

            case VirtualKeyType.Space:
                VirtualKeyboardManager.Instance.AddSpace();
                AudioUI.instance.PressKey();
                break;

            case VirtualKeyType.Delete:
                VirtualKeyboardManager.Instance.DeleteLastCharacter();
                AudioUI.instance.PressKey();
                break;

            case VirtualKeyType.Enter:
                VirtualKeyboardManager.Instance.Confirm();
                AudioUI.instance.PressKey();
                break;

            case VirtualKeyType.CapsLock:
                VirtualKeyboardManager.Instance.ToggleCapsLock();
                AudioUI.instance.PressKey();
                break;
        }
    }
}