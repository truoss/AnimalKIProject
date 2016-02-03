using UnityEngine;
using System.Collections;

public class BodyRotateAt : MonoBehaviour
{
    public float targetAngle;
    Transform body;
    float MaxBodyTurnRate = 80;

    public void SetParameter(Transform body, float maxTurnRate)
    {
        this.body = body;
        MaxBodyTurnRate = maxTurnRate;
    }

    public void SetTarget(float angle)
    {
        //Debug.LogWarning(target, target);
        this.targetAngle = angle;
    }

    public void DoUpdate()
    {
        //Debug.LogWarning("Update BodyRotateAt", this);
        if (!body)
            return;

        //body.rotation. = Mathf.LerpAngle(body.ro, targetAngle, Time.deltaTime * MaxBodyTurnRate);
        //float angle = Mathf.LerpAngle(minAngle, maxAngle, Time.time);
        //transform.eulerAngles = new Vector3(0, angle, 0);
        Debug.LogWarning(Mathf.Abs(body.rotation.eulerAngles.y- targetAngle));
        if (Mathf.Abs(body.rotation.eulerAngles.y- targetAngle) > 0.2f)
            body.Rotate(0, Mathf.LerpAngle(body.rotation.eulerAngles.y, targetAngle, Time.deltaTime * MaxBodyTurnRate), 0);
    }
}
