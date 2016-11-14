using UnityEngine;
using System.Collections;

public class Lerp_1 : MonoBehaviour {
    public GameObject target;
    public float chasing_speed;
    public float chasing_acc;
    public float rushing_speed;
    public float rushing_acc;

    public float fastchasing_speed;
    //public float fastchasing_acc;

    int attacking_state;
    Vector3 _relativePositionOfLastFrame;
    bool startTrackinLastFrameForAttacking;
    float _timer_for_attacking;
    // Use this for initialization


    public float dis2;//  慢走和近程攻击的界限
    public float dis1;// 慢走和快走的界限
    public float dis3;// 冲击距离
    public float t1;// 冲的技能CD
    public float t2;// 准备时间
    public float t3;// 冲完继续冲的时间
    public float t4;// 休息时间
    
	void Start () { 
        //target = GameObject.FindGameObjectWithTag("target");
        chasing_speed = 5;
        chasing_acc = 5;
        rushing_speed = 30;
        rushing_acc = 5;
        fastchasing_speed = 20;
        startTrackinLastFrameForAttacking = false;
        _timer_for_attacking = 0;
        attacking_state = 0;
        dis2 = 5;
        dis1 = 20; 
        dis3 = 15;
        t1 = 5;
        t2 = 2;
        t3 = 0.4f;
        t4 = 3;
		this.transform.position = new Vector3 (this.transform.position.x, target.transform.position.y, this.transform.position.z);
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

        Monster_behavior();
    }

    //void Chasing1()
    //{
    //    Vector3 _relativePosition = target.transform.position - this.transform.position;
    //    Vector3 _targetVelocity = target.GetComponent<Rigidbody>().velocity;
    //    Vector3 _targetVelocityOnRelativePosition = Vector3.Dot(_targetVelocity, _relativePosition.normalized) * _relativePosition.normalized;
        
    //}

