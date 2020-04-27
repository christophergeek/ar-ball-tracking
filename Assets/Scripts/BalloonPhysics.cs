using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonPhysics : MonoBehaviour
{
    private float speed;
    private float maxHeight;

    public Material[] randomMaterials;

    // Start is called before the first frame update
    void Start()
    {
        // apply random material
        GetComponent<Renderer>().material = randomMaterials[Random.Range(0, randomMaterials.Length)];

        //randomise speed and final height
        speed = Random.Range(0.15f, 0.45f);
        maxHeight = Random.Range(0.4f, 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        // float balloon upwards until hits ceiling
        if (transform.position.y <= maxHeight)
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
        }
    }
}
