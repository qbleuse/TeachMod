using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct Sequence
{
	public AudioClip audio;
	public VideoClip video;
	public float audioDelay;
}

/* class that handles the video displayed on the skybox, and synchronize the audio with it */
public class VideoController : MonoBehaviour
{
	/*==== INSTANCE ====*/
	static public VideoController Instance = null;
	
	/*==== STATE ====*/
	public int currentVideoIndex = 0;
	private Camera mainCam = null;

	/*==== SETTINGS ====*/
	[SerializeField] private List<Sequence> sequences = null;

	/*==== COMPONENTS ====*/
	private VideoPlayer _player = null;
	private AudioSource _audio = null;

	// Start is called before the first frame update
	private void Start()
	{
		Instance = this;

		/* caching the camera */
		mainCam = Camera.main;

		/* getting and setting the components */
		_player = GetComponent<VideoPlayer>();
		_audio	= GetComponent<AudioSource>();
		_player.SetTargetAudioSource(0,_audio);

		/* to go to the next video */
		_player.loopPointReached += OnMovieFinished;

		/* setting sequence (video/audio clips) */
		SetSequence();

		/* hitting play */
		_player.Play();
		_audio.Play();
	}

	public void SetSequence()
    {
		_player.clip	= sequences[currentVideoIndex].video;

		if (sequences[currentVideoIndex].audio)
		{
			_player.EnableAudioTrack(0, false);
			_audio.clip = sequences[currentVideoIndex].audio;
			_audio.time = sequences[currentVideoIndex].audioDelay;
			_audio.Play();
		}
		else
        {
			_player.EnableAudioTrack(0, true);
		}

		_player.Play();
	}

	public void PauseAndResume()
	{
		if (_player.isPaused)
		{
			_player.Play();
			_audio.Play();
		}
		else
		{
			_player.Pause();
			_audio.Pause();
		}
	}

	private void OnMovieFinished(VideoPlayer player)
    {
		
		currentVideoIndex++;

		LevelManager.Instance.StartCoroutine(LevelManager.Instance.FadeRestart(SetSequence));

	}

	// Update is called once per frame
	private void Update()
	{
	}
}
