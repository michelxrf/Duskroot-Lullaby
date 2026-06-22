using UnityEngine;

public class MainMenuCameraPositioner : MonoBehaviour
{
    [SerializeField] GameObject leftAngle;
    [SerializeField] GameObject rightAngle;
    [SerializeField] float transitionDuration = .4f;
    [SerializeField] LeanTweenType transitionEaseType = LeanTweenType.easeInOutSine;

    GameObject cameraGameObject;

    private void Start()
    {
        cameraGameObject = this.gameObject;
        MoveToRightAngle();
    }

    public void MoveToLeftAngle()
    {
        LeanTween.move(cameraGameObject, leftAngle.transform.position, transitionDuration).setEase(transitionEaseType);
        LeanTween.rotate(cameraGameObject, leftAngle.transform.rotation.eulerAngles, transitionDuration).setEase(transitionEaseType);
    }

    public void MoveToRightAngle()
    {
        LeanTween.move(cameraGameObject, rightAngle.transform.position, transitionDuration).setEase(transitionEaseType);
        LeanTween.rotate(cameraGameObject, rightAngle.transform.rotation.eulerAngles, transitionDuration).setEase(transitionEaseType);
    }

}
