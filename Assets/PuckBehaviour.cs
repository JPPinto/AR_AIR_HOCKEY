using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PuckBehaviour : MonoBehaviour {
    public float impulseThrust;
    public float currentSpeed;
    private Rigidbody puck;
    public int leftBarrierHitPoints = 0;
    public int rightBarrierHitPoints = 0;
    public bool stacionary;
    public float afterThrust;
    public State currentState;
    public float pausedVelocity;
    private GameObject manager;

    void Start() {
        puck = GetComponent<Rigidbody>();
        impulseThrust = 800f;
        afterThrust = 800f;
        leftBarrierHitPoints = 0;
        rightBarrierHitPoints = 0;
        currentSpeed = puck.velocity.magnitude;
        stacionary = true;
        pausedVelocity = 0;
        manager = GameObject.FindGameObjectWithTag("GameMaster");

        puck.drag = 0.5f; 
    }

    void FixedUpdate() {
        if (GetComponent<MeshRenderer>().enabled) {
            currentSpeed = puck.velocity.magnitude;

            currentState = manager.GetComponent<Manager>().getCurrentState();

            switch (currentState) {
                case State.STARTING:
                    transform.position = Vector3.zero;
                    stacionary = true;
                    break;
                case State.PLAYING:
                    //if (pausedVelocity != 0) {
                    //    applyForce(pausedVelocity, transform.forward, false);
                    //    pausedVelocity = 0;
                    //} else {
                        if (stacionary) {
                            stacionary = false;
                            int gen = Random.Range(0, 2);
                            if (gen == 0)
                                transform.Rotate(new Vector3(0, 1, 0), 45f, Space.Self);
                            else
                                transform.Rotate(new Vector3(0, 1, 0), -45f, Space.Self);

                            applyForce(impulseThrust / 2.5f, transform.forward, false);
                        }
                    //}
                    break;
                case State.PAUSED:
                    //pausedVelocity = currentSpeed;
                    puck.velocity = Vector3.zero;
                    break;
                case State.ENDED:
                    puck.velocity = Vector3.zero;
                    transform.position = Vector3.zero;
                    stacionary = true;
                    break;
            }
        }
    }

    void applyForce(float thrust, Vector3 direction, bool collision) {
        thrust = Mathf.Abs(thrust);
        puck.AddForce(direction * thrust, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision col) {
        //Check collisions and apply conter-force

        if (col.gameObject.name == "P1 Goal") {
            //SCORE
            puck.velocity = Vector3.zero;
            transform.position = new Vector3(-250f, 0f, 0f);
            manager.GetComponent<Manager>().playerTwoCounter++;
        }

        if (col.gameObject.name == "P2 Goal") {
            //SCORE
            puck.velocity = Vector3.zero;
            transform.position = new Vector3(250f, 0f, 0f);
            manager.GetComponent<Manager>().playerOneCounter++;
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

            ContactPoint contact = col.contacts[0];

            /* This gave to much work to be deleted, so it will remain a mark forever...
             * 
            Vector3 sizeOfImpactedObject = col.collider.bounds.size;
            Debug.Log("IMPACTED OBJECT SIZE: " + sizeOfImpactedObject);
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
