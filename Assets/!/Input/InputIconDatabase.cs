using UnityEngine;

[CreateAssetMenu(
    fileName = "InputIconDatabase",
    menuName = "UI/Input Icon Database"
)]
public class InputIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class IconData
    {
        public Sprite sprite;
        public Vector2 size = new Vector2(64, 64);
    }

    [System.Serializable]
    public class InputIconSet
    {
        public InputActionIcon action;

        [Header("Keyboard")]
        public IconData keyboard;

        [Header("Xbox")]
        public IconData xbox;

        [Header("PlayStation")]
        public IconData playstation;

        [Header("Nintendo Switch")]
        public IconData nintendoSwitch;
    }

    public InputIconSet[] icons;

    public IconData GetIcon(
        InputActionIcon action,
        InputDeviceType deviceType)
    {
        foreach (var icon in icons)
        {
            if (icon.action != action)
                continue;

            switch (deviceType)
            {
                case InputDeviceType.KeyboardMouse:
                    return icon.keyboard;

                case InputDeviceType.Xbox:
                    return icon.xbox;

                case InputDeviceType.PlayStation:
                    return icon.playstation;

                case InputDeviceType.NintendoSwitch:
                    return icon.nintendoSwitch;
            }
        }

        return null;
    }
}

//[System.Serializable]
//public class InputIconSet
//{
//    public InputActionIcon action;

 //   [System.Serializable]
 //   public class IconData
//    {
 //       public Sprite sprite;

 //       [Tooltip("Recommended UI size")]
 //       public Vector2 size = new Vector2(64, 64);
  //  }

    //[Header("Keyboard")]
    //public Sprite keyboardSprite;

    //[Header("Xbox")]
    //public Sprite xboxSprite;

    //[Header("PlayStation")]
    //public Sprite playstationSprite;

    //[Header("Nintendo Switch")]
    //public Sprite switchSprite;
//}

// public InputIconSet[] icons;

//public Sprite GetIcon(
//    InputActionIcon action,
//    InputDeviceType deviceType)
//{
//    foreach (var icon in icons)
//    {
//        if (icon.action != action)
//            continue;

//        switch (deviceType)
//        {
//            case InputDeviceType.KeyboardMouse:
//                return icon.keyboardSprite;
//
//            case InputDeviceType.Xbox:
//                return icon.xboxSprite;

//             case InputDeviceType.PlayStation:
//                return icon.playstationSprite;

//             case InputDeviceType.NintendoSwitch:
//                 return icon.switchSprite;
//         }
//     }

//     return null;
// }