using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryPointAt : MonoBehaviour
{

    private SentryTargeting sentryTargeting;

    private Quaternion startingRotation;

    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    void Start()
    {
        startingRotation = transform.localRotation;
        sentryTargeting = GetComponentInParent<SentryTargeting>();
    }


    void Update()
    {
        TurnTowardsTarget();

    }

    private void TurnTowardsTarget()
    {


        if (sentryTargeting && sentryTargeting.target && sentryTargeting.wantsToTarget)
        {
            Vector3 disToTarget = sentryTargeting.target.position - transform.position;


            Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up);

            Vector3 euler1 = transform.localEulerAngles; // get local angles before rotation
            Quaternion prevRot = transform.rotation;
            transform.rotation = targetRotation; // set rotation
            Vector3 euler2 = transform.localEulerAngles; // get local angles after rotation

            if (lockRotationX) euler2.x = euler1.x;
            if (lockRotationY) euler2.y = euler1.y;
            if (lockRotationZ) euler2.z = euler1.z;

            transform.rotation = prevRot; // revert rotation

            //animate rotation
            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), .25f);


            //transform.rotation = AnimMath.Slide(transform.rotation, targetRotation, .1f);
        }
        else
        {
            //figure out bone rotation, no target

            transform.localRotation = AnimMath.Slide(transform.localRotation, startingRotation, .05f);
        }
    }
}
