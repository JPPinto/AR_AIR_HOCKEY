using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PuckBehaviour : MonoBehaviour {
    public float impulseThrust;
    public float currentSpeed;
    private Rigidbody puck;
    public int leftBarrierHitPoints = 0;
    public int rightBarrierHitPoints = 0;
    public bool outsideBarrierHitPoints;
    public bool stacionary;
    public State currentState;
    public bool paused;
    private GameObject manager;

    private AudioSource _ACfallDown;
    private AudioSource _ACGoal;
    private AudioSource _ACHitPaddle;
    private AudioSource _ACPuckHitSide;

    private AudioSource _ACBackgroundMusic;
    private bool backgroundMusicOn = false;

    void Start() {
        LoadAudio();
        puck = GetComponent<Rigidbody>();
        impulseThrust = 800f;
        leftBarrierHitPoints = 0;
        rightBarrierHitPoints = 0;
        outsideBarrierHitPoints = false;
        currentSpeed = puck.velocity.magnitude;
        stacionary = true;
        paused = false;
        manager = GameObject.FindGameObjectWithTag("GameMaster");

        puck.drag = 0.5f;
    }

    void LoadAudio() {
        _ACfallDown = (AudioSource)gameObject.AddComponent<AudioSource>();
        _ACGoal = (AudioSource)gameObject.AddComponent<AudioSource>();
        _ACHitPaddle = (AudioSource)gameObject.AddComponent<AudioSource>();
        _ACPuckHitSide = (AudioSource)gameObject.AddComponent<AudioSource>();
        _ACBackgroundMusic = (AudioSource)gameObject.AddComponent<AudioSource>();

        AudioClip fallDownClip = (AudioClip)Resources.Load("fall_down");
        AudioClip goalClip = (AudioClip)Resources.Load("goal");
        AudioClip hitPaddleClip = (AudioClip)Resources.Load("hit_paddle");
        AudioClip hitSideClip = (AudioClip)Resources.Load("puck_hit_side");
        AudioClip backgroundMusicClip = (AudioClip)Resources.Load("purity");

        _ACfallDown.clip = fallDownClip;
        _ACGoal.clip = goalClip;
        _ACHitPaddle.clip = hitPaddleClip;
        _ACPuckHitSide.clip = hitSideClip;
        _ACBackgroundMusic.clip = backgroundMusicClip;
        _ACBackgroundMusic.loop = true;
    }

    void FixedUpdate() {
        if (GetComponent<MeshRenderer>().enabled) {
            currentSpeed = puck.velocity.magnitude;

            currentState = manager.GetComponent<Manager>().getCurrentState();

            switch (currentState) {
                case State.STARTING:
                    transform.position = Vector3.zero;
                    stacionary = true;
                    if (!backgroundMusicOn) {
                        _ACBackgroundMusic.Play();
                        backgroundMusicOn = true;
                    }
                    break;
                case State.PLAYING:
                    if (paused) {
                        if (backgroundMusicOn) {
                            _ACBackgroundMusic.Pause();
                            backgroundMusicOn = false;
                        }
                        applyForce(100f, transform.forward, false);
                        paused = false;
                    } else {
                        if (!backgroundMusicOn) {
                            _ACBackgroundMusic.UnPause();
                            backgroundMusicOn = true;
                        }
                        if (stacionary) {
                            stacionary = false;
                            int gen = Random.Range(0, 2);
                            if (gen == 0)
                                transform.Rotate(new Vector3(0, 1, 0), 45f, Space.Self);
                            else
                                transform.Rotate(new Vector3(0, 1, 0), -45f, Space.Self);

                            applyForce(impulseThrust / 2.5f, transform.forward, false);
                        }
                    }
                    break;
                case State.PAUSED:
                    paused = true;
                    puck.velocity = Vector3.zero;
                    break;
                case State.ENDED:
                    puck.velocity = Vector3.zero;
                    transform.position = Vector3.zero;
                    if (backgroundMusicOn) {
                        _ACBackgroundMusic.Stop();
                        backgroundMusicOn = false;
                    }
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
        if (col.gameObject.name == "P1 Goal") {
            //SCORE
            puck.velocity = Vector3.zero;
            outsideBarrierHitPoints = false;
            transform.position = new Vector3(-250f, 0f, 0f);
            _ACGoal.Play();
            manager.GetComponent<Manager>().playerTwoCounter++;
        }

        if (col.gameObject.name == "P2 Goal") {
            //SCORE
            puck.velocity = Vector3.zero;
            outsideBarrierHitPoints = false;
            transform.position = new Vector3(250f, 0f, 0f);
            _ACGoal.Play();
            manager.GetComponent<Manager>().playerOneCounter++;
        }

        if (col.gameObject.name == "Right Barrier" && rightBarrierHitPoints == 0) {
            rightBarrierHitPoints++;
            leftBarrierHitPoints = 0;
            outsideBarrierHitPoints = false;
            _ACPuckHitSide.Play();

            invertDirectionZ();
        }

        if (col.gameObject.name == "Left Barrier" && leftBarrierHitPoints == 0) {
            leftBarrierHitPoints++;
            rightBarrierHitPoints = 0;
            outsideBarrierHitPoints = false;
            _ACPuckHitSide.Play();

            invertDirectionZ();
        }

        if (col.gameObject.tag == "Outside Barrier" && !outsideBarrierHitPoints) {
            leftBarrierHitPoints = 0;
            rightBarrierHitPoints = 0;
            outsideBarrierHitPoints = true;
            _ACPuckHitSide.Play();

            invertDirectionX();
        }

        if (col.gameObject.name == "Paddle") {
            leftBarrierHitPoints = 0;
            rightBarrierHitPoints = 0;
            outsideBarrierHitPoints = false;

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
            _ACHitPaddle.Play();
            changeDirection(contact.normal);
        }
    }

    void changeDirection(Vector3 direction) {
        //Stop puck before applying the same force but with inverted direction of axis x with a rotation
        puck.isKinematic = true;
        puck.velocity = Vector3.zero;
        puck.isKinematic = false;

        transform.forward = direction;

        applyForce(impulseThrust, transform.forward, false);
    }

    void invertDirectionZ() {
        //Stop puck before applying the same force but with inverted direction of axis z
        puck.isKinematic = true;
        puck.velocity = Vector3.zero;
        puck.isKinematic = false;

        transform.forward = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z * (-1f));

        applyForce(currentSpeed, transform.forward, true);
    }

    void invertDirectionX() {
        //Stop puck before applying the same force but with inverted direction of axis z
        puck.isKinematic = true;
        puck.velocity = Vector3.zero;
        puck.isKinematic = false;

        transform.forward = new Vector3(transform.forward.x * (-1f), transform.forward.y, transform.forward.z);

        applyForce(currentSpeed, transform.forward, true);
    }
}
