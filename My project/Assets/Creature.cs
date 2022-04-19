using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    //credit to kitgparker for code inspo
    public float speed;//Speed Multiplier
    public float rotation;//Rotation multiplier
    public LayerMask raycastMask;//Mask for the sensors
    // Start is called before the first frame update
    public Brain brain;
    public float[] input = new float[7];

    public float timeElapsed; 
    public bool inbounds;

    private Rigidbody rb;
    private Renderer rend;  
    private int finalStanding; 

    void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rend = gameObject.GetComponent<Renderer>();
        finalStanding = God.creatureCount;
    }
  
    void FixedUpdate()
    {
        input[6] = 1.0f;
        
        if (inbounds)//if the car has not exited the ring, it uses the neural network to get an output
        {
            timeElapsed += Time.fixedDeltaTime;
            //print(timeElapsed);
            for (int i = 0; i < 5; i++)//draws five debug rays as inputs
            {
                Vector3 newVector = Quaternion.AngleAxis(i * 45 - 90, new Vector3(0, 1, 0)) * transform.right;//calculating angle of raycast
                RaycastHit hit;
                Ray Ray = new Ray(transform.position, newVector);

                if (Physics.Raycast(Ray, out hit, 10, raycastMask))
                {
                    input[i] = (10 - hit.distance) / 10;//return distance, 1 being close
                    if(i == 2)
                    {
                        //print("collider: " + hit.collider.transform.name);
                        //input 6 is to allow the creatures to see in "color"
                        if(hit.collider.transform.name == "Death")
                        {
                            input[6] = 0.0f;
                        }
                        else if(hit.collider.transform.name == "Creature(Clone)")
                        {
                            input[6] = 0.5f;
                        }
                        
                    }
                }
                else
                {
                    input[i] = 0;//if nothing is detected, will return 0 to network
                }
            }

            input[5] = (timeElapsed - 5) / 10;

            float[] output = brain.FeedForward(input);//Call to network to feedforward
        
            //transform.Rotate(0, output[0] * rotation, 0, Space.World);//controls the cars movement
            rb.AddTorque(transform.up * rotation * output[0]);
            //transform.position += this.transform.forward * output[1] * speed;//controls the cars turning
            rb.AddForce(transform.forward * output[1] * speed );
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if(other.transform.name == "Cylinder")
        {
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //print(other.transform.name);
        if(other.transform.name == "Death")
        {
            Die();
        }
    }

    void Die()
    {
        inbounds = false;
        rend.enabled = false;
        finalStanding = God.creatureCount;
        God.creatureCount--;
        print(God.creatureCount);
    }

    public void UpdateFitness()
    {
        brain.fitness = finalStanding*-1;//updates fitness of network for sorting
    }
}
