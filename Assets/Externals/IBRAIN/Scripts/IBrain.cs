// Source : https://www.youtube.com/watch?v=RGjMBEGhd7Y

using UnityEngine;
using UnityEngine.AI;

public class IBrain : MonoBehaviour
{
    [Header("Settings")]
    public Mode mode;
    public Type type;

    public bool AutoSpeed;
    public float speed;
    public int Health = 100;
    private AudioClip footStepAudio;

    private float m_fleeTriggerDistance = 12f;
    private float m_fleeSafeDistance = 20f;

    //[Header("Target info")]
    private GameObject Target;
    private Vector3 TargetPosition;

    [Header("Waypoint Settings")]
    public bool FindWaypoint;
    public bool ClosestWaypoint;
    private GameObject[] waypoints;
    public GameObject Waypoint;
    private GameObject waypointmp;
    private Vector3 WaypointPosition;
    private float WaypointDistance;
    private int currentWaypoint = 0;

    private float AvatarRange = 25;
    //private float SpeedDampTime = .25f;
    //private float DirectionDampTime = .25f;

    protected UnityEngine.AI.NavMeshAgent agent;
    private Animator animator;
    protected Locomotion locomotion;
    private NavMeshHit hit;
    private bool blocked = false;

    [Header("Weapon Settings")]
    public GameObject Weapon;
    public int Ammo = 0;
    public int ShootRange = 5;
    public GameObject Enemy;
    private GameObject[] Enemys;
    private WeapMod WeaponMod;
    private Weapon weascr;

    public Transform m_player;

    public enum Type
    {
        //	Zombie,
        Human,
    }

    public enum Mode
    {
        Waypoint,
        Chase,
        Random,
        Armed,
        Flee
    }

    public enum WeapMod
    {
        Unarmed,
        Armed,
        Reload,
        Shot,
    }

    void Start()
    {
        if (Weapon == null)
            WeaponMod = WeapMod.Unarmed;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;

        animator = GetComponent<Animator>();
        locomotion = new Locomotion(animator);

        //if (Type.Zombie == type)
        //	animator.SetBool ("ZombieMod", true);

        if (Type.Human == type)
            animator.SetBool("HumanMod", true);

        //Enemys = GameObject.FindGameObjectsWithTag("Enemy");


        //if (Mode.Chase == mode)
        //{
        //    Target = GameObject.FindWithTag("Player");
        //}

        //if (Mode.Armed == mode)
        //{
        //    Enemy = GameObject.FindWithTag("Enemy");
        //    if (Enemy != null)
        //        Target = GameObject.FindWithTag("Gun");
        //}

        //if (Mode.Random == mode)
        //{
        //    random();
        //}

        //// Source : https://www.youtube.com/watch?v=RGjMBEGhd7Y
        //if (Mode.Flee == mode)
        //{
        //    Debug.Log("Start flee mode");
        //    RunAway();
        //}

        if (FindWaypoint == true && !Waypoint && Mode.Waypoint == mode)
        {
            Waypoint = GameObject.FindWithTag("WaypointBase");
            waypoints = new GameObject[Waypoint.transform.childCount];
            for (int i = 0; i < Waypoint.transform.childCount; i++)
            {
                waypoints[i] = Waypoint.transform.GetChild(i).gameObject;
            }

        }

        if (ClosestWaypoint == true)
            FindNearWay();
    }


    void OnCollisionEnter(Collision collision)
    {
        //Health--;
    }

    void OnTriggerEnter(Collider other)
    {
        if (Type.Human == type)
        {

            //if (other.CompareTag("Jump"))
            //{
            //    animator.SetTrigger("Jump");
            //}

            //if (other.CompareTag("Dive"))
            //{
            //    animator.SetTrigger("Dive");
            //}

            //if (other.CompareTag("Climb"))
            //{
            //    animator.SetTrigger("Climb");
            //}
            //if (other.CompareTag("Gun") && Mode.Armed == mode)
            //{
            //    animator.SetBool("Pickup", true);

            //    Weapon = other.gameObject;
            //    var Spine = GameObject.Find("RightHand").transform;
            //    other.gameObject.tag = "Untagged";

            //    other.transform.parent = Spine;
            //    //Sholder//
            //    //other.transform.localPosition = new Vector3(0.158f,-0.237f,-0.072f);
            //    //other.transform.localEulerAngles  = new Vector3(-80.01601f,-377.131f,186.03f); 
            //    //RightHand//
            //    other.transform.localPosition = new Vector3(0.1233f, -0.0124f, -0.0072f);
            //    other.transform.localEulerAngles = new Vector3(-2.854f, -101.101f, 252.547f);
            //    Target = null;
            //    WeaponMod = WeapMod.Armed;

            //}

        }
    }

