using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameScriptX : MonoBehaviour
{
    public Animator gunanimator;
    private bool[] Chasing;
    public GameObject hitmarker;
    public GameObject scope;
    public Camera BigCam;
    public int numberoftrees;
    public int numberofdroids;
    public int numberofScreechers;
    public int NumberofResources;
    public int totalnumberofitems;
    public GameObject[] deaddroid;
    public PhysicMaterial physics;
    //weapons. each weapon has a location relative to the ui, a location for muzzle smoke and flashes, and a location for casings to come from. their [#]s should match.
    public List<int> inventory;
    public float[] gunscalex, gunscaley, gunscalez;
    public int[] Weapons; //weaponslist
    public Vector3[] Barrels; //barrel locations
    public Vector3[] Chambers;   //where bullet casings come from
    public Vector3[] DefaultGunTransforms; //gun mesh start locations in relation to character ui
    public Vector3[] DefaultGunRotations;
    public Mesh[] weaponmeshes; //meshlist
    public GameObject gunmeshobject; //holds the mesh
    public GameObject gun; //holds the entire gun for easier scripted animations
    public MeshFilter gunmesh; //the component that is mesh
    public GameObject barrelmuzzle; //end of gun
    public GameObject chamberOpening; //the chamber opening
    public int Weaponselectednumber; //this number will be used as a fill in for gun related values, so weapons can be changed easily. ****** which inventory spot the player has drawn ******
    public AudioSource[] GunSound;
    public GameObject bullet;
    public GameObject BulletCasing;
    public float range;
    public int damage;
    public GameObject gunsmoke;
    public GameObject muzzleflash;
    public GameObject bullethits;
    public float[] weapondamage;
    public float enemyupdatetimer;


    public Camera cam;
    public GameObject baseobjectprefab;
    public GameObject[] trees;
    public GameObject[] enemies;
    public GameObject[] Resources;
    public GameObject[] DeadResources;
    public int[] ResourceType;
    //the following values constitute a list of items and their locations and bodies. 
    
    private Vector3[] EntityLocations;
    private Transform[] EntityLocales;
    private GameObject[] items;
    private float[] Health;
    private float[] DistanceToPlayer;
    private Rigidbody[] Rigidbodies;
    private float[] preliminarytimers;
    //the following values hold which types of items exist
    private int[] EntityClass;
    //0=player;
    //1=trees;
    //2=resources;
    //3=enemies;
    private float[] Defense;
    private int[] EnemyType;
    
    //these all deal with character movements
    public float speed;
    public float maxvelocity;
    public float mouseSensitivity = 25.0F;
    public float clampAngle = 80.0F;
    float rotX = 0.001F, rotY = 0.0F;
    public float rotationSpeed = 3;
    public float characterheight;
    public float charactergirth;

    public float distancethreshold;
    private float timer;
    private float updatetimer;


    // Start is called before the first frame update
    void Start()
    {
        StartTerrainGeneration();
        gunanimator = gunanimator.GetComponent<Animator>();
        //map details
        ResourceType = new int[totalnumberofitems];
        EntityLocations = new Vector3[totalnumberofitems];
        EntityLocales = new Transform[totalnumberofitems];
        items = new GameObject[totalnumberofitems];
        Health = new float[totalnumberofitems];
        DistanceToPlayer = new float[totalnumberofitems];
        Rigidbodies = new Rigidbody[totalnumberofitems];
        preliminarytimers = new float[totalnumberofitems];
        EntityClass = new int[totalnumberofitems];
        Defense = new float[totalnumberofitems];
        EnemyType = new int[totalnumberofitems];
        Chasing = new bool[totalnumberofitems];
        //deaddroid = new GameObject[totalnumberofitems];

        //player locations
        EntityLocations[0] = new Vector3(500, 0, 500);
        //EntityLocales[0].position = new Vector3(500,0,500);
        //gunsway
        weapondamage[0] = 30;
        enemyupdatetimer = 3;
        items[0] = Instantiate(baseobjectprefab, EntityLocales[0]);
        items[0].AddComponent<Rigidbody>();
        items[0].layer = 8;
        items[0].AddComponent<CapsuleCollider>();
        items[0].layer = 0;
        //StartTerrainGeneration();
        Cursor.visible = false;

        inventory.Add(0);
        //starting location and weapon
        Weaponselectednumber = 0;
        this.transform.position = new Vector3(500, 0, 500);

        //setting entity classes or item types
        EntityClass[0] = 0;

        //secondary update timer
        timer = 3;

        // for (int i = 0; i < EntityLocations.Length; i++)
        // {
        //      Rigidbodies[i] = items[i].GetComponent<Rigidbody>();
        //  }
        //character settings
        items[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        items[0].GetComponent<Rigidbody>().mass = 10;
        items[0].GetComponent<CapsuleCollider>().height = characterheight;
        cam.transform.parent = items[0].transform;
        //gun things / positions
        gun.transform.parent = cam.transform;
        gun.transform.localPosition = new Vector3(0, 0, 0);

        gunmesh.mesh = weaponmeshes[inventory[Weaponselectednumber]];
        gunmeshobject.transform.localPosition = DefaultGunTransforms[0];
        barrelmuzzle.transform.parent = gunmesh.transform;
        barrelmuzzle.transform.localPosition = Barrels[inventory[Weaponselectednumber]];
        chamberOpening.transform.parent = gunmesh.transform;
        chamberOpening.transform.localPosition = Chambers[inventory[Weaponselectednumber]];
        Rigidbodies[0] = items[0].GetComponent<Rigidbody>();
        items[0].transform.position = new Vector3(500, 50, 500);


        TreeSpawn();
        ResourceSpawn();

        enemyspawn();
        
        GameEntry();
        updatetimer = 25;

    }
    private void GameEntry()
    {
        Weaponselectednumber = 0;
        gunmeshobject.transform.localScale = new Vector3(gunscalex[inventory[Weaponselectednumber]], gunscaley[inventory[Weaponselectednumber]], gunscalez[inventory[Weaponselectednumber]]);
    }
    private void TreeSpawn()
    {
        for (int i = 1; i < numberoftrees; i++)
        {
            Vector3 localtree = new Vector3((Random.Range(0, 1000)), 60, (Random.Range(0, 1000)));
            var x = Mathf.RoundToInt(Random.Range(0, trees.Length));
            GameObject Trei = Instantiate(trees[x]);
            Trei.transform.localEulerAngles = new Vector3(Random.Range(0, 6), Random.Range(0, 6), Random.Range(0, 6));
            Trei.transform.position = localtree;
            items[i] = Trei;
            EntityLocations[i] = Trei.transform.position;
            EntityClass[i] = 1;
            items[i].isStatic=true;

            RaycastHit hit;
            Physics.Raycast(localtree, -Trei.transform.up, out hit, 800);
            Trei.transform.position = hit.point;
            EntityLocations[i] = Trei.transform.position;
            //if (x == 9)
            // {
            //     EntityClass[i] = 9;
            // }
        }
    }
    private void ResourceSpawn()
    {
        var fuck = numberoftrees + NumberofResources;
        for (int i = numberoftrees; i < fuck; i++)
        {
            Vector3 localtreez = new Vector3((Random.Range(0, 1000)), 60, (Random.Range(0, 1000)));
            var xz = Mathf.RoundToInt(Random.Range(0, Resources.Length));
            GameObject Trei = Instantiate(Resources[xz]);
            Trei.transform.localEulerAngles = new Vector3(Random.Range(0, 6), Random.Range(0, 6), Random.Range(0, 6));
            Trei.transform.position = localtreez;
            items[i] = Trei;
            EntityLocations[i] = Trei.transform.position;
            items[i].isStatic = true;
            EntityClass[i] = 2;
            ResourceType[i] = xz;

            RaycastHit hit;
            Physics.Raycast(localtreez, -Trei.transform.up, out hit, 800);
            Trei.transform.position = hit.point;
            EntityLocations[i] = Trei.transform.position;
        } 
    }
    private void enemyspawn()
    {
        var treeanre = NumberofResources + numberoftrees;
        var goy = treeanre+ numberofdroids;
        var scritch = goy + numberofScreechers;
        for (int i = treeanre; i < goy; i++) //droids
        {
            Vector3 NewDroid = new Vector3((Random.Range(0, 1000)), 40, Random.Range(0, 1000));
            GameObject Droid = Instantiate(enemies[0]);
            Droid.transform.position = NewDroid;
            items[i] = Droid;
            EntityLocations[i] = Droid.transform.position;
            preliminarytimers[i] = 5;
            Defense[i] = .7f;
            Health[i] = 100;
            EntityClass[i] = 3;
            Rigidbodies[i] = Droid.GetComponent<Rigidbody>();
            EnemyType[i] = 0;

        }
        for (int i = goy; i < scritch; i++) //screechers
        {
            Vector3 NewDroid = new Vector3((Random.Range(0, 1000)), 40, Random.Range(0, 1000));
            GameObject Droid = Instantiate(enemies[1]);
            Droid.transform.position = NewDroid;
            items[i] = Droid;
            EntityLocations[i] = NewDroid;
            preliminarytimers[i] = 5;
            Defense[i] = .7f;
            Health[i] = 100;
            EntityClass[i] = 3;
            Rigidbodies[i] = Droid.GetComponent<Rigidbody>();
            EnemyType[i] = 1;

        }
    }

    // Update is called once per frame  
    void Update()
    {
        //gunsway 



        //var v = Rigidbodies[0].velocity;
        timer -= 1;
        if (timer < 1)
        {
            //aaaaaaaaaaaaGameUpdate();
        }


        //character movement
        if (Input.GetKey(KeyCode.W))
        {
            Rigidbodies[0].velocity = items[0].transform.forward * speed;
            gunanimator.SetTrigger("Walk");
        }

        else if (Input.GetKey(KeyCode.A))
        {
            Rigidbodies[0].velocity = items[0].transform.right * -speed;
            gunanimator.SetTrigger("Walk");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Rigidbodies[0].velocity = items[0].transform.forward * -speed;
            gunanimator.SetTrigger("Walk");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rigidbodies[0].velocity = items[0].transform.right * speed;
            gunanimator.SetTrigger("Walk");
        }

        cam.transform.position = items[0].transform.position;
        items[0].transform.Rotate(items[0].transform.up, -Input.GetAxis("Mouse X") * rotationSpeed,
                                             Space.World);
        cam.transform.parent = items[0].transform;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotX = mouseY * mouseSensitivity;
        rotY = mouseX * mouseSensitivity;
        cam.transform.rotation *= Quaternion.Euler(-rotX, 0, 0.0f);




        if (Input.GetMouseButtonDown(0))
        {
            gunanimator.SetTrigger("Fire");
            cam.transform.rotation *= Quaternion.Euler(-1f, 0, 0.0f);
            GameObject casing = Instantiate(BulletCasing, chamberOpening.transform);
            GameObject smoke = Instantiate(gunsmoke, barrelmuzzle.transform);
            smoke.transform.SetParent(this.transform);
            Destroy(casing, 5f);
            GunSound[0].Play();
            GameObject lightGameObject = new GameObject("The Light");
            Light lightComp = lightGameObject.AddComponent<Light>();
            lightComp.color = Color.white;
            lightGameObject.transform.position = barrelmuzzle.transform.position;
            lightGameObject.GetComponent<Light>().range = 50;
            lightGameObject.GetComponent<Light>().intensity = 2;
            Destroy(lightGameObject, .2f);

            RaycastHit hit;
            int layerMask = (1 << 9);
            layerMask = ~layerMask;

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
            {
                
                Debug.Log("hit");
                GameObject g = Instantiate(bullethits);
                g.transform.position = hit.point;

                Rigidbody target = hit.transform.GetComponent<Rigidbody>();
                Debug.Log(target);
                for (int i = 1; i < EntityLocations.Length; i++)
                {
                    if (target == Rigidbodies[i])
                    {
                        if (EntityClass[i] == 3)
                        {
                            
                                Health[i] -= weapondamage[Weaponselectednumber] * Defense[i];
                                Debug.Log("lose health");
                                if (Health[i] < 0)
                                {
                                    Kill(i);
                                }
                            
                        }
                        
                    }
                }
            }


        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Weaponselectednumber == 0)
            {
                hitmarker.SetActive(false);
                scope.SetActive(true);
                gunmeshobject.SetActive(false);
                BigCam.fieldOfView = 5;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            scope.SetActive(false);
            hitmarker.SetActive(true);
            gunmeshobject.SetActive(true);
            BigCam.fieldOfView = 31.7f;
        }
        if (Input.GetMouseButtonDown(2))
        {
            if (Weaponselectednumber < inventory.Count - 1)
            {
                Weaponselectednumber += 1;

                gunmesh.mesh = weaponmeshes[inventory[Weaponselectednumber]];
                gunmeshobject.transform.localPosition = DefaultGunTransforms[inventory[Weaponselectednumber]];
                gunmeshobject.transform.localEulerAngles = DefaultGunRotations[inventory[Weaponselectednumber]];
                gunmeshobject.transform.localScale = new Vector3(gunscalex[inventory[Weaponselectednumber]], gunscaley[inventory[Weaponselectednumber]], gunscalez[inventory[Weaponselectednumber]]);
                barrelmuzzle.transform.localPosition = Barrels[inventory[Weaponselectednumber]];
                chamberOpening.transform.localPosition = Chambers[inventory[Weaponselectednumber]];


            }
            else if (Weaponselectednumber >= inventory.Count - 1)
            {
                Weaponselectednumber = 0;

                gunmesh.mesh = weaponmeshes[inventory[Weaponselectednumber]];
                gunmeshobject.transform.localPosition = DefaultGunTransforms[inventory[Weaponselectednumber]];
                gunmeshobject.transform.localEulerAngles = DefaultGunRotations[inventory[Weaponselectednumber]];
                gunmeshobject.transform.localScale = new Vector3(gunscalex[inventory[Weaponselectednumber]], gunscaley[inventory[Weaponselectednumber]], gunscalez[inventory[Weaponselectednumber]]);
                barrelmuzzle.transform.localPosition = Barrels[inventory[Weaponselectednumber]];
                chamberOpening.transform.localPosition = Chambers[inventory[Weaponselectednumber]];


            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //inventory.Add(1);
            for (int i = 1; i<totalnumberofitems; i++)
            {
                DistanceToPlayer[i] = Vector3.Distance(EntityLocations[i], EntityLocations[0]);
                if (EntityClass[i] == 2) //is a renewable or thing you can pick up 
                {
                    GameObject geeked = Instantiate(DeadResources[ResourceType[i]]);
                    geeked.transform.position = items[i].transform.position;
                    geeked.transform.rotation = items[i].transform.rotation;
                    items[i].transform.position = new Vector3(Random.Range(0, 1000), 0, Random.Range(0, 1000));
                    
                }


            }
            
        }



        EntityLocations[0] = items[0].transform.position;
        
  
        enemyupdatetimer -= 1;
        if (enemyupdatetimer < 1)
        {
            EnemyUpdate();
        }
        updatetimer -= 1;
        if (updatetimer < 2)
        {
            GameUpdate();
        }
    }

    private void GameUpdate()
    {
        var grish = numberoftrees + NumberofResources ;
        var grish2 = numberofdroids + numberofScreechers;
        var grish3 = grish2 + grish;
        for (int i = 1; i < grish3; i++)
        {
            DistanceToPlayer[i] = Vector3.Distance(EntityLocations[i], EntityLocations[0]);
            if (DistanceToPlayer[i] < 200)
            {
                items[i].SetActive(true);
            }
            if (DistanceToPlayer[i] > 200)
            {
                items[i].SetActive(false);
            }
        }
        updatetimer = 5;
    }
    private void EnemyUpdate()
    {
        var fuck = NumberofResources + numberoftrees;
        var goy = numberofdroids + fuck;
        var scritch = goy + numberofScreechers;

        for (int i = fuck; i < goy; i++) //enemy[0]
        {
            if (Health[i] > 0)
            {
                DistanceToPlayer[i] = Vector3.Distance(EntityLocations[i], EntityLocations[0]);

                if (DistanceToPlayer[i] < 25)
                {
                    preliminarytimers[i] -= .5f;
                    items[i].transform.LookAt(items[0].transform.position);
                    if (preliminarytimers[i] == 3)
                    {
                        GameObject muzzleflass = Instantiate(muzzleflash, items[i].transform);
                        GameObject smoke = Instantiate(gunsmoke, items[i].transform);
                        smoke.transform.SetParent(items[i].transform);
                        muzzleflass.transform.SetParent(items[i].transform);
                        muzzleflass.transform.localPosition = new Vector3(.5f, 0, 1);
                        smoke.transform.localPosition = new Vector3(.5f, 0, 1);
                        GameObject cases = Instantiate(BulletCasing);
                        cases.transform.position = new Vector3(.5f, 0, 1);

                        GunSound[1].Play();
                    }
                    if (preliminarytimers[i] == 1)
                    {
                        GameObject muzzleflass = Instantiate(muzzleflash, items[i].transform);
                        GameObject smoke = Instantiate(gunsmoke, items[i].transform);
                        smoke.transform.SetParent(items[i].transform);
                        muzzleflass.transform.SetParent(items[i].transform);
                        muzzleflass.transform.localPosition = new Vector3(-.5f, 0, 1);
                        smoke.transform.localPosition = new Vector3(-.5f, 0, 1);
                        GameObject cases = Instantiate(BulletCasing);
                        cases.transform.position = new Vector3(-.5f, 0, 1);

                        GunSound[1].Play();
                    }
                    if (preliminarytimers[i] < 1) 
                    {
                        preliminarytimers[i] = 5;
                    }
                }
            }
        }
        for (int i = goy; i < scritch; i++)//enemy[1]
        {
            DistanceToPlayer[i] = Vector3.Distance(EntityLocations[i], EntityLocations[0]);
            if (DistanceToPlayer[i] < 180)
            {
                items[i].transform.LookAt(items[0].transform.position);
            }
            if (DistanceToPlayer[i] < 60)
            {

                Chasing[i] = true;
            }
            if (Chasing[i] == true)
            {
                if (DistanceToPlayer[i] > 3)
                {
                    items[i].transform.position += items[i].transform.forward * 40f * Time.deltaTime;
                    EntityLocations[i] = items[i].transform.position;
                }
                if (DistanceToPlayer[i] < 4)
                {

                    preliminarytimers[i] -= 1;
                    if (preliminarytimers[i] < 2)
                    {
                        Health[0] -= 2;
                        preliminarytimers[i] = 5;
                    }
                }
            }
        }
        
            enemyupdatetimer = 3;
    }


    //same
    public int width = 1000;
    public int height = 1000;
    public int depth = 20;
    public float scale = 20;
    public Terrain terrainz;


    /// <summary>
    /// perlin generation shit for terrain data very slow
    /// </summary>
    public void Kill(int x)
    {
        if (EntityClass[x] == 3)
        {
            GameObject geeked = Instantiate(deaddroid[EnemyType[x]]);
            geeked.transform.position = items[x].transform.position;
            geeked.transform.rotation = items[x].transform.rotation;
            items[x].transform.position = new Vector3(Random.Range(0, 1000), 0, Random.Range(0, 1000));
            Health[x] = 100;
            Chasing[x] = false;
           // Destroy(items[x].gameObject);
           // items[x] = geeked;
        }
    }
    private void StartTerrainGeneration()
    {
        Terrain terrain = terrainz.GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }
    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }
    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);

            }
        }
        return heights;
    }
    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    ///something like this for enemy checks 
    /// if (EntityClass[i] == 9)
    ///            {
    ///                items[i].transform.LookAt(items[0].transform.position);
    ///           }
    ///
}
