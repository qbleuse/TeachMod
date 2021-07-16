using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* a script that rotate the camera from the rotation it has to the target rotation
 * used to show the POI */
public class AnimationCam : MonoBehaviour
{
    /*==== SETTINGS ====*/
    [SerializeField, Min(0.00001f)]     private float   animDuration = 0.0f;
    [SerializeField]                    private Vector2 screenOffset = new Vector2(0.5f,0.5f); 

    /*==== STATE ====*/
    Quaternion _targetRot;
    Quaternion _offsetRot;

    // Start is called before the first frame update
    void Start()
    {
        Camera cam              = GetComponent<Camera>();
        Vector3 forwardOffset   = cam.ScreenToWorldPoint(new Vector3(screenOffset.x, screenOffset.y, 1.0f));
        _offsetRot              = Quaternion.LookRotation(forwardOffset, Vector3.up);

        /* is only used in particular situation, so disable it as we don't use it now */
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateToTarget(Quaternion targetRot_)
    {
        /* the rotation provided should be compared to a look at:
         * the prefabs of the POI consists in a rotation and the circle being moved 0,0,1 from this rotation
         * wich is basically the same thing we would obtain computing with the point minus the float imprecisions */
        _targetRot = targetRot_;

        enabled = true;

        StartCoroutine(ToTarget());
    }

    private IEnumerator ToTarget()
    {
        float animCountDown = animDuration;
        Quaternion currRot = transform.rotation;

        while (animCountDown > 0.0f)
        {
            animCountDown -= Time.deltaTime;

            /* goes from b to a because we count backward */
            transform.rotation = Quaternion.Slerp(_targetRot, currRot, animCountDown);

            yield return null;
        }

        /* set the rotation to avoid Quaternion imprecision */
        transform.rotation = _targetRot;

        /* animation is done no need for it */
        enabled = false;
    }
}
