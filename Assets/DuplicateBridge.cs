using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateBridge : MonoBehaviour
{
    public GameObject bridge;
    public int copies;

    void Start()
    {
        Collider collider = bridge.GetComponent<Collider>();
        float length = collider.bounds.size.z;
        Vector3 start = bridge.transform.position;

        for (int i = 1; i < copies + 1; i++)
        {
            GameObject cloneA = Instantiate(bridge, transform);
            GameObject cloneB = Instantiate(bridge, transform);
            cloneA.transform.localPosition = new Vector3(0, 0, length * i) + start;
            cloneB.transform.localPosition = new Vector3(0, 0, -length * i) + start;
        }
        
    }
}
