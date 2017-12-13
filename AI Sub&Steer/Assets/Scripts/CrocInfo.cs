using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrocInfo : MonoBehaviour
{

    public GameObject food;

    public GameObject croc;
    int maxHealth = 100;
    public float currentHealth = 100;
    public GameObject healthBar;
    public bool FoodRespawn = false;
    public float foodTimer = 0.0f;




    // Use this for initialization
    void Start()
    {

        InvokeRepeating("decreaseHealth", 2f, 2f);
    }

    // Update is called once per frame
    void Update()
    {



    }
    //decreasing health over time
    void decreaseHealth()
    {
        currentHealth -= 2f;
        float calcHealth = currentHealth / maxHealth;
        setHealth(calcHealth);

    }
    //passing info to healthBar
    public void setHealth(float crocHealth)
    {
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(crocHealth, 0f, 1f), healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        //0f, 1f ---> so it doesn't overflow

    }
    //pickup food for health


    //health and damage
    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "food")
        {
            currentHealth = currentHealth + 10;
            setHealth(currentHealth);
            other.gameObject.SetActive(false);
            foodTimer += Time.deltaTime;


        }
        else if (other.gameObject.tag == "enemy" && (this.gameObject.name != other.gameObject.name))
        {

            this.GetComponent<Steering>().decreaseHealthFromDamage();

        }

    }


}




