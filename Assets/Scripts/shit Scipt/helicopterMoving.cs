using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helicopterMoving : MonoBehaviour
{


    [Header("helicopter")]
    [SerializeField] float RPM = 100;

    public float horizontalMovement;
    public float verticalMovement;
    public float TurningMovement;
    [SerializeField] Vector3 shit = new Vector3(1,0,0);
    float angle = 0;
    Rigidbody rb;
    private Vector2 trustPosisition;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        input();


        float upForce = RPM * angle;
        rb.AddRelativeForce(Vector3.up * upForce);
    }

    void input()
    {
        angle = Input.GetAxisRaw("Fly") * 50;

        horizontalMovement = -Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        TurningMovement = Input.GetAxisRaw("Turning");

        rb.AddRelativeTorque(Vector3.forward * horizontalMovement);
        rb.AddRelativeTorque(Vector3.right * verticalMovement);
        rb.AddRelativeTorque(shit * TurningMovement);

        //trustPosisition = (verticalMovement, horizontalMovement); 
    }

}
