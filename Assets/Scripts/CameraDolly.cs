using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDolly : MonoBehaviour
{
    public float pivotAngle = 35f;
    public float smoothTime = 0.25f;
    float pivotVelocity;
    

    StateManager stateMngr;
    // Start is called before the first frame update
    void Start()
    {
        stateMngr = GameObject.FindObjectOfType<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle  = this.transform.rotation.eulerAngles.y;
        if (angle > 180) angle -=360;

        angle = Mathf.SmoothDamp(angle, 
                        stateMngr.currentPlayerId == 0 ? pivotAngle : -pivotAngle, 
                        ref pivotVelocity, 
                        smoothTime);
        
        this.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }
}
