using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* POI stands for Point Of Interest, this class will be used to store
 * the info of the important moments of the video. */
public class POI : MonoBehaviour, IComparable<POI>
{
	/* an enum to base if the point is wanted 
	 * or not in this situation */
	public enum Alignment
	{
		UNKNOWN,
		GOOD,
		BAD,

		NB
	}

	/*==== STATE ====*/
	[HideInInspector] public Alignment _userJudgement = Alignment.GOOD;

	/*==== SETTINGS ====*/
	/* says when will it begin to appear */
	[SerializeField]            private float      _timestamp       = 0.0f;
	/* says when will it disappear */
	[SerializeField]            private float      _endTimestamp    = -1.0f;
	/* the comment that explain why this is a poi */
	[SerializeField, Multiline]	public string		_comments		= null;
	/* to know if it is good or not */
	[SerializeField]            private Alignment  _fitting         = Alignment.UNKNOWN;


	/*==== METHODS ====*/
	// Start is called before the first frame update
	void Start()
	{
 
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	/*==== Comparison Interface ====*/
	public int CompareTo(POI other)
	{
		if (other._timestamp > _timestamp)
		{
			return -1;
		}

		return 1;
	}
}