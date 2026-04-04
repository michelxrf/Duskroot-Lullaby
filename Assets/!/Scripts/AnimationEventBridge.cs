using UnityEngine;

/// <summary>
/// Forwards the animation event call to a parent object
/// Used since the main entities logic and animator are usually detached
/// </summary>
public class AnimationEventBridge : MonoBehaviour
{
    public void CallMethodInParent(string method)
    {
        transform.parent.SendMessage(method, SendMessageOptions.DontRequireReceiver);
    }
}
