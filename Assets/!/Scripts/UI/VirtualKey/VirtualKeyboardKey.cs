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
                break;

            case VirtualKeyType.Space:
                VirtualKeyboardManager.Instance.AddSpace();
                break;

            case VirtualKeyType.Delete:
                VirtualKeyboardManager.Instance.DeleteLastCharacter();
                break;

            case VirtualKeyType.Enter:
                VirtualKeyboardManager.Instance.Confirm();
                break;

            case VirtualKeyType.CapsLock:
                VirtualKeyboardManager.Instance.ToggleCapsLock();
                break;
        }
    }
}