using UnityEngine;

public class HeliRotorController : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public Axis rotateAxis = Axis.Y;

    [Range(0f, 3000f)]
    public float rotorSpeed = 1000f;

    void Update()
    {
        float rotation = rotorSpeed * Time.deltaTime;

        switch (rotateAxis)
        {
            case Axis.X:
                transform.Rotate(rotation, 0f, 0f, Space.Self);
                break;

            case Axis.Y:
                transform.Rotate(0f, rotation, 0f, Space.Self);
                break;

            case Axis.Z:
                transform.Rotate(0f, 0f, rotation, Space.Self);
                break;
        }
    }
}