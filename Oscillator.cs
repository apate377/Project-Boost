using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;

    Vector3 startPos;
    public float oscillationSpeed= 2f;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (oscillationSpeed <= Mathf.Epsilon) { return; }
        const float tau = Mathf.PI * 2f;
        transform.position = startPos + movementVector * Mathf.Sin(Time.time * tau/ oscillationSpeed );
    }
}
