//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;


//public class MovementSpecial : MonoBehaviour
//{
//    //player and input components
//    protected Player player;
//    protected InputController input;
//    protected Controller2D controller;

//    [SerializeField]
//    protected float specialTime;
//    [SerializeField]
//    protected float cooldownTime;


//    private float specialTimer;
//    private float cooldownTimer;

//    //bools for determining current state
//    public bool canUse { get { return !isInUse && !isDisabled; } }
//    public bool isInUse { get { return specialTime >= 0; } }
//    public bool isDisabled { get; set; }


//    // Use this for initialization
//    protected virtual void Start()
//    {
//        isDisabled = false;
//    }
    
	
//	// Update is called once per frame
//	void Update ()
//    {
//        specialTimer -= Time.deltaTime;
//        cooldownTimer -= Time.deltaTime;
//    }

//    //Used when the movement special button is hit
//    public virtual void activate()
//    {
//    }

   

//}
