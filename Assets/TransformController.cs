using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformController : MonoBehaviour
{
    public OVRCameraRig rig;

    // Radius of the real life circle to map.
    public float radius = 5;

    // Rotational accuracy is perfect at the limit as approximation approaches zero. Very low values may introduce floating point error.
    public float approximation = 0.01f;

    public GameObject cloneSource;

    public float cloneLegnth = 50;

    private GameObject clone;

    void Start()
    {
        clone = Instantiate(cloneSource, transform);
        clone.transform.localScale = new Vector3(1, 1, -1);
    }

    void Update()
    {
        // Map cartesian to polar
        Vector3 origin = new Vector3(radius, 0, 0);

        Vector3 position = rig.centerEyeAnchor.position + origin;
        Quaternion rotation = rig.centerEyeAnchor.rotation;
        Vector3 forwardTarget = position + rig.centerEyeAnchor.forward * approximation;
        Vector3 upTarget = position + rig.centerEyeAnchor.up * approximation;

        Vector3 polarPosition = ToPolar(position);
        Vector3 polarForwardTarget = ToPolar(forwardTarget);
        Vector3 polarUpTarget = ToPolar(upTarget);
        Quaternion polarRotation = Quaternion.LookRotation(polarForwardTarget - polarPosition, polarUpTarget - polarPosition);

        transform.position = position - polarPosition;

        Quaternion worldRotation = rotation * Quaternion.Inverse(polarRotation);
        worldRotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.rotation = Quaternion.identity;
        transform.RotateAround(rig.centerEyeAnchor.position, axis, angle);

        // Move clone
        clone.transform.localPosition = new Vector3(0, 0, Mathf.Sign(polarPosition.z) * cloneLegnth);
    }

    // x = radius, y unchanged, z = arclength traveled (theta * r)
    Vector3 ToPolar(Vector3 cartesian)
    {
        float r = Mathf.Sqrt(cartesian.x * cartesian.x + cartesian.z * cartesian.z);
        float t = Mathf.Atan2(cartesian.z, cartesian.x);
        return new Vector3(r, cartesian.y, t * radius);
    }
}