    // Source : https://www.youtube.com/watch?v=RGjMBEGhd7Y
    void CheckDistance()
    {
        //Debug.Log("Run Away");
        
        float distanceNPCvsBlob = Vector3.Distance(m_player.position, transform.position);
        //Debug.Log("Player position : " + m_player.position);
        //Debug.Log("transform.position : " + transform.position);
        //Debug.Log("Actual distance : " + distanceNPCvsBlob);
        //Debug.Log("To be smaller  to run : ");
        //Debug.Log("Flee trigger distance : " + m_fleeTriggerDistance);
        if (distanceNPCvsBlob < m_fleeTriggerDistance)
        {
            //Debug.Log("Running");
            //float magnitude = distanceNPCvsBlob.magnitude;
            Vector3 opositeDirection = (m_player.position - transform.position).normalized;
            MoveToPos(transform.position - (opositeDirection));
        }
        else if (distanceNPCvsBlob > m_fleeSafeDistance)
        {
            //Debug.Log("Walking");
            Walking();
        }
    }

    private void MoveToPos(Vector3 position)
    {
        //if (WaypointDistance < 1 || blocked)
        //{
            //agent.SetDestination(position);
            TargetPosition = position;
            WaypointDistance = Vector3.Distance(TargetPosition, transform.position);
            speed = 10;
            SetDestinationTarget();
        //}
        //blocked = NavMesh.Raycast(transform.position, TargetPosition, out hit, NavMesh.AllAreas);
        //Debug.DrawLine(transform.position, TargetPosition, blocked ? Color.red : Color.green);
    }

    void Walking()
    {
        if (WaypointDistance < 1 || blocked)
        {
            TargetPosition = new Vector3(UnityEngine.Random.Range(-AvatarRange, AvatarRange), 0, UnityEngine.Random.Range(-AvatarRange, AvatarRange));
            WaypointDistance = Vector3.Distance(TargetPosition, transform.position);
            speed = 1;
            SetDestinationTarget();
        }
        blocked = NavMesh.Raycast(transform.position, TargetPosition, out hit, NavMesh.AllAreas);
        //Debug.DrawLine(transform.position, TargetPosition, blocked ? Color.red : Color.green);
    }
    void random()
    {
        if (WaypointDistance < 1 || blocked)
        {
            TargetPosition = new Vector3(UnityEngine.Random.Range(-AvatarRange, AvatarRange), 0, UnityEngine.Random.Range(-AvatarRange, AvatarRange));
            WaypointDistance = Vector3.Distance(TargetPosition, transform.position);
            speed = 1;
            SetDestinationTarget();
        }
        blocked = NavMesh.Raycast(transform.position, TargetPosition, out hit, NavMesh.AllAreas);
        Debug.DrawLine(transform.position, TargetPosition, blocked ? Color.red : Color.green);
    }

    protected void SetDestination()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
            speed = 5f;

