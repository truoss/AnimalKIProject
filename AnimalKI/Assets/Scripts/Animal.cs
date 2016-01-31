using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{
    public enum AttentionState
    {
        Idle,
        Alert
    }

    public AttentionState attentionState = AttentionState.Idle;
    public Animator animator;
    public LookAt lookAt;
    public AnimalSight sight;
    AnimalSightFilter sightFilter;
    Transform[] sights;
    public float lastChangedThreshold = 5;
    public float MemoryTime = 60;
    public float SightRange = 20;
    public float fieldOfViewAngle = 160;
    public float MaxHeadTurnRate = 5;
    public float UpdateLookAtRate = 3;
    public float lastLookAtUpdate = 0;


    void Start()
    {
        animator = GetComponent<Animator>();
        Random.seed = Random.Range(0, 100);

        if (lookAt)
        {
            lookAt.speed = MaxHeadTurnRate;
            lookAt.MaxHeadTurnAngle = fieldOfViewAngle;
        }

        if (sight)
        {
            sight.Init(fieldOfViewAngle, SightRange);
            sightFilter = new AnimalSightFilter(lastChangedThreshold, MemoryTime);
        }
    }

    void Update()
    {
        if (sight)
        {
            sight.Init(fieldOfViewAngle, SightRange);
            sightFilter.lastChangedThreshold = lastChangedThreshold;
            sightFilter.MemoryTime = MemoryTime;
            sight.DoUpdate();
        }

        if (lookAt)
        {
            //lookAt.speed = MaxHeadTurnRate;
            lookAt.MaxHeadTurnAngle = fieldOfViewAngle;
        }
    }

    public void SetAttationState(AttentionState state)
    {
        attentionState = state;
    }
    
    void LateUpdate ()
    {
        UpdateAttentionState();

        if (lastLookAtUpdate + UpdateLookAtRate < Time.time)
        {
            sights = sightFilter.OptimizedSightings(sight.transformsInSight.ToArray());

            if (sights.Length == 0)
                lookAt.SetTarget(null);
            else if (sights.Length == 1)
            {
                if (Mathf.Round(Random.value*5) == 1.0f)
                    lookAt.SetTarget(sights[0]);
                else
                    lookAt.SetTarget(null);
            }
            else
            {
                //TODO: priorise target
                float highesVelocity = 0;
                Transform obj = sights[0];
                for (int i = 0; i < sights.Length; i++)
                {
                    var rigid = sights[i].GetComponent<Rigidbody>();
                    if (rigid.velocity.sqrMagnitude > highesVelocity)
                    {
                        highesVelocity = rigid.velocity.sqrMagnitude;
                        obj = sights[i];
                    }
                }

                //Debug.LogWarning(highesVelocity.ToString("f7"));
                if (highesVelocity > 0)
                {
                    SetAttationState(AttentionState.Alert);
                    lookAt.SetTarget(obj);
                }
                else
                {
                    SetAttationState(AttentionState.Idle);
                    lookAt.SetTarget(sights[(int)(Random.Range(0, sights.Length))]);
                }
            }


            //lookAt.SetFocusPosition(sight.GetPreviousSighting());
            lastLookAtUpdate = Time.time;
        }
        
        lookAt.DoUpdate();
    }

    private void UpdateAttentionState()
    {
        switch (attentionState)
        {
            case AttentionState.Idle:
                if (lookAt)
                {
                    lookAt.SetSpeed(MaxHeadTurnRate * 0.5f);
                }                        
                break;
            case AttentionState.Alert:
                if (lookAt)
                {
                    lookAt.SetSpeed(MaxHeadTurnRate);
                }
                break;
            default:
                break;
        }
    }
}
