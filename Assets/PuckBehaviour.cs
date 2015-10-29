using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Completed;

public class PuckBehaviour : MonoBehaviour
{
    public float impulseThrust;
    public float drag;
    public float currentSpeed;
    private Rigidbody puck;

    void Start()
    {
        impulseThrust = 200f;
        drag = (impulseThrust / 2) / impulseThrust;

        //check angle component is facing
        //Vector3 forward = transform.forward;
        // Zero out the y component of your forward vector to only get the direction in the X,Z plane
        //forward.y = 0;
        //float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;

        puck = GetComponent<Rigidbody>();
        
        //Testing impulse
        puck.drag = drag;
        puck.AddForce(transform.forward * impulseThrust, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision) { 
        //Check collisions and apply conter-force
    }
    void Update()
    {
        if (GetComponent<MeshRenderer>().enabled)
        {
            currentSpeed = puck.velocity.magnitude;
        }
    }

    void applyForce(float thrust, Vector3 direction) {
        thrust = Mathf.Abs(thrust);

        puck.drag = drag;
        puck.AddForce(direction * impulseThrust, ForceMode.VelocityChange);
    }
}
