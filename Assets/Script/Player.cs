using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* class that represents the player and its possible inputs */
public class Player : MonoBehaviour
{
    /*==== SETTINGS ====*/
    [SerializeField] private GameObject gameMenu = null;

    /*==== STATE ====*/
    private Action  _raycastCheck   = null;
    private POI     _currPOI        = null;

    /*==== COMPONENT ====*/
    private Camera _cam = null;

    // Start is called before the first frame update
    void Start()
    {
        /* change the raycast check depending on the platform */
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            _raycastCheck = HandheldRaycast;
        }
        else 
        {
            _raycastCheck = DesktopRaycast;
        }

        /* caching the camera inside */
        _cam = GetComponent<Camera>();

        /* no need for now */
        if (gameMenu != null)
            gameMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _raycastCheck();
    }

    public void SetCurrGood()
    {
        _currPOI._userJudgement = POI.Alignment.GOOD;

        if (_currPOI._ask)
            _currPOI.SetQuestion();
        else
        {
            VideoController.Instance.PauseAndResume();
        }

        if (gameMenu != null)
            gameMenu.SetActive(false);

        _currPOI = null;
    }

    public void SetCurrBad()
    {
        _currPOI._userJudgement = POI.Alignment.BAD;

        if (_currPOI._ask)
            _currPOI.SetQuestion();
        else
        {
            VideoController.Instance.PauseAndResume();
        }

        if (gameMenu != null)
            gameMenu.SetActive(false);

        _currPOI = null;
    }

    public void SetQuestionCurr()
    {

    }

    public void HandheldRaycast()
    {
        for (int i = 0; i < Input.touches.Length; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Began)
            {
                Ray worldDir = _cam.ScreenPointToRay(touch.position);
                RaycastHit outHit;

                if (Physics.Raycast(worldDir, out outHit))
                {
                    if (outHit.collider.gameObject.tag == "POI")
                    {
                        POI temp = outHit.collider.gameObject.GetComponent<POI>();
                        OnRaycastHitWithPOI(temp);
                    }
                }
            }
        }
    }

    public void DesktopRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray worldDir = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit outHit;

            if (Physics.Raycast(_cam.transform.position,worldDir.direction, out outHit))
            {
                if (outHit.collider.tag == "POI")
                {
                    POI temp = outHit.collider.gameObject.GetComponent<POI>();
                    OnRaycastHitWithPOI(temp);
                }
            }
        }
    }

    private void OnRaycastHitWithPOI(POI poi_)
    {
        /* no need for now */
        if (gameMenu != null)
            gameMenu.SetActive(true);

        _currPOI = poi_;
        poi_.OnHit();
        EndMenu.Instance._POI_Score++;
    }
}
