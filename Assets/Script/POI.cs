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
	/* says what sequence it is suppose to appear */
	[SerializeField]			public int			_sequence		= 0;
	/* says when will it begin to appear */
	[SerializeField]            public float		_timestamp      = 0.0f;
	/* says when will it disappear */
	[SerializeField]            public float		_endTimestamp   = -1.0f;
	/* the comment that explain why this is a poi */
	[SerializeField, Multiline]	public string		_comments		= null;
	/* to know if it is good or not */
	[SerializeField]			private Alignment	_fitting		= Alignment.UNKNOWN;
	/* to know if it is good or not */
	[SerializeField]			public  MCQ			_question		= null;
	/* to know if we stop the video when this SM reached its timestamp */
	[SerializeField]			private	bool		_pauseOnTime	= false;

	/*==== ACCESSOR ====*/
	public Alignment _POI_Fitting { get { return _fitting; } }

	public event Action _onTimeEvent;
	
	/*==== METHODS ====*/
	void Awake()
    {
		StartCoroutine(RegisterToManager());
    }

	/* the Awake instances can begin in different orders so we try multiple times to register ourselves */
	IEnumerator RegisterToManager()
    {
		while (POI_Manager.Instance == null)
		{
			yield return null;
		}
		/* register itself to the poi_Manager */
		POI_Manager.Instance._pois.Add(this);
	}

	void SetQuestion()
    {
		VideoController.Instance.PauseAndResume();
		MCQ_Manager.Instance.gameObject.SetActive(true);
		if (!MCQ_Manager.Instance.SetMCQ(_question))
        {
			MCQ_Manager.Instance.gameObject.SetActive(false);
			VideoController.Instance.PauseAndResume();
		}
    }


	// Start is called before the first frame update
	void Start()
	{
		if (_pauseOnTime)
		{
			if (_question != null)
			{
				_onTimeEvent += SetQuestion;
			}

			StartCoroutine(OnTime(_timestamp));
			gameObject.transform.GetChild(0).gameObject.SetActive(false);
		}
        else
        {
			StartCoroutine(OnTime(_endTimestamp - _timestamp));
			gameObject.SetActive(false);
		}
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	/*==== Comparison Interface ====*/
	public int CompareTo(POI other)
	{
		if (other._sequence > _sequence || other._timestamp > _timestamp)
		{
			return -1;
		}

		return 1;
	}

	/* Used to disable when it is the time to ends its timestamp */
	IEnumerator OnTime(float waitTime)
    {
		yield return new WaitForSeconds(waitTime);

		_onTimeEvent?.Invoke();

		gameObject.SetActive(false);
    }

}
