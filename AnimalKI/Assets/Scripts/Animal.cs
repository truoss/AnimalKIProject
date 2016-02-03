using UnityEngine;

public class Animal : MonoBehaviour
{
    public enum AttentionState
    {
        Idle,
        Alert
    }

    public AttentionState attentionState = AttentionState.Idle;
    public Animator animator;    
    public AnimalSight sight;
    AnimalSightFilter sightFilter;

    Transform[] sights;
    //Head
    public LookAt lookAt;
    public float lastChangedThreshold = 5;
    public float MemoryTime = 60;
    public float SightRange = 20;
    public float fieldOfViewAngle = 160;
    public float MaxHeadTurnRate = 5;
    public float UpdateLookAtRate = 3;
    public float lastLookAtUpdate = 0;

    //Body
    public BodyRotateAt bodyRotateAt;
    public float MaxBodyTurnRate = 80; //° per second
    public float UpdateBodyTurnTargetRate = 2;
    public float lastBodyTurnTargetUpdate = 0;

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

        if (bodyRotateAt)
        {
            bodyRotateAt.SetParameter(this.transform, MaxBodyTurnRate);
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

        if (bodyRotateAt)
        {
            bodyRotateAt.SetParameter(this.transform, MaxBodyTurnRate);
        }
    }

    public void SetAttationState(AttentionState state)
    {
        attentionState = state;
    }

    Transform LookTarget;
    void LateUpdate ()
    {
        UpdateAttentionState();

        if (lastLookAtUpdate + UpdateLookAtRate < Time.time)
        {
            SetLookAtTarget();
            //lookAt.SetFocusPosition(sight.GetPreviousSighting());
            lastLookAtUpdate = Time.time;
        }

        if (lastBodyTurnTargetUpdate + UpdateBodyTurnTargetRate < Time.time)
        {
            SetBodyTurnTarget();
            lastBodyTurnTargetUpdate = Time.time;
        }
        

        bodyRotateAt.DoUpdate();

        lookAt.DoUpdate();        
    }

    private void SetBodyTurnTarget()
    {
        //get angle diff
        //var target = lookAt.GetTarget();
        if (LookTarget)
        {
            var vec = LookTarget.position - transform.position;
            float diffAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, vec.y);
            Debug.LogWarning("diffAngle: " + diffAngle*Mathf.Rad2Deg);
            //bodyRotateAt.SetTarget(transform.rotation.eulerAngles.y + diffAngle * Mathf.Rad2Deg);
        }
        //else
            //bodyRotateAt.SetTarget(transform.rotation.eulerAngles.y);
    }

    private void SetLookAtTarget()
    {
        sights = sightFilter.OptimizedSightings(sight.transformsInSight.ToArray());

        if (sights.Length == 0)
            LookTarget = null;
        else if (sights.Length == 1)
        {
            if (Mathf.Round(Random.value * 5) == 1.0f)
                LookTarget = sights[0];
            else
                LookTarget = null;
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
                LookTarget = obj;
            }
            else
            {
                SetAttationState(AttentionState.Idle);
                LookTarget = sights[(int)(Random.Range(0, sights.Length))];
            }
        }

        lookAt.SetTarget(LookTarget);
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
