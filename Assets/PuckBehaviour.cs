using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PuckBehaviour : MonoBehaviour {
    public float impulseThrust;
    public float drag;
    public float currentSpeed;
    public Vector3 inverseForward;
    private Rigidbody puck;
    public bool switchDirection = false;
    public int leftBarrierHitPoints = 0;
    public int rightBarrierHitPoints = 0;
    public bool stacionary;
    public float afterThrust;

    void Start() {
        puck = GetComponent<Rigidbody>();
        impulseThrust = 800f;
        afterThrust = 800f;
        switchDirection = true;
        leftBarrierHitPoints = 0;
        rightBarrierHitPoints = 0;
        currentSpeed = puck.velocity.magnitude;
        stacionary = true;

        drag = 0.5f; // (thrust / 2) / thrust;
        puck.drag = drag;

    }

    void Update() {
        if (GetComponent<MeshRenderer>().enabled) {
            currentSpeed = puck.velocity.magnitude;

            if (stacionary) {
                //transform.Rotate(new Vector3(0, 1, 0), 170f, Space.Self);
                //applyForce(afterThrust, transform.forward, false);
                stacionary = false;
            }
        }
    }

    void applyForce(float thrust, Vector3 direction, bool collision) {
        thrust = Mathf.Abs(thrust);
        puck.AddForce(direction * thrust, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision col) {
        //Check collisions and apply conter-force
        print("Collided");

        if (col.gameObject.name == "P1 Goal") {
            //SCORE
            puck.velocity = Vector3.zero;
            transform.position = new Vector3(-310f, 0f, 0f);
        }

        if (col.gameObject.name == "P2 Goal") {
            //SCORE
            puck.velocity = Vector3.zero;
            transform.position = new Vector3(310f, 0f, 0f);
        }

        if (col.gameObject.name == "Right Barrier" && rightBarrierHitPoints == 0) {
            rightBarrierHitPoints++;
            leftBarrierHitPoints = 0;

            invertDirectionZ();
        }

        if (col.gameObject.name == "Left Barrier" && leftBarrierHitPoints == 0) {
            leftBarrierHitPoints++;
            rightBarrierHitPoints = 0;

            invertDirectionZ();
        }

        if (col.gameObject.name == "Paddle") {
            leftBarrierHitPoints = 0;
            rightBarrierHitPoints = 0;

            Vector3 sizeOfImpactedObject = col.collider.bounds.size;
            Debug.Log("IMPACTED OBJECT SIZE: " + sizeOfImpactedObject);

            ContactPoint contact = col.contacts[0];
            
            /* This gave to much work to be deleted, so it will remain a mark forever...
             * 
            Debug.Log(contact.thisCollider.name + " hit " + contact.otherCollider.name);
            Debug.Log("CONTACT POINT: " + contact.point);
            Debug.Log("CONTACT NORMAL: " + contact.normal);
            Debug.Log("OTHER'S POSITION: " + contact.otherCollider.transform.position);

            float distToCenterZ = Mathf.Abs(contact.point.z) - Mathf.Abs(contact.otherCollider.transform.position.z);
            Debug.Log("DISTANCE TO CENTER: " + distToCenterZ);

            float sizeOfOtherZ = sizeOfImpactedObject.z;
            Debug.Log("SIZE OTHER ON Z: " + sizeOfOtherZ);

            float distCenterPercentage = distToCenterZ / (sizeOfOtherZ / 2);
            Debug.Log("DISTANCE TO CENTER PERCENTAGE: " + distCenterPercentage);
            
            float angle = distCenterPercentage * 45;
            Debug.Log("ANGLE TO ROTATE: " + angle);*/

            invertDirectionX(contact.normal);
        }
    }

    void invertDirectionX(Vector3 direction) {
        //Stop puck before applying the same force but with inverted direction of axis x with a rotation
        puck.isKinematic = true;
        puck.velocity = Vector3.zero;
        puck.isKinematic = false;

        transform.forward = direction;

        applyForce(impulseThrust, transform.forward, false);
    }

    void invertDirectionZ() {
        afterThrust = currentSpeed;

        //Stop puck before applying the same force but with inverted direction of axis z
        puck.isKinematic = true;
        puck.velocity = Vector3.zero;
        puck.isKinematic = false;

        transform.forward = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z * (-1f));

        applyForce(afterThrust, transform.forward, true);
    }
}
