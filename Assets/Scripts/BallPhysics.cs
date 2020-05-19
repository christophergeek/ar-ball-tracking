using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    public Vector3 velocity = new Vector3(2, 0, 0);
    private Vector3 acceleration = new Vector3(0, -1, 0);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(velocity * Time.deltaTime, Space.World);
        velocity += acceleration * Time.deltaTime;
    }

    void SetVelocity(float deltaTouchTime, Vector3 cameraForward)
    {

    }
}
