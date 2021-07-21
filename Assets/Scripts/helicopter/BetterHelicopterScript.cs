using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterHelicopterScript : MonoBehaviour
{
    public GameObject main_Rotor_GameObject;
    public GameObject tail_Rotor_GameObject;

    [SerializeField] float max_Rotor_Force = 22241;
    [SerializeField] float max_Rotor_Velocity = 7200;
    float rotor_Velocity;
    float rotor_Rotation;

    [SerializeField] float max_tail_Rotor_Force = 15000;
    [SerializeField] float max_Tail_Rotor_Velocity = 2200;
    float tail_Rotor_Velocity;
    float tail_Rotor_Rotation;

    [SerializeField] float forward_Rotor_Torque_Multiplier = 0.5f;
    [SerializeField] float sideways_Rotor_Torque_Multiplier = 0.5f;

    //[SerializeField] float multiplier = 1.5f;

    [SerializeField] bool main_Rotor_Active = true;
    [SerializeField] bool tail_Rotor_Active = true;
    [SerializeField] float minimunFlightHieght = 3;

    [SerializeField] float backToZero = 1f;


    AudioSource audio;
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        rb.drag = 0.5f;
    }
    void FixedUpdate()
    {

        //controls the input of rotation
        Vector3 torqueValue = new Vector3(0, 0, 0);
        Vector3 controlTorque = new Vector3(-Input.GetAxisRaw("Pitch") * forward_Rotor_Torque_Multiplier, 0, -Input.GetAxisRaw("Roll") * sideways_Rotor_Torque_Multiplier);

        Vector3 rotation = new Vector3(-transform.localRotation.x * 180, 0, -transform.rotation.z * 180);


        //calculate back to zero
        if (Input.GetAxis("Pitch") == 0.0f && Input.GetAxis("Roll") == 0.0f)
        {
            controlTorque = Vector3.Lerp(controlTorque, rotation, backToZero);
        }
        //Debug.Log(controlTorque);
        Debug.Log(rotation);

        //calculates main rotor
        if (main_Rotor_Active == true)
        {
            //calculate rotational force
            //torqueValue += (controlTorque * max_Rotor_Force * rotor_Velocity);
            torqueValue += (controlTorque);

            //adds upForce
            rb.AddRelativeForce(Vector3.up * max_Rotor_Force * rotor_Velocity);

            //interpolates rotation
            if (Vector3.Angle(Vector3.up, transform.up) < 80)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), Time.deltaTime * rotor_Velocity * 2);
            }
        }

        //calculate tail rotor torque
        if (tail_Rotor_Active == true)
        {
            torqueValue -= (Vector3.up * Input.GetAxis("Yaw") * 50f); 
        }
        //Debug.Log(torqueValue);

        //adds tail rotor torque
        rb.AddRelativeTorque(torqueValue);
        rb.angularDrag = 2;
    }

    private void Update()
    {
        //adjusting the pitch of the audio
        audio.pitch = rotor_Velocity;

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.rotation = Quaternion.identity;
        }

        if (main_Rotor_Active == true) 
        { 
            //rotating the blades
            main_Rotor_GameObject.transform.rotation = transform.rotation * Quaternion.Euler(0, rotor_Rotation, 0); 
        }
        if (tail_Rotor_Active == true) 
        {
            //rotating the blades
            tail_Rotor_GameObject.transform.rotation = transform.rotation * Quaternion.Euler(tail_Rotor_Rotation, 0, 0); 
        }

        //calculating rpm of blade
        rotor_Rotation += max_Rotor_Velocity * rotor_Velocity * Time.deltaTime; 
        tail_Rotor_Rotation += max_Tail_Rotor_Velocity * rotor_Velocity * Time.deltaTime;

        //calculating hover posisition
        float hover_Rotor_Velocity = (rb.mass * Mathf.Abs(Physics.gravity.y) / max_Rotor_Force);
        float hover_Tail_Rotor_Velocity = (max_Rotor_Force * rotor_Velocity) / max_tail_Rotor_Force;

        //animates the blades
        if (Input.GetAxis("Height") != 0.0f) 
        { 
            rotor_Velocity += Input.GetAxis("Height") * 0.001f;
        } else 
        { 
            if (ToCloseToGround())
            {
                rotor_Velocity = Mathf.Lerp(rotor_Velocity, 0.1f , 0.1f);
            }
            else
            {
                rotor_Velocity = Mathf.Lerp(rotor_Velocity, hover_Rotor_Velocity, Time.deltaTime * 500);
            }
        }
        //Debug.Log(rotor_Velocity);

        //calculates tail rotor
        tail_Rotor_Velocity = hover_Tail_Rotor_Velocity - Input.GetAxis("Yaw") * 0.2f; 

        if (rotor_Velocity > 1.0f) 
        { 
            rotor_Velocity = 1.0f; 
        } else if (rotor_Velocity < 0.0) 
        { 
            rotor_Velocity = 0.0f; 
        }


        //checks if to close to ground
        bool ToCloseToGround()
        {
            return Physics.Raycast(transform.position, Vector3.down, minimunFlightHieght);
        }
    }
}