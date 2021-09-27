using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/* class that handles the video displayed on the skybox, and synchronize the audio with it */
public class VideoController : MonoBehaviour
{
	/*==== INSTANCE ====*/
	static public VideoController Instance = null;
	
	/*==== STATE ====*/
	[HideInInspector] public int _currentVideoIndex = -1;

	/* variable that pause the video after the first frame of the video is shown after calling play */
	private bool _frameOnly = false;
	private float _targetTime = 0.0f;
	private bool _setTimeDone = false;
	/* getter/setter that pause the video after the first frame of the video is shown after calling play */
	[HideInInspector] public bool _framePerFrame {	set 
													{ 
														if (value) 
														{ 
															_player.audioOutputMode = VideoAudioOutputMode.None;
															_frameOnly = value; 
														} 
													} } 

	/*==== SETTINGS ====*/
	[SerializeField] private List<string> _sequences = null;

	/*==== COMPONENTS ====*/
	private VideoPlayer _player = null;
	private AudioSource _audio = null;

	[HideInInspector] public GameObject _pauseButton = null;

	private void Awake()
	{
		Instance = this;
	}

	// Start is called before the first frame update
	private void Start()
	{
		/* getting and setting the components */
		_player = GetComponent<VideoPlayer>();
		_audio	= GetComponent<AudioSource>();
		_player.SetTargetAudioSource(0,_audio);

		_pauseButton = transform.GetChild(0).gameObject;

		/* to go to the next video */
		_player.loopPointReached += OnMovieFinished;
		_player.sendFrameReadyEvents = true;
		_player.frameReady += OnFrameReady;

		/* setting sequence (video/audio clips) */
		SetSequence();

		_frameOnly = false;

		/* hitting play */
		_player.Play();
	}

	private void OnFrameReady(VideoPlayer source, long frameIdx)
	{
		if (_frameOnly && _player.isPlaying && _setTimeDone)
		{
			_setTimeDone = false;
			source.Stop();
		}
		else if (_frameOnly && _player.isPlaying && !_setTimeDone)
		{
			StartCoroutine(SetTime());
		}
	}

	IEnumerator SetTime()
    {
		while (!_player.canSetTime)
			yield return null;

		_player.time = _targetTime;
		_setTimeDone = true;
    }

	public void SetSequence()
	{
		_currentVideoIndex++;
		_player.Stop();

#if UNITY_ANDROID && !UNITY_EDITOR
		_player.url = Application.streamingAssetsPath + "/" + _sequences[_currentVideoIndex];
#else
		_player.url = "file://" + Application.streamingAssetsPath + "/" + _sequences[_currentVideoIndex];
#endif

		StartCoroutine(TryPlay());
	}


	IEnumerator TryPlay()
    {
		while (!_player.isPrepared)
		{
			yield return null;
			_player.Prepare();
		}

		while (!_player.isPlaying)
		{
			yield return null;
			_player.Play();
		}
	}

	public void SetVideo(int sequenceNb, float timestamp)
	{
		_currentVideoIndex = sequenceNb;

#if UNITY_ANDROID && !UNITY_EDITOR
		_player.url = Application.streamingAssetsPath + "/" + _sequences[_currentVideoIndex];
#else
		_player.url = "file://" + Application.streamingAssetsPath + "/" + _sequences[_currentVideoIndex];
#endif
		_targetTime = timestamp;

		_player.Play();
	}

	public void PauseAndResume()
	{
		if (_player.isPaused)
		{
			_player.Play();
			/* we want everything to resume */
			Time.timeScale = 1;
		}
		else
		{
			_player.Pause();
			/* we want everything to wait */
			Time.timeScale = 0;
		}
	}

	private void OnMovieFinished(VideoPlayer player)
	{
		if ((_currentVideoIndex + 1) >= _sequences.Count)
		{
			EndMenu.Instance.WakeUp();
			return;
		}

		LevelManager.Instance.StartCoroutine(LevelManager.Instance.FadeRestart(SetSequence));
	}

	// Update is called once per frame
	private void Update()
	{

	}

	/*==== ACCESSOR ====*/
	/* get the timestamp of the currently played sequence */
	public float GetVideoTimeStamp()
	{
		return (float)_player.time;
	}
}
