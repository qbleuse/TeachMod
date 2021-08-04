using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI_Manager : MonoBehaviour
{
	/*==== SINGLETON ====*/
	public static POI_Manager Instance = null;

	[SerializeField]  public CSVSerializer _csvSerial = null;

	/*==== STATE ====*/
	public	List<POI>		_pois		= null;
	public	List<MCQ>		_mcqs		= null;
	[HideInInspector] public	List<string>	_comments	= null;

	private	int _mcqIndex = 0;
	private int _poiIndex = 0;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		if (_csvSerial)
		{
			_csvSerial.Load();

			_pois = _csvSerial._pois;
			_mcqs = _csvSerial._mcqs;
			_comments = _csvSerial._comments;

			_csvSerial._pois = null;
			_csvSerial._mcqs = null;
			_csvSerial._comments = null;

			_pois.Sort();
			_mcqs.Sort();
		}
	}

	// Update is called once per frame
	void Update()
	{
		TryEnablePOI();
		TryEnableMCQ();
	}

	void TryEnablePOI()
	{
		while (_poiIndex < _pois.Count)
		{
			if (_pois[_poiIndex]._sequence == VideoController.Instance._currentVideoIndex 
				&& _pois[_poiIndex]._timestamp < VideoController.Instance.GetVideoTimeStamp())
			{
				_pois[_poiIndex].WakeUp();
				_poiIndex++;
			}
			else
			{
				break;
			}
		}
	}

	void TryEnableMCQ()
	{
		while (_mcqIndex < _mcqs.Count)
		{
			if (_mcqs[_mcqIndex]._sequence == VideoController.Instance._currentVideoIndex
				&& _mcqs[_mcqIndex]._timestamp < VideoController.Instance.GetVideoTimeStamp())
			{
				_mcqIndex++;
				if (!_mcqs[_mcqIndex - 1]._pause)
					continue;

				VideoController.Instance.PauseAndResume();
				MCQ question = _mcqs[_mcqIndex - 1];
				SetMCQ(question);
			}

			break;
		}
	}

	public void SetMCQ(MCQ mcq_)
	{

		if (!MCQ_Manager.Instance.SetMCQ(mcq_))
		{
			VideoController.Instance.PauseAndResume();
		}
	}

	public void SetMCQAndRotate(MCQ mcq_, Quaternion rot_)
	{
		if (!MCQ_Manager.Instance.SetMCQAndRotate(mcq_, rot_))
		{
			VideoController.Instance.PauseAndResume();
		}
	}
}
