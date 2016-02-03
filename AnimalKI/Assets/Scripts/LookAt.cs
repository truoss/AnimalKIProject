using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour
{
    Transform target;
    //Vector3 targetPos;
    //Transform lastTarget;
    Quaternion lastRotation;    
    public Transform HeadBone;
    public float speed = 0;
    public float MaxHeadTurnAngle;
    Vector3 relativePos;
    float angle;

    public GameObject targetObj;

    public void SetTarget(Transform target)
    {
        //Debug.LogWarning(target, target);
        this.target = target;        
    }

    public Transform GetTarget()
    {
        return target;
    }

    /*
    public void SetFocusPosition(Vector3 target)
    {
        targetPos = target;
    }
    */

    public void SetSpeed(float value)
    {
        speed = value;
    }   
    
    public void DoUpdate()
    {
        //Debug.LogWarning("Update LookAt", this);
        
        if (HeadBone)
        {
            if (!target)
            {
                HeadBone.rotation = Quaternion.Lerp(lastRotation, HeadBone.rotation, Time.deltaTime * speed);
                lastRotation = HeadBone.rotation;
            }
            else
            {
                relativePos = target.position - HeadBone.position;
                angle = Vector3.Angle(relativePos, HeadBone.forward);
                if(targetObj)
                    targetObj.transform.position = relativePos;
                if (angle < MaxHeadTurnAngle * 0.5f)
                {
                    if (relativePos != Vector3.zero)
                    {
                        HeadBone.rotation = Quaternion.Lerp(lastRotation, Quaternion.LookRotation(relativePos), Time.deltaTime * speed);
                        lastRotation = HeadBone.rotation;
                    }
                }
                else
                    SetTarget(null);
            }            
        }      
    }
}
