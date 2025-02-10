using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceToTarget : MonoBehaviour
{
    public Rigidbody rb;
    public Transform target;
    public float speed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void SetTarget(Transform target, float speed)
    {
        this.target = target;
        this.speed = speed;
        Vector3 direction = (target.position + new Vector3(0, 1f, 0) - transform.position).normalized;
        rb.AddForce((target.position - transform.position).normalized * speed, ForceMode.VelocityChange);
    }

    // private void FixedUpdate() {
    //     if(target == null) return;
    //     Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    // }

}
