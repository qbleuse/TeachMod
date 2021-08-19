using System;
using UnityEngine;
using TMPro;

public class SummaryContainer : MonoBehaviour, IComparable<SummaryContainer>
{
	/*==== SETTINGS ====*/
	/* says what sequence it is suppose to appear */
	[SerializeField] public int _sequence = 0;
	/* the timestamp when it starts acting */
	[SerializeField] public float _timestamp = 0.0f;

	/* the rot should be applied when setting this as the main summary */
	[SerializeField] POI _poi = null;

	/*====  COMPONENTS ====*/
	public TextMeshProUGUI content = null;

	public void Init(POI poi_)
	{
		content = GetComponent<TextMeshProUGUI>();

		_sequence = poi_._sequence;
		_timestamp = poi_._timestamp;

		_poi = poi_;
	}

	public void Init(MCQ mcq_)
	{
		content = GetComponent<TextMeshProUGUI>();

		_sequence = mcq_._sequence;
		_timestamp = mcq_._timestamp;
	}

	public void Set(AnimationCam cam)
	{
		if (_poi != null)
        {
			cam.RotateToTarget(_poi.transform.rotation);
        }

		VideoController.Instance.SetVideo(_sequence, _timestamp);
	}

	public void ShutOff()
	{
		if (_poi != null)
        {
			_poi.gameObject.SetActive(false);
        }
	}

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public int CompareTo(SummaryContainer other)
	{
		if (_sequence < other._sequence || (_sequence == other._sequence && _timestamp < other._timestamp))
		{
			return -1;
		}

		return 1;
	}
}
