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

	[Serializable]
	public struct KeyFrame
	{
		public float timestamp;

		/* the rotation will actually move the feedback, 
		 * so this is the only info we need to move it */
		public float yaw;
		public float pitch;

		/* we scale it uniformly on the x and y axis,
		 * so we only need one value */
		public float size;
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
	/* make the animation of the POI, 
	 * the timestamps are used to activate/deactivate the POI */
	[SerializeField]			public List<KeyFrame>	_movement		= null;
	/* the comment that explain why this is a poi */
	[SerializeField, Multiline]	public string			_comments		= null;
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

		if (_pauseOnTime)
		{
			/* register itself to the poi_Manager */
			POI_Manager.Instance._pausePois.Add(this);
			yield break;
		}
		/* register itself to the poi_Manager */
		POI_Manager.Instance._pois.Add(this);
	}

	public void SetQuestion()
	{
		if (_askOnHit ? !MCQ_Manager.Instance.SetMCQAndRotate(_question, transform.rotation) : !MCQ_Manager.Instance.SetMCQ(_question))
		{
			VideoController.Instance.PauseAndResume();
		}
	}

	public void WakeUp()
	{
		if (_movement != null)
		{
			gameObject.SetActive(true);
			StartCoroutine(OnTime(_endTimestamp - _timestamp));
			if (_movement.Count > 1)
			{
				StartCoroutine(Move());
			}
		}
	}

	private IEnumerator Move()
	{
		int i = 0;
		KeyFrame curr		= _movement[i];
		KeyFrame next		= _movement[i + 1];
		float dur			= next.timestamp - curr.timestamp;
		Quaternion currRot	= Quaternion.Euler(curr.yaw, curr.pitch, 0.0f);
		Quaternion nextRot	= Quaternion.Euler(next.yaw, next.pitch, 0.0f);
		Vector3 currScale = new Vector3(curr.size, curr.size, 1.0f);
		Vector3 nextScale	= new Vector3(next.size, next.size, 1.0f);

		float time = VideoController.Instance.GetVideoTimeStamp();
		float lerp = 0;

		while (i < _movement.Count - 1)
		{
			if (time > next.timestamp)
			{
				curr		= _movement[i];
				next		= _movement[i + 1];
				dur			= next.timestamp - curr.timestamp;
				currRot		= Quaternion.Euler(curr.yaw, curr.pitch, 0.0f);
				nextRot		= Quaternion.Euler(next.yaw, next.pitch, 0.0f);
				currScale	= new Vector3(curr.size, curr.size, 1.0f);
				nextScale	= new Vector3(next.size, next.size, 1.0f);
			}

			lerp = (time - curr.timestamp) / dur;

			transform.rotation		= Quaternion.Slerp(currRot, nextRot, lerp);
			transform.localScale	= Vector3.Lerp(currScale, nextScale, lerp);

			yield return null;


			time = VideoController.Instance.GetVideoTimeStamp();

			if (time > next.timestamp)
			{
				i++;
			}
		}
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
		if (_pauseOnTime && !_askOnHit && _movement != null)
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
