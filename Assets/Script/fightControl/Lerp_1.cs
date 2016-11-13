using UnityEngine;
using System.Collections;

public class Lerp_1 : MonoBehaviour {
    public GameObject target;
    float chasing_speed;
    float chasing_acc;
    float rushing_speed;
    float rushing_acc;
    int attacking_status;
    Vector3 _relativePositionOfLastFrame;
    bool startTrackinLastFrameForAttacking;
    float _timer_for_attacking;
	// Use this for initialization
	void Start () { 
        //target = GameObject.FindGameObjectWithTag("target");
        chasing_speed = 5;
        chasing_acc = 10;
        rushing_speed = 25;
        rushing_acc = 10;
        startTrackinLastFrameForAttacking = false;
        _timer_for_attacking = 0;
        attacking_status = 1;

    }
    //Vector3 Vector3.toXZ()
    //{

    //    return new Vector3(0, 0, 0);
    //}
    // Update is called once per frame
    void Update () {
        //this.GetComponent<Rigidbody>().velocity = (target.transform.position - this.transform.position).normalized * chasing_speed;
	}
    void FixedUpdate()
    {
        //Vector3 _tmp1 = Vector3.Cross(this.GetComponent<Rigidbody>().velocity, target.transform.position - this.transform.position);
        //Vector3 _tmp2 = Vector3.Cross(_tmp1, GetComponent<Rigidbody>().velocity);
        //this.GetComponent<Rigidbody>().velocity += chasing_acc * _tmp2.normalized * Time.fixedDeltaTime;
        //this.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * chasing_speed;
        //Vector3 _tmp1 = Vector3.Cross(this.GetComponent<Rigidbody>().velocity.normalized, (target.transform.position - this.transform.position).normalized);
        //Vector3 _tmp2 = Vector3.Cross(_tmp1, GetComponent<Rigidbody>().velocity);
        //this.GetComponent<Rigidbody>().velocity += chasing_acc * _tmp2 * Time.fixedDeltaTime;
        //this.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * chasing_speed;
        //Chasing3();
		this.transform.position = new Vector3(this.transform.position.x,target.transform.position.y,this.transform.position.z);
        Attacking();
        
    }
    void Chasing1()
    {
        Vector3 _relativePosition = target.transform.position - this.transform.position;
        Vector3 _targetVelocity = target.GetComponent<Rigidbody>().velocity;
        Vector3 _targetVelocityOnRelativePosition = Vector3.Dot(_targetVelocity, _relativePosition.normalized) * _relativePosition.normalized;
        
    }

    void Chasing2()
    {
        Vector3 _relativePosition = target.transform.position - this.transform.position;
        Vector3 _targetVelocity = target.GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = chasing_speed * _relativePosition.normalized;
      
        float rotating_angle = Mathf.Acos(Vector3.Dot(_relativePosition.normalized, new Vector3(1, 0, 0))) * 180f / 3.14f;
        Vector3 rotating_axis = Vector3.Cross(new Vector3(1,0,0),_relativePosition).normalized; 
        this.transform.rotation = Quaternion.AngleAxis(rotating_angle, rotating_axis);
    }
    void Chasing3()
    {
        Vector3 _relativePosition = target.transform.position - this.transform.position;
        Vector3 _targetVelocity = target.GetComponent<Rigidbody>().velocity;
        Vector3 _selfVelocity = this.GetComponent<Rigidbody>().velocity;
        if (_selfVelocity.magnitude > 0)
        {
            Vector3 _tmpVector = Vector3.Cross(_selfVelocity.normalized, _relativePosition.normalized);
            Vector3 _n_direction_times_sin = Vector3.Cross(_tmpVector, GetComponent<Rigidbody>().velocity.normalized);
            this.GetComponent<Rigidbody>().velocity += chasing_acc * _n_direction_times_sin * Time.fixedDeltaTime;
            this.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * chasing_speed;
            float rotating_angle = Mathf.Acos(Vector3.Dot(this.GetComponent<Rigidbody>().velocity.normalized, new Vector3(1, 0, 0))) * 180f / 3.14f;
            Vector3 rotating_axis = Vector3.Cross(new Vector3(1, 0, 0), this.GetComponent<Rigidbody>().velocity).normalized;
            this.transform.rotation = Quaternion.AngleAxis(rotating_angle, rotating_axis);
        }
        else
        {
            this.GetComponent<Rigidbody>().velocity += Time.fixedDeltaTime * new Vector3(0.01f, 0, 0);
        }
    }
    void Rushing()
    {
        Vector3 _relativePosition = target.transform.position - this.transform.position;
        Vector3 _targetVelocity = target.GetComponent<Rigidbody>().velocity;
        Vector3 _selfVelocity = this.GetComponent<Rigidbody>().velocity;
        if (_selfVelocity.magnitude > 0)
        {
            Vector3 _tmpVector = Vector3.Cross(_selfVelocity.normalized, _relativePosition.normalized);
            Vector3 _n_direction_times_sin = Vector3.Cross(_tmpVector, GetComponent<Rigidbody>().velocity.normalized);
            this.GetComponent<Rigidbody>().velocity += rushing_acc * _n_direction_times_sin * Time.fixedDeltaTime;
            this.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * rushing_speed;
            float rotating_angle = Mathf.Acos(Vector3.Dot(this.GetComponent<Rigidbody>().velocity.normalized, new Vector3(1, 0, 0))) * 180f / 3.14f;
            Vector3 rotating_axis = Vector3.Cross(new Vector3(1, 0, 0), this.GetComponent<Rigidbody>().velocity).normalized;
            this.transform.rotation = Quaternion.AngleAxis(rotating_angle, rotating_axis);
        }
        else
        {
            this.GetComponent<Rigidbody>().velocity += Time.fixedDeltaTime * new Vector3(0.01f, 0, 0);
        }
    }
    void Attacking()
    {
        if (startTrackinLastFrameForAttacking)
        {
            Vector3 _relativePosition = target.transform.position - this.transform.position;
            if(attacking_status == 2 && _relativePosition.magnitude > _relativePositionOfLastFrame.magnitude)
            {
                attacking_status = 1;
                //Debug.Log("aaa");
                _timer_for_attacking = 0;
            }
            else if(_timer_for_attacking > 5 && attacking_status == 1 && _relativePosition.magnitude < 20)
            {
                attacking_status = 2;
                Chasing2();
            }
            switch (attacking_status)
            {
                case 1:
                    if (_timer_for_attacking > 0.4f) { 
                        Chasing3();
                    }
                    break;
                case 2:
                    Rushing(); 
                    break;
                default:
                    break;
            }
        }
        _relativePositionOfLastFrame = target.transform.position - this.transform.position;
        startTrackinLastFrameForAttacking = true;
        _timer_for_attacking += Time.fixedDeltaTime;
        Debug.Log(_timer_for_attacking);
        //Debug.Log(attacking_status);
    }

}
