using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI_Manager : MonoBehaviour
{
	/*==== SINGLETON ====*/
	public static POI_Manager Instance = null;

	/*==== SETTINGS ====*/
	[SerializeField] private POI _poiGo = null;

	[SerializeField]	string			_poiList	= null;
	[SerializeField]	string			_mcqList	= null;
	[SerializeField]	string			_animList	= null;

	/*==== STATE ====*/
	[HideInInspector] public	List<POI>		_pois		= null;
	public	List<MCQ>		_mcqs		= null;
	[HideInInspector] public	List<string>	_comments	= null;

	public	int _mcqIndex = 0;
	private int _poiIndex = 0;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		CSVLoader serializer = new CSVLoader();

		if (_poiList.Length > 1)
		{
			serializer.LoadFile(_poiList);
			serializer.PopulatePOI(this);
		}

		if (_mcqList.Length > 1)
		{
			serializer.LoadFile(_mcqList);
			serializer.PopulateMCQ(this);
		}

		_pois.Sort();
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
				SetMCQ(_mcqIndex - 1);
			}

			break;
		}
	}

	public POI InstantiatePOI(int i_)
    {
		_pois.Insert(i_,Instantiate(_poiGo));
		return _pois[i_];
    }

	public void SetMCQ(int i_)
    {
		MCQ question = _mcqs[i_];
		if (!MCQ_Manager.Instance.SetMCQ(question))
		{
			VideoController.Instance.PauseAndResume();
		}
	}

	public void SetMCQAndRotate(int i_, Quaternion rot_)
	{
		MCQ question = _mcqs[i_];
		if (!MCQ_Manager.Instance.SetMCQAndRotate(question, rot_))
		{
			VideoController.Instance.PauseAndResume();
		}
	}
}
