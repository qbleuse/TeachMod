using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The class that does the animation of the circles (color) */
public class CircleAnimation : MonoBehaviour 
{
	/*======== COMPONENTS ========*/

	/* the different circles */
	public GameObject[] animObjects;

	/*======== SETTINGS ========*/

	[SerializeField] private float animSpeed = 50.0f;

	/*======== METHODS ========*/

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 angle = Vector3.zero;

		for (int i = 0; i < animObjects.Length; i++)
		{
			GameObject go = animObjects[i];

			angle = go.transform.eulerAngles;

			angle.z += Time.deltaTime * animSpeed;

			go.transform.eulerAngles = angle;
		}
	}
}
