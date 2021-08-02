using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class POIEditor : EditorWindow
{
	RenderTexture           _texture = null;
	Camera                  _camera = null;
	string                  _prevScene;
	Scene                   _editScene;
	Rect                    _renderRect;
	EditVideoPlayer         _videoPlayer;
	RenderTexture           _videoTex;
	VideoClip               _videoClip;

	[MenuItem("Window/POIEditor")]
	public static void ShowWindow()
	{
		GetWindow<POIEditor>("POIEditor");
	}


	private void OnEnable()
	{ 
		_prevScene  = SceneManager.GetActiveScene().path;

		_editScene  = EditorSceneManager.OpenScene("Assets/Edit/EditScene.unity",OpenSceneMode.Additive);

		_renderRect = new Rect();
		_texture    = new RenderTexture((int)position.width, (int)(position.height),0);

		_camera					= _editScene.GetRootGameObjects()[0].GetComponent<Camera>();
		_camera.targetTexture	= _texture;
		_videoPlayer			= _camera.GetComponent<EditVideoPlayer>() ;
		_videoPlayer.Start();

	}

	private void OnDisable()
	{
		EditorSceneManager.CloseScene(_editScene,true);
	}

	private void OnGUI()
	{
		_renderRect.x = 0;
		_renderRect.y = 0;
		_renderRect.width = position.width;
		_renderRect.height = position.height / 2.0f;
		_camera.aspect = _renderRect.width / _renderRect.height;

		GUILayout.BeginArea(_renderRect);

		_camera.Render();

		GUI.DrawTexture(_renderRect, _texture);

		GUILayout.EndArea();

		_renderRect.x = 0;
		_renderRect.y = position.height / 2.0f;
		_renderRect.width = position.width;
		_renderRect.height = position.height / 2.0f;

		GUILayout.BeginArea(_renderRect);

		if (_videoClip && _videoPlayer && _videoPlayer._player)
		{
			GUILayout.BeginHorizontal();
			float time = EditorGUILayout.Slider("timestamp", (float)_videoPlayer._time, 0.0f, (float)_videoClip.length);
			
			Event e = Event.current;
			if (e.type == EventType.Used)
			{
				_videoPlayer._time = time;
				_videoPlayer._player.time = _videoPlayer._time;
			}

				if (GUILayout.Button("play"))
				_videoPlayer.OnPlayModeChange();
			
			GUILayout.EndHorizontal();
		}

		_videoClip = EditorGUILayout.ObjectField("video",_videoClip,typeof(VideoClip),false) as VideoClip;

		if (_videoPlayer && _videoPlayer._player)
			_videoPlayer._player.clip = _videoClip;

		GUILayout.EndArea();

	}

	private void Update()
	{
		if (_videoPlayer && _videoPlayer._player && _videoPlayer._player.isPlaying)
			Repaint();
	}
}
