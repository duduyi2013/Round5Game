using UnityEngine;
using System.Collections;

public class weapon : MonoBehaviour
{

    GameObject centralPoint;
    // Use this for initialization
    float factor_force, factor_resi_n, factor_resi_tau, weaponRadius;
    void Start()
    {
        //centralPoint = GameObject.FindGameObjectWithTag("CentralPoint");
        factor_force = 5;
        factor_resi_n = 6;
        factor_resi_tau = 5;
        //PublicVariables.weaponRadius = 1; // NTC1
        //weaponRadius = PublicVariables.weaponRadius;
        weaponRadius = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        Gettingback(Time.fixedDeltaTime);
    }
    public void UpdateCentralPoint(GameObject cP)
    {
        centralPoint = cP;
    }
    void Gettingback(float deltaTime)
    {
        Vector3 _relativePosition = this.transform.position - centralPoint.transform.position;
        Vector3 _relativeVelocity = GetComponent<Rigidbody>().velocity;// - centralPoint.GetComponent<Rigidbody>().velocity;
        Vector3 _n_direction = -_relativePosition.normalized;
        Vector3 _tau_direction = Vector3.Cross(_n_direction, Vector3.Cross(_relativeVelocity, _n_direction)).normalized;
        // force for coming back
        GetComponent<Rigidbody>().velocity += _relativePosition.magnitude * factor_force * _n_direction * deltaTime;
        // resistance on n direction
        GetComponent<Rigidbody>().velocity += -Vector3.Dot(_relativeVelocity, _n_direction) * factor_resi_n * _n_direction * deltaTime;
        // resistance on tau direction
        GetComponent<Rigidbody>().velocity += -Vector3.Dot(_relativeVelocity, _tau_direction) * factor_resi_tau * _tau_direction * deltaTime;
        //force for spin
        //GetComponent<Rigidbody>().velocity += -Vector3.Cross(_relativeV elocity, GetComponent<Rigidbody>().angularVelocity) * 0.1f * deltaTime;
    }
    void OnTriggerEnter(Collider c)
    {
        if (c.transform.tag == "Controller")
        {
            Vector3 _reletive_speed = (c.gameObject.GetComponent<Controller_possition_tracking>().velocity  * 10 - GetComponent<Rigidbody>().velocity);
            Vector3 _reletive_position = (c.gameObject.transform.position - this.transform.position);
            Vector3 _angularVelocityDirection = Vector3.Cross(_reletive_position, _reletive_speed).normalized;
            float _angularVelocityMagnitude = Vector3.Cross(_reletive_position, _reletive_speed).magnitude / _reletive_position.magnitude; 
            GetComponent<Rigidbody>().velocity += c.gameObject.GetComponent<Controller_possition_tracking>().velocity * 10;
            GetComponent<Rigidbody>().angularVelocity += _angularVelocityMagnitude * _angularVelocityDirection * 10;
        }
    }      
}
