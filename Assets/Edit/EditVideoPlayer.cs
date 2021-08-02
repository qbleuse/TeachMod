using System;
using UnityEngine;
using UnityEngine.Video;

[ExecuteInEditMode]
public class EditVideoPlayer : MonoBehaviour
{
	/*==== COMPONENTS ====*/
	[HideInInspector] public VideoPlayer _player = null;
	[HideInInspector] public AudioSource _audio  = null;

	/*==== STATE ====*/
	public double   _time       = 0.0f;

	// Start is called before the first frame update
	void Start()
	{
		/* getting and setting the components */
		_player = GetComponent<VideoPlayer>();
		_audio  = GetComponent<AudioSource>();
		_player.SetTargetAudioSource(0, _audio);
	}

	// Update is called once per frame
	void Update()
	{
		_time		= _player.time;
	}


	public void OnPlayModeChange()
	{
		if (_player.isPlaying)
		{
			_player.Pause();
		}
		else
		{
			_player.Play();
		}
	}
}
