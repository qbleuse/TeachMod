using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public Animator transition = null;
    public float transitionTime = 1.0f;

    private void Awake()
    {
        Instance = this;
        transition = GetComponent<Animator>();
    }

    public static void LoadLevel(int levelIndex)
    {
        Instance.StartCoroutine(Instance.SmoothLoadLevel(levelIndex));
    }

    IEnumerator SmoothLoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
