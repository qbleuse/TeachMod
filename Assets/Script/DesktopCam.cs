using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* the camera that is used for those on desktop (mouse input and key input will be handled) */
public class DesktopCam : MonoBehaviour
{
	/*======== STATE ========*/
	float _yRot;
	float _xRot;
	Quaternion _tempRot;

	/*======== SETTINGS ========*/
	[SerializeField, Range(0.0f, 1.0f)] private float _smoothing	= 0.1f;
	[SerializeField]					private float _sensibility	= 100.0f;

	[SerializeField, Range( 0.0f , 90.0f)]	private float _yUpMaxRot	= 90.0f;
	[SerializeField, Range(-90.0f, 0.0f)]	private float _yBotMaxRot	= -90.0f;

	// Start is called before the first frame update
	void Start()
	{
		if (SystemInfo.deviceType == DeviceType.Handheld)
		{
			enabled = false;
			return;
		}
	}

	// Update is called once per frame
	void Update()
	{
		_yRot += Input.GetAxis("Horizontal") * Time.deltaTime * _sensibility;
		_xRot -= Input.GetAxis("Vertical") * Time.deltaTime * _sensibility;

		_yRot += Input.GetAxis("Mouse X") * Time.deltaTime * _sensibility;
		_xRot -= Input.GetAxis("Mouse Y") * Time.deltaTime * _sensibility;

		_xRot = Mathf.Clamp(_xRot, _yBotMaxRot, _yUpMaxRot);

		_tempRot = Quaternion.Euler(_xRot, _yRot, 0.0f);

		transform.rotation = Quaternion.Slerp(transform.rotation,_tempRot, _smoothing);


	}
}
