using UnityEngine;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour// : RaycastController
{
    protected struct PassengerMovement
    {
        public Transform transform { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 relativePosition { get; set; }
        public bool standingOnPlatform { get; set; }
        public bool moveBeforePlatform { get; set; }
    }

    [SerializeField] protected LayerMask passengerMask;

    protected Vector3 move = Vector3.one;

    private List<PassengerMovement> passengerMovement;

    private BoxCollider2D _collider;

    protected virtual void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    Vector3 velocity = CalculatePlatformMovement();
        
        CalculatePassengerMovement(velocity, Time.deltaTime);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    protected virtual Vector3 CalculatePlatformMovement()
    {
        return move*Time.deltaTime;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (var passenger in passengerMovement)
        {
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passenger.transform.GetComponent<Controller2D>().Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity, float rewindAmount = 0.1f)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionY = Mathf.Sign(velocity.y);

        //used to calculate anyone hanging onto the back of the platform
        Vector3 rewind = velocity.normalized * rewindAmount;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            _collider.bounds.center - rewind,
            _collider.bounds.size,
            0f,
            velocity.normalized,
            velocity.magnitude + rewindAmount,
            passengerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (movedPassengers.Add(hit.transform))
            {
                passengerMovement.Add(new PassengerMovement
                {
                    transform = hit.transform,
                    velocity = velocity,
                    relativePosition = hit.transform.position - transform.position,
                    standingOnPlatform = hit.normal == Vector2.down,
                    moveBeforePlatform = directionY > 0
                });
            }
        }
    }
}
