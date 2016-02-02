using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimalSight : MonoBehaviour
{
    //public Animal animal; 
    float fieldOfViewAngle = 110f; //TODO: get this from animal
    float sightRange = 10;
            
    public SphereCollider col;                    
    public LayerMask SightMask;

    RaycastHit hit;
    public List<Transform> transformsInSight = new List<Transform>();
    //private int lastSightCount;
    private Transform newestSighting;
    private Transform previousSighting;

    void Awake()
    {
        //gameObject.AddComponent<SphereCollider>();
        col = GetComponent<SphereCollider>();
        //lastFocus = this.transform.forward * col.radius;
        previousSighting = null;
    }

    public void Init(float fov, float range)
    {
        fieldOfViewAngle = fov;
        sightRange = range;
    }

    public void DoUpdate()
    {
        Debug.LogWarning("DoUpdate()");
        //hit = new RaycastHit();
        if (col)
            col.radius = sightRange;


        //TODO: check all in sight 
        for (int i = transformsInSight.Count-1; i >= 0; i = i - 1)
        {
            Vector3 direction = transformsInSight[i].position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            // If the angle between forward and where the player is, is less than half the angle of view...
            if (angle < fieldOfViewAngle * 0.5f)
            {                
                if (transformsInSight[i].gameObject.activeInHierarchy == false)
                    transformsInSight.Remove(transformsInSight[i]);
            }
            else
                transformsInSight.Remove(transformsInSight[i]);
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, col.radius, SightMask))
        {
            if (hit.transform.gameObject)
            {
                //lastFocus = hit.point;
                previousSighting = hit.transform;
            }
        }
    }


    public Vector3 GetPreviousSighting()
    {
        if (previousSighting != null)
            return previousSighting.position;
        else
            return this.transform.forward * col.radius;
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.LogWarning(other, other);
        //every collider goes through here
        //TODO: Filter collider
        if (Utils.IsInLayerMask(other.gameObject, SightMask))
        {
            //Debug.LogWarning("Layer is in sight mask");
            //check if it is in fov
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            // If the angle between forward and where the player is, is less than half the angle of view...
            if (angle < fieldOfViewAngle * 0.5f)
            {
                // ... and if a raycast towards the player hits something...
                if (Physics.Raycast(transform.position, direction.normalized, out hit, col.radius, SightMask))
                {
                    // ... and if the raycast hits the player...
                    if (hit.collider.gameObject.transform && hit.collider.gameObject.tag != "Ground" && hit.collider.gameObject.activeInHierarchy == true)
                    {
                        if (!transformsInSight.Contains(hit.collider.gameObject.transform))
                        {
                            transformsInSight.Add(hit.collider.gameObject.transform);
                            newestSighting = hit.collider.gameObject.transform;
                        }
                    }
                    else if (hit.collider.gameObject.activeInHierarchy == false)
                    {
                        if (transformsInSight.Contains(hit.collider.gameObject.transform))
                            transformsInSight.Remove(hit.collider.gameObject.transform);
                    }
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, direction.normalized, out hit, col.radius, SightMask))
                {
                    // ... and if the raycast hits the player...
                    if (hit.collider.gameObject.transform)
                    {
                        if (transformsInSight.Contains(hit.collider.gameObject.transform))
                            transformsInSight.Remove(hit.collider.gameObject.transform);
                    }
                }
            }
        }
        else if (transformsInSight.Contains(other.gameObject.transform))
        {
            transformsInSight.Remove(other.gameObject.transform);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (SightMask.value == 1 << other.gameObject.layer)
        {
            //check if it is in fov
            Vector3 direction = other.transform.position - transform.position;
            
            // ... and if a raycast towards the player hits something...
            if (Physics.Raycast(transform.position, direction.normalized, out hit, col.radius, SightMask))
            {
                // ... and if the raycast hits the player...
                if (hit.collider.gameObject.transform)
                {
                    if (transformsInSight.Contains(hit.collider.gameObject.transform))
                        transformsInSight.Remove(hit.collider.gameObject.transform);
                }
            }            
        }
    }
}
