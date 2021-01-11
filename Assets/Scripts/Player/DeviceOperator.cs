using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceOperator : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            const float maxDistance = 1.5f;
            if (Physics.Raycast(transform.position, 
                transform.forward, out hit, maxDistance))
            {
                GameObject device = hit.transform.gameObject;
                if (Mathf.Abs(Vector3.Dot(hit.transform.forward.normalized,
                    transform.forward.normalized)) < 0.5)
                    return;

                IDevice deviceController;
                if( device.TryGetComponent<IDevice>(out deviceController))
                    deviceController.Operate(string.Empty);
                
            }
        }
    }
}
