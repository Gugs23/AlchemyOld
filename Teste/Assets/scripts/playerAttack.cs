using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttack : MonoBehaviour {

	playerController pc;

	private Animator anim;

	public Collider2D attackTrigger;

	private float attackTimer = 0.0f;

	public float attackCd = 0.3f;

	private bool attacking = false;

	void Awake(){
		anim = gameObject.GetComponent<Animator>();
		attackTrigger.enabled = false;
	}

	// Use this for initialization
	void Start () {
		pc = gameObject.GetComponent<playerController>();
	}
	
	// Update is called once per frame
	void Update () {
		bool grounded = pc.isOnGround();
		if(Input.GetKeyDown(KeyCode.C) && !attacking && grounded){
			attacking = true;
			attackTimer = attackCd;
			attackTrigger.enabled = true;
		}

		if(attacking){
			if(attackTimer > 0){
				attackTimer -= Time.deltaTime;
			} else {
				attacking = false;
				attackTrigger.enabled = false;
			}
		}

		anim.SetBool("Attack1", attacking);
		anim.SetFloat("attackTimer", attackTimer);
	}

	public bool isAttacking(){
		return attacking;
	}

}
