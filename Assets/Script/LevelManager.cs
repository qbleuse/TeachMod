using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

/* class that handles changing level and the fade in fade out animation. */
public class LevelManager : MonoBehaviour
{
	/*======== COMPONENTS ========*/

	/* animator for the fade in/fade out */
	public Animator _transition = null;


	/*======== SETTINGS ========*/

	/* tecnically the duration of the animation, can be changed if wanted*/
	[SerializeField] private float _transitionTime = 1.0f;

	/*======== METHODS ========*/

	private void Start()
	{
		_transition = GetComponent<Animator>();
	}

	public void LoadLevel(int levelIndex_)
	{
		StartCoroutine(SmoothLoadLevel(levelIndex_));
	}

	IEnumerator SmoothLoadLevel(int levelIndex_)
	{
		/* fade in animation start */
		_transition.SetTrigger("Start");

		/* wait for animation to finish */
		yield return new WaitForSeconds(_transitionTime);

		/* load scene, the fade out animation should be done in the other scene as 
		 * they should all have a LevelManager and the animation is played on entry*/
		SceneManager.LoadScene(levelIndex_);
	}
}
