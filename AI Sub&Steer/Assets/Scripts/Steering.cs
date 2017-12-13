using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(CharacterController))]

public class Steering : MonoBehaviour
{


    //movement
    protected Vector3 acceleration;
    protected Vector3 velocity;
    protected Vector3 desired;
    public Vector3 targetPos;
    public GameObject enemy;
    public float seekWeight = 200.0f;
    public float fleeWeight = 1000.0f;

    public float maxVelocity = 8.0f;
    public float maxForce = 10.0f;
    public float mass = 1.0f;
    public float radius = 1.0f;

    //health
    
    
    public GameObject food;

    public GameObject croc;
    int maxHealth = 100;
    public bool FoodRespawn = false;
    public float foodTimer = 0.0f;
    public float currentHealth = 100;
    public GameObject healthBar;


    //access to Character Controller component
    CharacterController controller;



    public Vector3 Velocity
    {
        get { return velocity; }
    }

    public void Start()
    {

        InvokeRepeating("decreaseHealth", 2f, 4f); //to decrease health by default
        acceleration = Vector3.zero;
        velocity = transform.forward;

        controller = GetComponent<CharacterController>();
        
    }

    private void SeekToTarget(Vector3 target)
    {
        CalculateSteeringForces(target);
        Vector3 down = new Vector3(0, -1, 0);
        down = down * maxVelocity;
        velocity += acceleration * Time.deltaTime;
        velocity.y = 0;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        controller.Move(velocity * Time.deltaTime);
        controller.Move(down * Time.deltaTime);
        //initialize the acceleration
        acceleration = Vector3.zero;
        transform.forward = velocity.normalized;
    }

    /// <summary>
    /// This is showing how to flee from the target
    /// </summary>
    /// <param name="target"></param>
    private void FleeFromTarget(Vector3 target)
    {
        CalculateSteeringForcesForFlee(target);
        Vector3 down = new Vector3(0, -1, 0);
        down = down * maxVelocity;
        velocity += acceleration * Time.deltaTime;
        velocity.y = 0;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        controller.Move(velocity * Time.deltaTime);
        controller.Move(down * Time.deltaTime);
        //initialize the acceleration
        acceleration = Vector3.zero;
        transform.forward = velocity.normalized;
    }

    private Vector3 force;
    private Vector3 currentTarget;
    // Update is called once per frame
    protected void Update()
    {

        //subsumption
        //check if dead
        //flee if health is low
        //check if enemy is nearby, seek
        //seek food

        //calculating distance to decide which to seek
        food = FindClosestFood();
        string enemyName;
        var currentName = gameObject.name;
        if (currentName == "Croc1")
        {
            enemyName = "Croc2";
        }
        else
        {
            enemyName = "Croc1";
        }
        enemy = GameObject.Find(enemyName);
        float enemydistance = Vector3.Distance(enemy.transform.position, transform.position);
        float fooddistance = Vector3.Distance(food.transform.position, transform.position);



        if (currentHealth > 0)
        {

            Debug.Log("ENEMY DIST: " + enemydistance);
            Debug.Log("FOOD DIST: " + fooddistance);

            if (fooddistance > enemydistance)
            {
                if (currentHealth > 30.0f)
                {
                    SeekToTarget(enemy.transform.position);
                    
                    
                    
                }
                else if(currentHealth < 30.0f)
                {
                    //Debug.Log("FLEEING");
                    FleeFromTarget(enemy.transform.position);
                    SeekToTarget(food.transform.position);

                }
            }
            else if (enemydistance > fooddistance)
            {
                SeekToTarget(food.transform.position);

                //Debug.Log("SEEKING FOOD");
            }

        }
         else if (currentHealth == 0) 
        {
            Destroy(gameObject);
        }

    }



    protected void CalculateSteeringForces(Vector3 target)
    {
        force = Vector3.zero;
        force += Seek(target) * seekWeight;
        force = Vector3.ClampMagnitude(force, maxForce);
        ApplyForce(force);
    }

    protected void CalculateSteeringForcesForFlee(Vector3 target)
    {
        force = Vector3.zero;
        force += Fleeing(target) * fleeWeight;
        force = Vector3.ClampMagnitude(force, maxForce);
        ApplyForce(force);
    }

    protected void ApplyForce(Vector3 steeringForce)
    {
        acceleration += steeringForce / mass;


    }




    //find closest food
    public GameObject FindClosestFood()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("food");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;

    }




    //Seeking the target
    public Vector3 Seek(Vector3 Pos)
    {

        desired = (Pos - transform.position).normalized * maxVelocity;
        desired.y = 0;
        desired -= velocity;
        return desired;


    }
    //attack enemy if health is more than 20, else flee


    protected Vector3 Fleeing(Vector3 Pos)
    {

        return (-Seek(Pos));

    }

    //decreasing health over time
    void decreaseHealth()
    {
        currentHealth -= 2f;
        float calcHealth = currentHealth / maxHealth;
        setHealth(calcHealth);

    }
    public void decreaseHealthFromDamage()
    {
        currentHealth -= 30f;
        setHealth(currentHealth / maxHealth);
    }

 
    //passing info to healthBar
    public void setHealth(float crocHealth)
    {
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(crocHealth, 0f, 1f), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        //0f, 1f ---> so it doesn't overflow

    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "food")
        {
            if(currentHealth < maxHealth)
            currentHealth = currentHealth + 10;
            setHealth(currentHealth);
            other.gameObject.SetActive(false);
            foodTimer += Time.deltaTime;


        }
        else if (other.gameObject.tag == "enemy" && (this.gameObject.name != other.gameObject.name))
        {

            decreaseHealthFromDamage();

        }

    }






}


