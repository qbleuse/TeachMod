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
	[SerializeField] private List<VideoClip> _sequences = null;

	/*==== COMPONENTS ====*/
	private VideoPlayer _player = null;
	private AudioSource _audio = null;

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
		if (_frameOnly)
			source.Pause();
	}

	public void SetSequence()
	{
		_currentVideoIndex++;

		_player.clip	= _sequences[_currentVideoIndex];

		_player.Play();
	}

	public void SetVideo(int sequenceNb, float timestamp)
	{
		_currentVideoIndex = sequenceNb;

		_player.clip = _sequences[sequenceNb];
		_player.time = timestamp;

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
		if (Input.GetKeyDown(KeyCode.N))
			if (_player.canSetTime)
				_player.time = _player.clip.length - 2.0f;
		if (Input.GetKeyDown(KeyCode.E))
		{
			EndMenu.Instance.WakeUp();
		}
	}

	/*==== ACCESSOR ====*/
	/* get the timestamp of the currently played sequence */
	public float GetVideoTimeStamp()
	{
		return (float)_player.time;
	}
}
