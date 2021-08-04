using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;


/* class that acts as a video player in Unity for the POIEditor.
 * Handles the displaying in the editor window of the video and the controls
 * of the video player */
[ExecuteInEditMode]
public class EditVideoPlayer : MonoBehaviour
{
	/*==== SETTINGS ====*/
	[SerializeField] float _camSpeed = 10.0f;
	[SerializeField] public bool rotate = false;

	/*==== COMPONENTS ====*/
	[HideInInspector] public VideoPlayer _player = null;
	[HideInInspector] public AudioSource _audio  = null;
	Vector2 prevMousePos = Vector2.zero;

	/*==== STATE ====*/
	public	double			_time		= 0.0f;
	public RenderTexture	_texture	= null;
	private VideoClip		_videoClip	= null;

	// Start is called before the first frame update
	public void Start()
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

	public void DrawVideo(Rect rect_)
	{
		GUILayout.BeginArea(rect_);

		Event e = Event.current;
		if (e.type == EventType.MouseDown && rect_.Contains(e.mousePosition))
		{
			rotate = true;
			prevMousePos = e.mousePosition;
		}
		else if (e.type == EventType.MouseDrag && rotate)
        {
			Vector2 delta = e.mousePosition - prevMousePos;
			prevMousePos = e.mousePosition;

			transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - delta.y * _camSpeed * Time.deltaTime, transform.rotation.eulerAngles.y - delta.x * _camSpeed * Time.deltaTime, 0.0f);
		}
		if (e.type == EventType.MouseUp)
        {
			rotate = false;
        }

		GUI.DrawTexture(rect_, _texture);

		GUILayout.EndArea();
	}

	public void OnInspectorGUI()
	{
		if (_videoClip && _player)
		{
			GUILayout.BeginHorizontal();
			float time = EditorGUILayout.Slider("timestamp", (float)_time, 0.0f, (float)_videoClip.length);

			Event e = Event.current;
			if (e.type == EventType.Used)
			{
				_time = time;
				_player.time = _time;
			}

			if (GUILayout.Button("Play"))
				OnPlayModeChange();

			GUILayout.EndHorizontal();
		}

		_videoClip = EditorGUILayout.ObjectField("video", _videoClip, typeof(VideoClip), false) as VideoClip;

		if (_player)
			_player.clip = _videoClip;
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
