using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* class that represents the player and its possible inputs */
public class Player : MonoBehaviour
{

    /*==== STATE ====*/
    private Action _raycastCheck = null;

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
    }

    // Update is called once per frame
    void Update()
    {
        _raycastCheck();
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
                        temp.OnHit();
                        EndMenu.Instance._POI_Score++;
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
                    temp.OnHit();
                    outHit.collider.gameObject.SetActive(false);
                    EndMenu.Instance._POI_Score++;
                }
            }
        }
    }
}
