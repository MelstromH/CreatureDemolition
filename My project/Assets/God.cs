using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    //Credit to kipgparker for code inspo

    public float timeframe;
    public int populationSize;//creates population size
    public GameObject prefab;//holds bot prefab
    public GameObject ring;
    public Vector3 point;
    public static int creatureCount;

    public int[] layers = new int[4] { 7, 4, 4, 2 };//initializing network to the right size

    [Range(0.0001f, 1f)] public float MutationChance = 0.01f;

    [Range(0f, 1f)] public float MutationStrength = 0.5f;

    [Range(0.1f, 10f)] public float Gamespeed = 5f;

    //public List<Bot> Bots;
    public List<Brain> brains;
    private List<Creature> creatures;

    void Start()// Start is called before the first frame update
    {
        if (populationSize % 4 != 0)
            populationSize = 40;//if population size is not even, sets it to fifty

        Initbrains();
        InvokeRepeating("CreateCreatures", 0.1f, timeframe);//repeating function

        point = new Vector3(0,0,0);
        
    }

    public void Initbrains()
    {
        brains = new List<Brain>();
        for (int i = 0; i < populationSize; i++)
        {
            Brain brain = new Brain(layers);
            //brain.Load("Assets/Pre-trained.txt");//on start load the network save
            brains.Add(brain);
        }
    }

    public void CreateCreatures()
    {
        ring.transform.localScale = new Vector3(20, .5f, 20);
        creatureCount = populationSize;
        Time.timeScale = Gamespeed;//sets gamespeed, which will increase to speed up training
        if (creatures != null)
        {
            for (int i = 0; i < creatures.Count; i++)
            {
                GameObject.Destroy(creatures[i].gameObject);//if there are Prefabs in the scene this will get rid of them
            }

            Sortbrains();//this sorts brains and mutates them
        }

        creatures = new List<Creature>();

        //spawn the creatures in a ring
        for (int i = 0; i < populationSize; i++)
        {
            var radians = 2 * Mathf.PI / populationSize * i;
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            var spawnDir = new Vector3 (horizontal, 0, vertical);

            var spawnPos = point + spawnDir * 9;

            Creature creature = (Instantiate(prefab, spawnPos, new Quaternion(0, 0, 1, 0))).GetComponent<Creature>();//create bots

            creature.transform.Translate(Vector3.back, Space.World);
            creature.transform.LookAt(point);

            creature.brain = brains[i];//deploys network to each learner
            creatures.Add(creature);

            
        }
        
    }

    public void Sortbrains()
    {
        for (int i = 0; i < populationSize; i++)
        {
            creatures[i].UpdateFitness();//gets bots to set their corrosponding brains fitness
        }
        brains.Sort();
        brains[populationSize - 1].Save("Assets/Save.txt");//saves brains weights and biases to file, to preserve network performance
        for (int i = 0; i < populationSize / 4; i++)
        {
            brains[i] = brains[i + populationSize / 4].copy(new Brain(layers));
            brains[i].Mutate((int)(1/MutationChance), MutationStrength);
        }
    }
}