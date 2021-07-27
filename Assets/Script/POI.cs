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
	/* identifier for each poi to find itself in the POI_Manager*/
	[SerializeField]			public int				_number			= 0;
	/* says what sequence it is suppose to appear */
	[SerializeField]			public int				_sequence		= 0;
	/* the timestamp when the poi starts acting */
	[SerializeField]			public float			_timestamp		= 0.0f;
	/* the timestamp when the poi ends acting */
	[SerializeField]			public float			_endTimestamp	= 0.0f;
	/* to know if it is good or not */
	[SerializeField]			private Alignment		_fitting		= Alignment.UNKNOWN;
	/* to know if it is good or not */
	[SerializeField]			public  MCQ				_question		= null;
	/* to know if we ask the MCQ when this SM reached its timestamp */
	[SerializeField]			public bool				_pauseOnTime	= false;
	/* to know if we ask the MCQ when this SM is being clicked on */
	[SerializeField]			public bool				_askOnHit		= false;

	/*==== ACCESSOR ====*/
	public Alignment _POI_Fitting { get { return _fitting; } }

	/*==== EVENTS ====*/
	private event Action _onTimeEvent;
	private event Action _onHitEvent;
	
	/*==== METHODS ====*/

	public void SetQuestion()
	{
		if (_askOnHit ? !MCQ_Manager.Instance.SetMCQAndRotate(_question, transform.rotation) : !MCQ_Manager.Instance.SetMCQ(_question))
		{
			VideoController.Instance.PauseAndResume();
		}
	}

	public void WakeUp()
	{
		gameObject.SetActive(true);
		StartCoroutine(OnTime(_endTimestamp - _timestamp));
	}



	public void PutToSleep()
	{
		gameObject.SetActive(false);
	}

	public void UnSubscribeSelf()
	{
		MCQ_Manager.Instance._OnSubmitEvent -= PutToSleep;
	}

	// Start is called before the first frame update
	void Start()
	{
		/* the special behavior of the paused POI */
		if (_pauseOnTime && !_askOnHit)
		{
			/* we put the set question when the waiting time is completed */
			if (_question != null)
			{
				_onTimeEvent += VideoController.Instance.PauseAndResume;
				_onTimeEvent += SetQuestion;
			}

			/* we make it that it should begin when  the timestamp */
			StartCoroutine(OnTime(_timestamp));
			gameObject.transform.GetChild(0).gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(false);
		}
		

		if (_askOnHit)
		{
			_onHitEvent += VideoController.Instance.PauseAndResume;
			_onHitEvent += StopAllCoroutines;
			MCQ_Manager.Instance._OnSubmitEvent += PutToSleep;
		}
		else
		{
			_onHitEvent += VideoController.Instance.PauseAndResume;
			_onHitEvent += PutToSleep;
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	/*==== Comparison Interface ====*/
	public int CompareTo(POI other)
	{
		if (_number < other._number)
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

	public void OnHit()
	{
		_onHitEvent?.Invoke();
	}

}
