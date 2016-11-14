using UnityEngine;
using System.Collections;

public class weapon : MonoBehaviour
{

    GameObject centralPoint;
	GameObject monster;
	public GameObject level4;

    // Use this for initialization
    float factor_force, factor_resi_n, factor_resi_tau, weaponRadius;
	bool is_hitting_out, already_hit_other_things, is_hit, already_fired;
    float local_timer;
	Vector3 start_scale;
    void Start()
    {
        //centralPoint = GameObject.FindGameObjectWithTag("CentralPoint");
        factor_force = 100;
        factor_resi_n = 15;
        factor_resi_tau = 15;
        //PublicVariables.weaponRadius = 1; // NTC1
        //weaponRadius = PublicVariables.weaponRadius;
        weaponRadius = 1;
        is_hitting_out = false;
		already_hit_other_things = false;
		is_hit = false;
		start_scale = this.transform.localScale;
		already_fired = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        Gettingback(Time.fixedDeltaTime);
		if (monster != null) {
			HitTheMonster ();
		}
		AdjustColor ();

    }
    public void UpdateCentralPoint(GameObject cP) 
    {
        centralPoint = cP;
    }
	public void UpdateMonster(GameObject mS){
		monster = mS;
	}
    void Gettingback(float deltaTime)
    {
        Vector3 _relativePosition = this.transform.position - centralPoint.transform.position;
        Vector3 _relativeVelocity = GetComponent<Rigidbody>().velocity;// - centralPoint.GetComponent<Rigidbody>().velocity;
        Vector3 _n_direction = -_relativePosition.normalized;
        Vector3 _tau_direction = Vector3.Cross(_n_direction, Vector3.Cross(_relativeVelocity, _n_direction)).normalized;
		if (!is_hit) {
			GetComponent<Rigidbody>().velocity += _relativePosition.magnitude * factor_force * _n_direction * deltaTime*5;
			// resistance on n direction
			GetComponent<Rigidbody>().velocity += -Vector3.Dot(_relativeVelocity, _n_direction) * factor_resi_n * _n_direction * deltaTime*10;
			// resistance on tau direction
			GetComponent<Rigidbody>().velocity += -Vector3.Dot(_relativeVelocity, _tau_direction) * factor_resi_tau * _tau_direction * deltaTime*10;
		}else{
			if (_relativePosition.magnitude > 20) {
				this.transform.localScale = start_scale * ((_relativePosition.magnitude + 20) / 40);
			} else {
				this.transform.localScale = start_scale;
			}
		}
		if (is_hit && !is_hitting_out && Vector3.Dot( this.GetComponent<Rigidbody>().velocity,_relativePosition)> 0 )
        {
            // force for coming back
            GetComponent<Rigidbody>().velocity += _relativePosition.magnitude * factor_force * _n_direction * deltaTime;
            // resistance on n direction
            GetComponent<Rigidbody>().velocity += -Vector3.Dot(_relativeVelocity, _n_direction) * factor_resi_n * _n_direction * deltaTime;
            // resistance on tau direction
            GetComponent<Rigidbody>().velocity += -Vector3.Dot(_relativeVelocity, _tau_direction) * factor_resi_tau * _tau_direction * deltaTime;
            //force for spin
            //GetComponent<Rigidbody>().velocity += -Vector3.Cross(_relativeV elocity, GetComponent<Rigidbody>().angularVelocity) * 0.1f * deltaTime;
        }
		else if (is_hit &&!is_hitting_out)
        {

			GetComponent<Rigidbody>().velocity += _relativePosition.magnitude * factor_force * _n_direction * deltaTime;
			// resistance on n direction
			GetComponent<Rigidbody>().velocity += -Vector3.Dot(_relativeVelocity, _n_direction) * factor_resi_n * _n_direction * deltaTime ;
			// resistance on tau direction
			GetComponent<Rigidbody>().velocity += -Vector3.Dot(_relativeVelocity, _tau_direction) * factor_resi_tau * _tau_direction * deltaTime ;
        }
		else if (is_hit && is_hitting_out)
        {
			local_timer += deltaTime;
			if (local_timer > 2 || already_hit_other_things)
            {
                is_hitting_out = false;
				already_hit_other_things = false;
            }
        }
		if ((this.GetComponent<Rigidbody> ().velocity.magnitude + 1) * (_relativePosition.magnitude + 1) < 1.1f) 
		{
			is_hit = false;
		}
    } 
	public bool CheckIfItIsBack(){
        return (!is_hit);// && already_fired);
	}
    void OnTriggerEnter(Collider c)
    {
		if (c.transform.tag == "Controller" && !is_hit)
        {
            Vector3 _reletive_speed = (c.gameObject.GetComponent<Controller_possition_tracking>().velocity  * 10 - GetComponent<Rigidbody>().velocity);
            Vector3 _reletive_position = (c.gameObject.transform.position - this.transform.position);
            Vector3 _angularVelocityDirection = Vector3.Cross(_reletive_position, _reletive_speed).normalized;
            float _angularVelocityMagnitude = Vector3.Cross(_reletive_position, _reletive_speed).magnitude / _reletive_position.magnitude; 
            GetComponent<Rigidbody>().velocity += c.gameObject.GetComponent<Controller_possition_tracking>().velocity * 10;
            GetComponent<Rigidbody>().angularVelocity += _angularVelocityMagnitude * _angularVelocityDirection * 10;
            is_hitting_out = true;
            local_timer = 0;
			is_hit = true;
			already_fired = true;
        }
    }
	void OnCollisionEnter(Collision colli) {
		Explode ();
	}
	void HitTheMonster(){
		if (monster.activeSelf && is_hitting_out && (this.transform.position - monster.transform.position).magnitude < 3) {
			Explode ();
			Debug.Log ("hit");
		}
	}
	void Explode(){
		Debug.Log ("boom");
		this.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
		this.already_hit_other_things = true;
		if(monster != null){
			if ((this.transform.position - monster.transform.position).magnitude < 10){
				//Monster dies
				//monster.SetActive(false);
				PublicVariables.monsterDead = true;
			}
		}
	}

	void AdjustColor(){
		Color tmp = this.GetComponent<MeshRenderer> ().materials [0].color;
		if (!is_hit) {
			this.GetComponent<MeshRenderer> ().materials [1].color = new Color (tmp.r, tmp.g, tmp.b, 1);
		} else {
			this.GetComponent<MeshRenderer> ().materials [1].color = new Color (tmp.r, tmp.g, tmp.b, 0);

		}
	}
}
