using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : NetworkBehaviour
{
    public float speed = 1000;
    public float damage; // °ø°Ý·Â
    [Networked] public string team { get; set; }
    float time = 0;
    float waitingTime = 2;
    public void Start()
    {
        damage = PlayerFire.instance.atk;
        GetComponent<Rigidbody>().AddForce(transform.forward * speed, ForceMode.Impulse);
    }
    public override void FixedUpdateNetwork()
    {
        time += Time.deltaTime;
        if(time > waitingTime)
        {
            Runner.Despawn(Object);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        if (obj.CompareTag("PlayerCar") || obj.CompareTag("Wall"))
        {
            gameObject.GetComponent<SphereCollider>().isTrigger = false;
            time = 0;
            waitingTime = 0.5f;
        }
    }
}
