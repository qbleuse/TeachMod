using UnityEngine;

/* Camera that handles the rotation with gyroscopic input. */
public class GyroCam : MonoBehaviour
{
    /* STATE */
    private Transform   _rawGyroRotation;

    /* SETTINGS */
    [SerializeField] private float _smoothing = 0.1f;

    private void Start()
    {
        /* need to do that for android devices */
        Input.gyro.enabled          = true;
        Application.targetFrameRate = 60;

        _rawGyroRotation = new GameObject("GyroRaw").transform;
        _rawGyroRotation.position = transform.position;
        _rawGyroRotation.rotation = transform.rotation;
    }

    private void Update()
    {
        ApplyGyroRotation();

        transform.rotation = Quaternion.Slerp(transform.rotation, _rawGyroRotation.rotation, _smoothing);
    }

    private void ApplyGyroRotation()
    {
        _rawGyroRotation.rotation = Input.gyro.attitude;

#if UNITY_ANDROID
        /* Swap "handedness" of quaternion from gyro. (basically the image is displayed horizontally when holded vertically and vetically when holded horiontally)*/
        _rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self);
        /* Rotate to make sense as a camera pointing out the back of your device. */
        _rawGyroRotation.Rotate(270f, 180f, 180f, Space.World); 
#else
        /* Swap "handedness" of quaternion from gyro. (basically the image is displayed horizontally when holded vertically and vetically when holded horiontally) */
        _rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self);
        /* Rotate to make sense as a camera pointing out the back of your device. */
        _rawGyroRotation.Rotate(90f, 180f, 0f, Space.World);
#endif
    }
}
