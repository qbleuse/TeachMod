using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The class that does the animation of the circles (color) */
public class CircleAnimation : MonoBehaviour 
{
	/*======== COMPONENTS ========*/

	/* the different circles */
	public GameObject[] _animObjects;

	/*======== SETTINGS ========*/

	[SerializeField] private float _animSpeed = 50.0f;

	/*======== METHODS ========*/

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 angle = Vector3.zero;

		for (int i = 0; i < _animObjects.Length; i++)
		{
			GameObject go = _animObjects[i];

			angle = go.transform.eulerAngles;

			angle.z += Time.deltaTime * _animSpeed;

			go.transform.eulerAngles = angle;
		}
	}
}
