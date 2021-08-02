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
		Debug.Log("OnEnable");

		_prevScene  = SceneManager.GetActiveScene().path;


		_editScene  = EditorSceneManager.OpenScene("Assets/Edit/EditScene.unity");

		SceneManager.SetActiveScene(_editScene);
		//Debug.LogFormat("{0}", _editScene.name);

		_renderRect = new Rect();
		_texture    = new RenderTexture((int)position.width, (int)(position.height/4.0f),24, RenderTextureFormat.ARGB32);

		_camera = Camera.main;
		_camera.targetTexture = _texture;
		_videoPlayer = _camera.GetComponent<EditVideoPlayer>();

		//_editScene.

	}

	private void OnDisable()
	{
		Scene prevScene = EditorSceneManager.OpenScene(_prevScene);
		SceneManager.SetActiveScene(prevScene);
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

		//Handles.DrawCamera(_renderRect,_camera);

		GUI.DrawTexture(_renderRect, _texture);

		GUILayout.EndArea();

		_renderRect.x = 0;
		_renderRect.y = position.height / 2.0f;
		_renderRect.width = position.width;
		_renderRect.height = position.height / 2.0f;

		GUILayout.BeginArea(_renderRect);

		if (_videoClip != null)
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
		_videoPlayer._player.clip = _videoClip;

		GUILayout.EndArea();

	}

	private void Update()
	{
		if (_videoPlayer && _videoPlayer._player && _videoPlayer._player.isPlaying)
			Repaint();
	}
}
