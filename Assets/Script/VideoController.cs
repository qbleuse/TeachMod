using UnityEngine;
using UnityEngine.Video;

/* class that handles the video displayed on the skybox */
public class VideoController : MonoBehaviour
{
	/*==== COMPONENTS ====*/
	private VideoPlayer _player         = null;

	// Start is called before the first frame update
	void Start()
	{
		_player = GetComponent<VideoPlayer>();
	}

	/* method  */
	public void PauseAndResume()
	{
		if (_player.isPaused)
		{
			_player.Play();
		}
		else
		{
			_player.Pause();
		}
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}
