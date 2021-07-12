using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI_Manager : MonoBehaviour
{
	/*==== SINGLETON ====*/
	public static POI_Manager Instance = null;

	/*==== SETTINGS ====*/
	[SerializeField] public List<POI> _pois = null;

    private void Awake()
    {
		Instance = this;
	}

    // Start is called before the first frame update
    void Start()
	{
		_pois.Sort();
	}

	// Update is called once per frame
	void Update()
	{
		TryEnablePOI();
	}

	void TryEnablePOI()
    {
		for (int i = 0; i < _pois.Count; i++)
        {
			if (_pois[i]._timestamp <= VideoController.Instance.GetVideoTimeStamp() && i != (_pois.Count-1))
            {
				_pois[i].gameObject.SetActive(true);
				
				/* put the poi to the end of the list */
				POI temp = _pois[i];
				_pois.RemoveAt(i);
				_pois.Add(temp);

				/* the value of the current index has changed, we also want to check it */
				i--;
            }
			else
            {
				break;
            }
        }
    }
}
