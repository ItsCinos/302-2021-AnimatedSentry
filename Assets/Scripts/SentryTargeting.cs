using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTargeting : MonoBehaviour
{
    public Transform target;
    public bool wantsToTarget = true;
    public bool wantsToAttack = false;
    public float minVisionDistance = 2;
    public float maxVisionDistance = 10;
    public float visionAngle = 45;

    private List<TargetablePlayer> potentialTargets = new List<TargetablePlayer>();

    float cooldownScan = 0;
    float cooldownPick = 0;
    float cooldownShoot = 0;

    public float roundsPerSecond = 5;

    public Transform sentryNeck;
    private Vector3 startPosSentryNeck;

    public float bulletSpeed = 10;
    public Transform prefabBullet;
    private Vector3 startPosBullet;


    /// <summary>
    /// A reference to the particle system prefab
    /// </summary>
    public ParticleSystem prefabSentryMuzzleFlash;
    public Transform sentryMuzzle;

    void Start()
    {
        startPosSentryNeck = sentryNeck.localPosition;
        startPosBullet = sentryMuzzle.localPosition;
    }

    void Update()
    {
        wantsToTarget = true;
        wantsToAttack = true;

        if (!wantsToTarget) target = null;

        cooldownScan -= Time.deltaTime; // counting down
        if (cooldownScan <= 0 || (target == null && wantsToTarget)) ScanForTargets(); // do this when countdown finished

        cooldownPick -= Time.deltaTime; // counting down
        if (cooldownPick <= 0) PickATarget(); // do this when countdown finished

        if (cooldownShoot > 0) cooldownShoot -= Time.deltaTime;

        // if we have a target and we can't see it, target = null;
        if (target && !CanSeeThing(target)) target = null;

        SlideNeckHome();

        DoAttack();
    }

    private void SlideNeckHome()
    {
        sentryNeck.localPosition = AnimMath.Slide(sentryNeck.localPosition, startPosSentryNeck, .01f);
    }

    private void DoAttack()
    {
        if (cooldownShoot > 0) return; // too soon
        if (!wantsToTarget) return; // player not targeting
        if (!wantsToAttack) return; // player not attacking
        if (target == null) return; // no target
        if (!CanSeeThing(target)) return;

        HealthSystem targetHealth = target.GetComponent<HealthSystem>();

        if (targetHealth)
        {
            targetHealth.TakeDamage(0);
        }

        cooldownShoot = 2 / roundsPerSecond;


        // attack!

        if (sentryMuzzle) Instantiate(prefabSentryMuzzleFlash, sentryMuzzle.position, sentryMuzzle.rotation); // particle effect

        Transform newBullet = Instantiate(prefabBullet, startPosBullet, Quaternion.identity);
        newBullet.localPosition += AnimMath.Slide(startPosBullet, target.localPosition, 0.5f);

        // moves neck up
        sentryNeck.localEulerAngles += new Vector3(-5, 0, 0);

        SoundEffectBoard.PlaySentryShot();
    }

    private bool CanSeeThing(Transform thing)
    {
        if (!thing) return false; // error...

        Vector3 vToThing = thing.position - transform.position;

        //check distance
        if (vToThing.sqrMagnitude < minVisionDistance * minVisionDistance) return false; // too close to see...
        if (vToThing.sqrMagnitude > maxVisionDistance * maxVisionDistance) return false; // too far away to see...

        //check direction
        if (Vector3.Angle(transform.forward, vToThing) > visionAngle) return false; // out of vision 'cone'

        // TODO: check occlusion

        return true;
    }

    private void ScanForTargets()
    {
        cooldownScan = 1; // do the next scan in 1 second

        // empty the list
        potentialTargets.Clear();

        // refill the list

        TargetablePlayer[] things = GameObject.FindObjectsOfType<TargetablePlayer>();
        foreach (TargetablePlayer thing in things)
        {
            // if we can see it
            // add target to potentialTargets

            if (CanSeeThing(thing.transform))
            {
                potentialTargets.Add(thing);
            }
        }
    }


    void PickATarget()
    {
        cooldownPick = .25f;

        if (target) return; // we already have a target
        //target = null;

        float closestDistanceSoFar = 0;

        // finds closest targetable thing and sets it as our target
        foreach (TargetablePlayer pt in potentialTargets)
        {
            float d = (pt.transform.position - transform.position).sqrMagnitude;

            if (d < closestDistanceSoFar || target == null)
            {
                target = pt.transform;
                closestDistanceSoFar = d;
            }

        }
    }
}