        Quaternion q = new Quaternion();
        if (hit.normal != Vector3.zero)
            q.SetLookRotation(hit.normal, Vector3.forward);
            agent.destination = hit.point;
    }


    protected void SetDestinationTarget()
    {
        var ray = TargetPosition;
        RaycastHit hit = new RaycastHit();
        Quaternion q = new Quaternion();
        if (hit.normal != Vector3.zero)
            q.SetLookRotation(hit.normal, Vector3.forward);
            agent.destination = TargetPosition;
    }

    protected void SetupAgentLocomotion()
    {
        if (AgentDone())
        {
            locomotion.Do(0, 0);

        }
        else
        {
            if (AutoSpeed)
                speed = agent.desiredVelocity.magnitude;

            if (WaypointDistance > 10 && Mode.Chase == mode)
            {
                speed = 5;

            }

            if (WaypointDistance < 2 && Mode.Chase == mode)
            {
                speed = 0;
                animator.SetBool("Fight", true);

            }
            if (WaypointDistance > 2 && Mode.Chase == mode)
            {
                animator.SetBool("Fight", false);

            }

            Vector3 velocity = Quaternion.Inverse(transform.rotation) * agent.desiredVelocity;
            float angle = Mathf.Atan2(velocity.x, velocity.z) * 180.0f / 3.14159f;
            locomotion.Do(speed, angle);
        }
    }

    void OnAnimatorMove()
    {
        agent.velocity = animator.deltaPosition / Time.deltaTime;

        transform.rotation = animator.rootRotation;
    }

    protected bool AgentDone()
    {
        return !agent.pathPending && AgentStopping();
    }

    protected bool AgentStopping()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
    }

    void FindNearWay()
    {
        GameObject closest2;
        var distance = Mathf.Infinity;
        var position = transform.position;
        foreach (GameObject go in waypoints)
        {
            var diff2 = (go.transform.position - position);
            var curDistance2 = diff2.sqrMagnitude;
            var pos = go.transform.position;
            if (curDistance2 < distance)
            {
                closest2 = go;
                waypointmp = go;
                distance = curDistance2;
                int index = closest2.transform.GetSiblingIndex();
                currentWaypoint = index;
                WaypointPosition = waypointmp.transform.position;
            }
        }
    }

    void Update()
    {
        if (Health != 0)
        {

            if (Target)
                TargetPosition = Target.transform.position;

            if (Target != null)
                SetDestinationTarget();
        }
    }

    void FixedUpdate()
    {
        if (Health != 0)
        {
            //Debug.Log("Health not 0");
            // Source : https://www.youtube.com/watch?v=RGjMBEGhd7Y
            if (Mode.Flee == mode)
            {
                //Debug.Log("Check Distance");
                CheckDistance();
            }

            //if (Mode.Random == mode)
            //{
            //    random();
            //}


            if (Mode.Waypoint == mode)
            {

                if (FindWaypoint == true && !Waypoint)
                {
                    Waypoint = GameObject.FindWithTag("WaypointBase");

                    if (Waypoint)
                    {
                        waypoints = new GameObject[Waypoint.transform.childCount];

                        for (int i = 0; i < Waypoint.transform.childCount; i++)
                        {
                            waypoints[i] = Waypoint.transform.GetChild(i).gameObject;
                        }
                    }
                }


                for (int i = currentWaypoint; 1 > WaypointDistance; i++)
                {

                    if (waypoints.Length <= i)
                        i = 0;

                    currentWaypoint = i;
                    waypointmp = waypoints[i];
                    WaypointPosition = waypoints[i].transform.position;
                    WaypointDistance = Vector3.Distance(WaypointPosition, transform.position);
                    Target = waypointmp.gameObject;

                }

                if (speed == 0)
                    AutoSpeed = true;

            }

            if (Mode.Armed == mode)
            {

                if (Enemy == null)
                    Enemy = GameObject.FindWithTag("Enemy");

                if (WeapMod.Unarmed == WeaponMod && Enemy != null)
                    Target = GameObject.FindWithTag("Gun");

                if (WeapMod.Armed == WeaponMod && Enemy != null)
                {
                    Target = Enemy;
                    weascr = Weapon.GetComponent<Weapon>();
                    Ammo = weascr.Ammo;

                    if (WaypointDistance < ShootRange && Enemy != null && Ammo > 0)
                    {
                        animator.SetBool("Shoot", true);
                        weascr.Shoot = true;
                        speed = 1;
                        AutoSpeed = false;
                    }
                    else
                    {
                        weascr.Shoot = false;
                        animator.SetBool("Shoot", false);
                        AutoSpeed = true;
                    }
                }
            }


            SetupAgentLocomotion();

            WaypointDistance = Vector3.Distance(TargetPosition, transform.position);

        }
        else
        {
            animator.SetBool("Die", true);
            animator.SetBool("Fight", false);
        }


        if (animator.GetBool("Die") == true && Health > 1)
            animator.SetBool("Die", false);
    }
}
