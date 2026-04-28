using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float bounceSpeed = 1f;
    [SerializeField] private float bounceHeight = 0.5f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        Rotate();
        Bounce();
    }

    private void Rotate()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private void Bounce()
    {
        Vector3 newPosition = startPosition;
        newPosition.y += Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = newPosition;
    }
}
