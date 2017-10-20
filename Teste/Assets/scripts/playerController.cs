using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
	
	private Animator anim;

	private SpriteRenderer render;

	private playerAttack attackController;

	private bool right;
	
	private Vector2 dirct;

	public float maxSpeed = 10f;

    private Rigidbody2D rigidbodyComponent;

	private bool onGround;

	private bool onRightWall;

	private bool onLeftWall;

	private bool onWall;

	private bool wallJumping;

	private bool onStairs;

	private bool midAirJumping;

	private int jumpNumber;

	public int maxJumps;

	public Transform groundCheck;

	public Transform rightCheck;

	public Transform leftCheck;

	public Transform headCheck;

	public Transform chestCheck;

	public float groundRadius = 0.1f;

	public float middleBodyRadius = 1.0f;

	public LayerMask groundMask;

	public LayerMask stairMask;

	public float jumpForce = 200;

	public float pushForce = 100f;

	public float teste = 0;

	private float prevVelocity;

	private bool sClimb;


	// Use this for initialization

	void Awake(){
		rigidbodyComponent = this.GetComponent<Rigidbody2D>();
		render = this.GetComponent<SpriteRenderer>();
		anim = this.GetComponent<Animator>();
		right = true;
		onGround = false;
		maxJumps = 2;
		jumpNumber = 0;
		midAirJumping = false;
		onGround = true;
		sClimb = false;
	}

	void Start () {
		attackController = gameObject.GetComponent<playerAttack>();
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			if(onGround){
				anim.SetTrigger("jump");
				anim.SetBool("onGround", false);
				rigidbodyComponent.AddForce(new Vector2(0, jumpForce));
			} else if ((onLeftWall || onRightWall)){
				if(midAirJumping && (onLeftWall || onRightWall)){
					rigidbodyComponent.velocity = new Vector2(0, rigidbodyComponent.velocity.y);
				}
				if(jumpNumber == 2){
					wallJumping = false;
					return;
				}
				wallJumping = true;
				anim.SetBool("onWall", false);
				if(right)
					rigidbodyComponent.AddForce(new Vector2(-pushForce, jumpForce));
				else 
					rigidbodyComponent.AddForce(new Vector2(pushForce, jumpForce));
				Flip();
				jumpNumber++;
			} else if (onStairs){
				sClimb = true;
			}
		}
	}

    // Update is called once per frame
    void FixedUpdate () {
		if (rigidbodyComponent == null) rigidbodyComponent = this.GetComponent<Rigidbody2D>();
		bool stairF = Physics2D.OverlapCircle(groundCheck.position, groundRadius, stairMask);
		bool stairM = Physics2D.OverlapCircle(chestCheck.position, middleBodyRadius, stairMask);
		bool stairH = Physics2D.OverlapCircle(headCheck.position, groundRadius, stairMask);
		onGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask) || stairF;
		onLeftWall = Physics2D.OverlapCircle(leftCheck.position, groundRadius, groundMask);
		onRightWall = Physics2D.OverlapCircle(rightCheck.position, groundRadius, groundMask);
		
		onWall = onLeftWall || onRightWall;
		onStairs = stairM;

		dirct.y = Input.GetAxis("Vertical");
		dirct.x = Input.GetAxis("Horizontal");

		float accel = rigidbodyComponent.velocity.y - prevVelocity;
		accel = accel / Time.deltaTime;

		prevVelocity = rigidbodyComponent.velocity.y;

		anim.SetFloat("grCh", accel);
		anim.SetBool("onStairs", onStairs);


		anim.SetBool("onGround", onGround);
		anim.SetBool("onWall", onLeftWall || onRightWall);
		anim.SetBool("lWall", onLeftWall);
		anim.SetBool("rWall", onRightWall);
		anim.SetInteger("jumpN", jumpNumber);

		if(onGround){
			wallJumping = false;	
			jumpNumber = 0;
		}

		anim.SetFloat("vSpeed", rigidbodyComponent.velocity.y);
		anim.SetFloat("vSpeedMod", Mathf.Abs(rigidbodyComponent.velocity.y));
		if(!wallJumping){
			if(!attackController.isAttacking()){//If is not attacking, moving animation
				if(WallMove()){
					NormalMove();
				}
			} else {
				rigidbodyComponent.velocity = new Vector2(0, rigidbodyComponent.velocity.y);
			}
		}

		if(midAirJumping && (onLeftWall || onRightWall)){
			wallJumping = false;
		}

		midAirJumping = !(onLeftWall || onRightWall);
		midAirJumping = midAirJumping && !onGround;
		anim.SetBool("midAirWall", midAirJumping);
		
	}

	private bool WallMove(){
		if(!onWall || jumpNumber == 0){
			return true;
		}
		if(((right && dirct.x > 0) || (!right && dirct.x < 0))){
			anim.SetFloat("Speed", Mathf.Abs(dirct.x));
			rigidbodyComponent.velocity = new Vector2(dirct.x * maxSpeed, rigidbodyComponent.velocity.y);
		} else {
			rigidbodyComponent.velocity = new Vector2(0, rigidbodyComponent.velocity.y);
		}
		return false;
	}

	private void NormalMove(){
		anim.SetFloat("Speed", Mathf.Abs(dirct.x));

		if(dirct.x > 0 && !right){
			Flip();
		} else if (dirct.x < 0 && right){
			Flip();
		}
		rigidbodyComponent.velocity = new Vector2(dirct.x * maxSpeed, rigidbodyComponent.velocity.y);
	}

	public bool isOnGround(){
		return onGround;
	}

	void Flip(){
		right = !right;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
}
