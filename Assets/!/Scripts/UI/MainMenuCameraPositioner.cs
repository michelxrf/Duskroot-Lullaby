using UnityEngine;

public class MainMenuCameraPositioner : MonoBehaviour
{
    [SerializeField] GameObject leftAngle;
    [SerializeField] GameObject rightAngle;
    [SerializeField] float transitionDuration = .4f;
    [SerializeField] LeanTweenType transitionEaseType = LeanTweenType.easeInOutSine;

    public void MoveToLeftAngle()
    {
        LeanTween.move(gameObject, leftAngle.transform.position, transitionDuration).setEase(transitionEaseType);
        LeanTween.rotate(gameObject, leftAngle.transform.rotation.eulerAngles, transitionDuration).setEase(transitionEaseType);
    }

    public void MoveToRightAngle()
    {
        LeanTween.move(gameObject, rightAngle.transform.position, transitionDuration).setEase(transitionEaseType);
        LeanTween.rotate(gameObject, rightAngle.transform.rotation.eulerAngles, transitionDuration).setEase(transitionEaseType);
    }

}
