using System.Collections;
using UnityEngine;

/* Camera that handles the rotation with gyroscopic input. */
public class GyroCam : MonoBehaviour
{
	/*======== STATE ========*/
	private float       _initialYAngle      = 0f;
	private float       _appliedGyroYAngle  = 0f;
	private float       _calibrationYAngle  = 0f;
	private Transform   _rawGyroRotation;
	private float        _tempSmoothing;

	private float _yRot;
	private float _xRot;
	private Quaternion _tempRot;

	public bool useGyro = true;

	/*======== SETTINGS ========*/
	[SerializeField] private float _smoothing = 0.1f;
	[SerializeField] private float _sensibility = 10.0f;
	[SerializeField, Range(0.0f, 90.0f)] private float _yUpMaxRot = 90.0f;
	[SerializeField, Range(-90.0f, 0.0f)] private float _yBotMaxRot = -90.0f;

	/*======== METHODS ========*/
	private IEnumerator Start()
	{
		if (SystemInfo.deviceType == DeviceType.Desktop)
		{
			enabled = false;
			yield break;
		}

		/* need to do that for android devices */
		Input.gyro.enabled          = true;
		Application.targetFrameRate = 60;

		_rawGyroRotation = new GameObject("GyroRaw").transform;
		_rawGyroRotation.position = transform.position;
		_rawGyroRotation.rotation = transform.rotation;

		/* Wait until gyro is active, then calibrate to reset starting rotation. */
		yield return new WaitForSeconds(1);

		StartCoroutine(CalibrateYAngle());
	}

	private void Update()
	{
		if (Input.touches.Length > 0)
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Moved)
			{
				_yRot -= touch.deltaPosition.x * Time.deltaTime * _sensibility;
				_xRot += touch.deltaPosition.y * Time.deltaTime * _sensibility;

				_tempRot = Quaternion.Euler(_xRot, _yRot, 0.0f);

				_xRot = Mathf.Clamp(_xRot, _yBotMaxRot, _yUpMaxRot);
			}
		}
		
		if (useGyro)
		{
			ApplyGyroRotation();
			ApplyCalibration();
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, _tempRot, _smoothing);
	}

	/* make the rotation on the y axis is always compared to the absolute north being forward */
	private IEnumerator CalibrateYAngle()
	{
		_tempSmoothing = _smoothing;
		_smoothing = 1;
		/* Offsets the y angle in case it wasn't 0 at edit time. */
		_calibrationYAngle = _appliedGyroYAngle - _initialYAngle; 
		yield return null;
		_smoothing = _tempSmoothing;
	}

	private void ApplyCalibration()
	{
		_rawGyroRotation.Rotate(0f, -_calibrationYAngle, 0f, Space.World); // Rotates y angle back however much it deviated when calibrationYAngle was saved.
	}

	private void ApplyGyroRotation()
	{
		Quaternion tempRot = _rawGyroRotation.rotation;
		_rawGyroRotation.rotation = Input.gyro.attitude;

		/* Swap "handedness" of quaternion from gyro. (basically the image is displayed horizontally when holded vertically and vertically when holded horiontally) */
		_rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self);
		/* Rotate to make sense as a camera pointing out the back of your device. */
		_rawGyroRotation.Rotate(90f, 180f, 0f, Space.World);

		ApplyCalibration();

		/* basically recover a delta rather than the absolute rotation */
		_tempRot *= Quaternion.Inverse(tempRot) * _rawGyroRotation.rotation ;
	}

	public void ToggleGyro()
    {
		useGyro = !useGyro;
    }
}