    void Chasing2(float _speed)
    {
        Vector3 _relativePosition = target.transform.position - this.transform.position;
        Vector3 _targetVelocity = target.GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = _speed * _relativePosition.normalized;
      
        float rotating_angle = Mathf.Acos(Vector3.Dot(_relativePosition.normalized, new Vector3(1, 0, 0))) * 180f / 3.14f;
        Vector3 rotating_axis = Vector3.Cross(new Vector3(1,0,0),_relativePosition).normalized; 
        this.transform.rotation = Quaternion.AngleAxis(rotating_angle, rotating_axis);
    }
    void Chasing3(float _speed, float _acc)
    {
        Vector3 _relativePosition = target.transform.position - this.transform.position;
        Vector3 _targetVelocity = target.GetComponent<Rigidbody>().velocity;
        Vector3 _selfVelocity = this.GetComponent<Rigidbody>().velocity;
        if (_selfVelocity.magnitude > 0)
        {
            Vector3 _tmpVector = Vector3.Cross(_selfVelocity.normalized, _relativePosition.normalized);
            Vector3 _n_direction_times_sin = Vector3.Cross(_tmpVector, GetComponent<Rigidbody>().velocity.normalized);
            this.GetComponent<Rigidbody>().velocity += _acc * _n_direction_times_sin * Time.fixedDeltaTime;
            this.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * _speed;
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

        //switch (attacking_state)
        //{
        //    case 1:
        //        if (_timer_for_attacking > 0.4f) { 
        //            Chasing3();
        //        }
        //        break;
        //    case 2:
        //        Rushing(); 
        //        break;
        //    default:
        //        break;
        //}
        switch (attacking_state)
        {
            case 1:
                Fast_Chasing();
                break;
            case 2:
                Slow_Chasing();
                break;
            case 3:
                Fighting();
                break;
            case 4:
                Preparing_Rushing();
                break;
            case 5:
                Rushing();
                break;
            case 6:
                Rushing_Over();
                break;
            case 7:
                Rest_After_Rushing();
                break;
            case 0:
                Idle();
                break;
            case 8:
                Shielding();
                break;
        }
    }
    void Fast_Chasing()
    {
        Chasing2(fastchasing_speed);
    }
    void Slow_Chasing()
    {
        Chasing3(chasing_speed, chasing_acc);
    }
    void Fighting()
    {
        Chasing3(chasing_speed / 2, chasing_acc);

    }
    void Preparing_Rushing()
    {
        Chasing2(0.01f);
    }
    void Rushing_Over()
    {
        Rushing();
    }
    void Rest_After_Rushing()
    {
        Chasing2(0.01f);
    }
    void Idle()
    {
        Chasing2(0.01f);
    }
    void Shielding()
    {

    }
    void change_state(int state)
    { 
        /* state 1: Fast_Chasing
         * 
         * state 2: Slow_Chasing
         * 
         * state 3: Fighting
         * 
         * state 4: Preparing Rushing
         * 
         * state 5: Rushing
         * 
         * state 6: Rushing_Over
         * 
         * state 7: Rest_After_Rushing
         * 
         * state 0: Idle
         * 
         * state 8: Shielding // not now
         * 
         * 
         */
        switch (state)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
        }
        attacking_state = state;
        Debug.Log(state);
    }
    int Monster_state_machine()
    {
        /* state 1: Fast_Chasing
         * 
         * state 2: Slow_Chasing
         * 
         * state 3: Fighting
         * 
         * state 4: Preparing Rushing
         * 
         * state 5: Rushing
         * 
         * state 6: Rushing_Over
         * 
         * state 7: Rest_After_Rushing
         * 
         * state 0: Idle
         * 
         * state 8: Shielding // not now
         * 
         * 
         */


        Vector3 _relativePosition = target.transform.position - this.transform.position;
        float t = _timer_for_attacking;
        float _dis = _relativePosition.magnitude;
        //if (attacking_state == 2 && _relativePosition.magnitude > _relativePositionOfLastFrame.magnitude)
        //{
        //    attacking_state = 1;
        //    //Debug.Log("aaa");
        //    _timer_for_attacking = 0;
        //}
        //else if (_timer_for_attacking > 5 && attacking_state == 1 && _relativePosition.magnitude < 4)
        //{
        //    attacking_state = 2;
        //    Chasing2();
        //}
        switch (attacking_state)
        {
            case 0:
                if (_dis > dis1)
                {
                    change_state(1);
                }
                if (_dis > dis2 && _dis < dis1)
                {
                    change_state(2);
                }
                if (_dis < dis2)
                {
                    change_state(3);
                }
                _timer_for_attacking = 0;
                break;

            case 1:
                if (_dis < dis1 && t < t1)
                {
                    change_state(2);
                }
                if (t > t1)
                {
                    change_state(4);
                    _timer_for_attacking = 0;
                }
                break;

            case 2:
                if (_dis > dis1 && t < t1)
                {
                    change_state(1);
                }
                if (_dis < dis2 && t < t1)
                {
                    change_state(3);
                }
                if (t > t1 && _dis > dis3)
                {
                    change_state(4);
                    _timer_for_attacking = 0;

                }
                break;

            case 3:
                if (_dis > dis2 )
                {
                    change_state(2);
                }
                if (false && t > t1)
                {
                    change_state(4);
                    _timer_for_attacking = 0;

                }
                break;

            case 4:
                if (t > t2)
                {
                    change_state(5);
                }
                break;

            case 5:
                if ( Vector3.Dot(this.GetComponent<Rigidbody>().velocity, _relativePosition) < 0)
                {
                    change_state(6);
                    _timer_for_attacking = 0;
                }
                break;

            case 6:
                if (t > t3)
                {
                    change_state(7);
                    _timer_for_attacking = 0;
                }
                break;

            case 7:
                if (t > t4)
                {
                    change_state(0);
                }
                break;
        }
        return 0;
    }

    int Monster_behavior()
    {
        _timer_for_attacking += Time.fixedDeltaTime;
        if (!startTrackinLastFrameForAttacking)
        {
            _relativePositionOfLastFrame = target.transform.position - this.transform.position;
            startTrackinLastFrameForAttacking = true;
            return 0;
        }
        Monster_state_machine();
        Attacking();

        return 0;
    }
}
