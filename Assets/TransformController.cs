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

    public GameObject skybox;
    private Quaternion skyboxRotation;

    void Start()
    {
        clone = Instantiate(cloneSource, transform);
        clone.transform.localScale = new Vector3(1, 1, -1);
        skyboxRotation = skybox.transform.rotation;
    }

    void Update()
    {
        // Map cartesian to polar
        Vector3 origin = new Vector3(radius, 0, 0);

        Vector3 position = rig.centerEyeAnchor.position + origin;
        Quaternion rotation = rig.centerEyeAnchor.rotation;
        Vector3 forwardTarget = position + rig.centerEyeAnchor.forward * approximation;
        Vector3 upTarget = position + rig.centerEyeAnchor.up * approximation;

        Vector3 polarPosition = ToPolar(position, 0);
        Vector3 polarForwardTarget = ToPolar(forwardTarget, 1);
        Vector3 polarUpTarget = ToPolar(upTarget, 2);
        Quaternion polarRotation = Quaternion.LookRotation(polarForwardTarget - polarPosition, polarUpTarget - polarPosition);

        transform.position = position - polarPosition;

        Quaternion worldRotation = rotation * Quaternion.Inverse(polarRotation);
        worldRotation.ToAngleAxis(out float angle, out Vector3 axis);
        transform.rotation = Quaternion.identity;
        transform.RotateAround(rig.centerEyeAnchor.position, axis, angle);

        // Move clone
        clone.transform.localPosition = new Vector3(0, 0, Mathf.Sign(polarPosition.z) * cloneLegnth);

        // Update skybox
        skybox.transform.rotation = skyboxRotation * polarRotation;
    }

    private float[] lastTheta = { 0, 0, 0 };
    private float[] thetaOffset = { 0, 0, 0 };

    // x = radius, y unchanged, z = arclength traveled (theta * r)
    Vector3 ToPolar(Vector3 cartesian, int offsetIndex)
    {
        float r = Mathf.Sqrt(cartesian.x * cartesian.x + cartesian.z * cartesian.z);
        float t = Mathf.Atan2(cartesian.z, cartesian.x);
        if (lastTheta[offsetIndex] > Mathf.PI / 2 && t < -Mathf.PI / 2)
            thetaOffset[offsetIndex] += Mathf.PI * 2;
        if (t > Mathf.PI / 2 && lastTheta[offsetIndex] < -Mathf.PI / 2)
            thetaOffset[offsetIndex] -= Mathf.PI * 2;
        lastTheta[offsetIndex] = t;
        return new Vector3(r, cartesian.y, (t + thetaOffset[offsetIndex]) * radius);
    }
}
