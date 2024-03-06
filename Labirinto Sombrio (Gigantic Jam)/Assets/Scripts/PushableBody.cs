using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBody : MonoBehaviour
{
    private Rigidbody tRgbd;

    private void Start()
    {
        tRgbd = this.GetComponentInChildren<Rigidbody>();
    }
    private void OnCollisionEnter(Collision other)
    {
        var crgbd = other.rigidbody;
        if (crgbd is null) crgbd = other.gameObject.GetComponent<Rigidbody>();
        Push(crgbd);
    }

    private void OnTriggerEnter(Collider other)
    {
        var crgbd = other.attachedRigidbody;
        if (crgbd is null) crgbd = other.gameObject.GetComponent<Rigidbody>();
        Push(crgbd);
    }

    public void Push(Rigidbody cRgbd)
    {
        if (cRgbd == null || tRgbd == null) return;
        if (cRgbd != null & (cRgbd.CompareTag("Player") | cRgbd.CompareTag("Enemy")))
        {
            var mass = cRgbd.mass;
            if (cRgbd.CompareTag("Player")) mass /= 30;
            mass -= tRgbd.mass;

            var dir = tRgbd.transform.position - cRgbd.transform.position;
            tRgbd.AddForceAtPosition((mass) * 4f * dir.normalized, cRgbd.worldCenterOfMass);
            //Debug.Log("Pushing Cart! " + (dir.normalized * (cRgbd.mass - tRgbd.mass)).ToString());
        }
    }

    private void Update()
    {
        transform.localEulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
    }
}
