using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* a class that represent a simple popup */
public class Popup : MonoBehaviour
{
    /*==== SETTINGS ====*/
    [SerializeField] private RectTransform _targetTrs = null;

    /*==== STATE ====*/
    private Vector2 _prevPos = Vector2.zero;

    /*==== COMPONENT ====*/
    private RectTransform _trs = null;

    // Start is called before the first frame update
    void Start()
    {
        _trs = GetComponent<RectTransform>();
        _prevPos = _trs.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTargetPos()
    {
        _trs.anchoredPosition = _targetTrs.anchoredPosition;
    }

    public void SetPrevPos()
    {
        _trs.anchoredPosition = _prevPos;
    }

    /* switch between prev and target pos*/
    public void TogglePos()
    {
        if (_trs.anchoredPosition == _prevPos)
            _trs.anchoredPosition = _targetTrs.anchoredPosition;
        else
            _trs.anchoredPosition = _prevPos;
    }
}
