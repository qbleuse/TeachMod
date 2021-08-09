using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* POI stands for Point Of Interest, this class will be used to store
 * the info of the important moments of the video. */
public class POI : MonoBehaviour, IComparable<POI>
{
	/*==== SETTINGS ====*/
	/* says what sequence it is suppose to appear */
	[SerializeField]			public int				_sequence		= 0;
	/* the timestamp when the poi starts acting */
	[SerializeField]			public float			_timestamp		= 0.0f;
	/* the timestamp when the poi ends acting */
	[SerializeField]			public float			_endTimestamp	= 0.0f;
	/* to know if we ask the MCQ when this SM is being clicked on */
	[SerializeField]			public bool				_askOnHit		= false;
	/* the index of the MCQ associated with this POI */
	[HideInInspector]			public MCQ				_mcq			= null;

	/*==== EVENTS ====*/
	private event Action _onHitEvent;
	
	/*==== METHODS ====*/

	public void SetQuestion()
	{
		if (_mcq != null)
        {
			if (_askOnHit)
				POI_Manager.Instance.SetMCQAndRotate(_mcq,transform.rotation);
			else
				POI_Manager.Instance.SetMCQ(_mcq);
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

	// Start is called before the first frame update
	void Start()
	{
		gameObject.SetActive(false);
		

		if (_askOnHit)
		{
			_onHitEvent += VideoController.Instance.PauseAndResume;
			_onHitEvent += SetQuestion;
			_onHitEvent += StopAllCoroutines;
			MCQ_Manager.Instance._OnSubmitEvent += PutToSleep;
		}
		else
		{
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
		if (_sequence < other._sequence || (_sequence == other._sequence && _timestamp < other._timestamp))
		{
			return -1;
		}

		return 1;
	}

	/* Used to disable when it is the time to ends its timestamp */
	IEnumerator OnTime(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		gameObject.SetActive(false);
	}

	public void OnHit()
	{
		_onHitEvent?.Invoke();
	}

}
