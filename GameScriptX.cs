using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class GameScriptX : MonoBehaviour
{
    public GameObject HitMarkerHit;

    public Terrain[] terrainlist;
    //to implement:
    //Some of these public items should really be private, and allocating by findgameobject at start of runtime 
    //enemy and animal movement algorithm that forces animals apart & away from player SLIGHTLY when player is moving 
    //items and bodies  "thud" when hit ground
    //bodyweight 
    private List<GameObject> DynamicItems;
    private List<Vector3> DynamicItemsLocations;
    public GameObject[] PossibleDynamicItems;
    private List<float> DynamicItemsType;
    //0=fire
    //1=smg
    //2=meat
    private float[] DeadBodyMaterials;
    
    private float temperature;
    
    public AudioSource PickUpItemSound;

    public GameObject[] CraftableItems;
    public string[] CraftableItemNames;
    //0=fire

    //MenuScreen Components
    public GameObject MenuScreen;
    public GameObject[] MenuComponents;
    //0=command menu 
    //1=inventory
    public InputField MenuInput;    
    public bool MenuUp;

    //inventory
    public GameObject Inventory;
    private int[] InventoryAmounts; 
    public Text[] InventoryComponents;
    public string[] InventoryNames;
    private bool[] replanted;
    public float[] CarryWeightPerItem;
    //0 = sticks
    //1 = berries
    //2 = meat
    public Text[] HudComponents;
    //1=carry weight
    //2=health
    //3=hunger
    //4=heat
    //5=fatigue
    //6=ammo



    private float MaxCarryWeight;
    private float carryingweight;
    private float heat;
    private float maxheat;
    private float maxhealth;
    private float fatigue;
    private float maxfatigue;
    private float hunger;
    private float maxhunger;


    //for grabbing objects
    public GameObject ItemGrabObject;
    Animator ItemGrabber;
    
        //resource floats
    private float[] BranchesOnTrees; //number of branches on a given tree
    private int[] TreeBranchMax; //maximum number of branches to be yielded from scavenging a tree

    private GameObject[] waterlist;
    private Vector3[] waterlocationlist;


 //dealing with guns   + a few out of place things
    public Animator gunanimator;
    public GameObject[] guns;
    private bool[] Chasing;  //is item chasing ? for enemyupdate function
    public GameObject hitmarker; 
    public GameObject scope;
    public Camera BigCam; //main camera. 

    //for doing math at start of game / populating world 
    public int numberoftrees;
    public int numberofdroids;
    public int numberofScreechers;
    public int NumberofResources;
    public int totalnumberofitems;
    public GameObject[] deaddroid; //list of dead enemy instances to pull bodies from when enemies die. 
    public PhysicMaterial physics;

    //weapons. each weapon has a location relative to the ui, a location for muzzle smoke and flashes, and a location for casings to come from. their [#]s should match.
    public List<int> equipment;
    public float[] gunscalex, gunscaley, gunscalez; //how big is the mesh of the gun you switch to going to be 
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
    //0=rifle
    //1-smg
    public GameObject gunsmoke;
    public GameObject muzzleflash;
    public GameObject bullethits;
    public float[] weapondamage;
    public float enemyupdatetimer;
    public float[] gunweights;
    private float firingpin;
    //ammo
    private float[] ammo;
    //0=rifle
    //1=smg
    private List<float> magazinerounds;
    private float[] weaponmaxammo;
    private List<float> MagMax;
    private List <float> ammotype;
    private List<float> equipmentweight;
    //0=rifleammo
    //1=smg

    //list of item prefabs to pull from when populating, etc.
    public Camera cam;
    public GameObject baseobjectprefab;
    public GameObject[] trees;
    public GameObject[] enemies;
    public GameObject[] Resources;
    public GameObject[] DeadResources;
    public int[] ResourceType;
    //0=berries

    //the following values constitute a list of items and their locations and bodies.     
    private Vector3[] EntityLocations;
    private Transform[] EntityLocales;
    private GameObject[] items;
    private float[] Health;
    private float[] DistanceToPlayer;
    private Rigidbody[] Rigidbodies;
    private float[] preliminarytimers;
    private List<float> DIstanceToPlayer2;
    

    //the following values hold which types of items exist
    private int[] EntityClass;
    //0=player;
    //1=trees;
    //2=resources;
    //3=enemies;
    private float[] Defense;
    private int[] EnemyType;
    
    //these all deal with character movements
    private float speed;
    public float initialspeed;
    public float maxvelocity;
    public float mouseSensitivity = 25.0F;
    public float clampAngle = 80.0F;
    float rotX = 0.001F, rotY = 0.0F;
    public float rotationSpeed = 3;
    public float characterheight;
    public float charactergirth;

    //important floats - make threshold private before final build for #speed
    public float distancethreshold;
    private float timer;
    private float updatetimer;


    // Start is called before the first frame update
    void Start()
    {
        //for automatic and bolt action firing
        firingpin = 0;

        equipment = new List<int>();
        MagMax = new List<float>();
        equipmentweight = new List<float>();
        DynamicItemsType = new List<float>();
        DynamicItems = new List<GameObject>();
        DynamicItemsLocations = new List<Vector3>(); 
        DeadBodyMaterials = new float[DynamicItems.Count];
        DIstanceToPlayer2 = new List<float>();

        ammo = new float[2];
        magazinerounds = new List<float>();
        weaponmaxammo = new float[2];
        weaponmaxammo[0] = 12;
        weaponmaxammo[1] = 15;
        ammotype = new List<float>();
        
        
        ammotype.Add(0);
        

        temperature = 45;
        carryingweight = 0;
        MaxCarryWeight = 50;
        speed=initialspeed;

        var se = new InputField.SubmitEvent();
        se.AddListener(ConsoleCommands);
        MenuInput.onEndEdit = se;
        MenuUp = false;

    ItemGrabber = ItemGrabObject.GetComponent<Animator>();
        //StartTerrainGeneration();
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
        BranchesOnTrees = new float[totalnumberofitems];
        TreeBranchMax = new int[totalnumberofitems];
        replanted = new bool[totalnumberofitems];
        InventoryAmounts = new int[3]; //number of inventory slots

        
    
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

        //start with a gun 
        equipment.Add(0);
        magazinerounds.Add(0);
        MagMax.Add(weaponmaxammo[0]);
        equipmentweight.Add(gunweights[0]);
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

        gunmesh.mesh = weaponmeshes[equipment[Weaponselectednumber]];
        gunmeshobject.GetComponent<MeshRenderer>().enabled = false;
        gunmeshobject.transform.localPosition = DefaultGunTransforms[0];
        barrelmuzzle.transform.parent = gunmesh.transform;
        barrelmuzzle.transform.localPosition = Barrels[equipment[Weaponselectednumber]];
        chamberOpening.transform.parent = gunmesh.transform;
        chamberOpening.transform.localPosition = Chambers[equipment[Weaponselectednumber]];
        Rigidbodies[0] = items[0].GetComponent<Rigidbody>();
        items[0].transform.position = new Vector3(500, 50, 500);


        TreeSpawn();
        ResourceSpawn();
        
        enemyspawn();
        InitiateHud();
        GameEntry();
        UpdateAmmo();
        updatetimer = 25;
        items[0].GetComponent<MeshRenderer>().enabled = false;

    }
    //implement gradual carry weight increase (eat + sleep ->muscle mass increase) AND decrease 
    //implement weapon type areas where only certain types of weapons WORK
    private void ConsoleCommands(string arg0) 
    {
        Debug.Log("checking console commands");
        if (arg0=="/inventory")
        {
            for (int i = 0; i < InventoryAmounts.Length; i++)
            {
                InventoryComponents[i].text = InventoryNames[i] + " : " + InventoryAmounts[i] + " , " + "Carry Weight : " + CarryWeightPerItem[i] + "kg" + " (" + (CarryWeightPerItem[i] * InventoryAmounts[i]) + "kg" + ")";
            }
            
            MenuComponents[0].SetActive(false); //deactivate command list
            MenuComponents[1].SetActive(true); //activate inventory list 
            
        }
        if (arg0 =="/throw")
        {

        }
        if(arg0=="/list craftable")
        {

        }
        for (int cf = 0; cf < CraftableItemNames.Length; cf++) //crafting
        {
            Debug.Log("checking craftables");
            if (arg0 == "/craft " + (CraftableItemNames[cf]))
            {
                Debug.Log("we have a match");
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 15))
                {
                    Debug.Log("raycasted");
                    if (CraftableItemNames[cf] == "fire") //makes fire
                    {
                        Debug.Log("its a fire");
                        if (InventoryAmounts[0] > 3) //more than three sticks?
                        {
                            Debug.Log("spacious");
                            GameObject craftable = Instantiate(CraftableItems[cf]);
                            craftable.transform.position = new Vector3(hit.point.x,hit.point.y+1,hit.point.z);
                            InventoryAmounts[0] -= 3;
                            DynamicItems.Add(craftable);
                            DynamicItemsLocations.Add(craftable.transform.position);
                            DynamicItemsType.Add(0);
                            DIstanceToPlayer2.Add(0);
                        }
                    }
                }

            }
            CalculateCarryWeight();
            for (int i = 0; i < InventoryAmounts.Length; i++)
            {
                InventoryComponents[i].text = InventoryNames[i] + " : " + InventoryAmounts[i] + " , " + "Carry Weight : " + CarryWeightPerItem[i] + "kg" + " (" + (CarryWeightPerItem[i] * InventoryAmounts[i]) + "kg" + ")";
            }
        }
        if (arg0 == "/equipment")
        {

        }
        if (arg0 == "/equip")
        {

        }
        if (arg0 == "/savegame")
        {

        }
        if (arg0 == "/loadgame")
        {

        }
        if (arg0 == "/skills")
        {

        }
        if (arg0 == "/quests")
        {

        }
        if (arg0 == "/commands")
        {
            MenuComponents[0].SetActive(true); //activate command list
            MenuComponents[1].SetActive(false); //deactivate inventory list 
        }
        if(arg0=="/consume berry")
        {
            if (InventoryAmounts[1] > 0)
            {
                hunger += 30;
                Health[0] += 3;
                if (Health[0] > maxhealth)
                {
                    Health[0] = maxhealth;
                }
                InventoryAmounts[1] -= 1;
            }
        }


    }
    private void InitiateHud()
    {
        CalculateCarryWeight();
        HudComponents[0].text = "Carry Weight : " + carryingweight +"/"+MaxCarryWeight+ "kg"; //edit carry weight
        HudComponents[5].text = "Ammo :" + magazinerounds + "/" + ammo[Mathf.RoundToInt(ammotype[Weaponselectednumber])]; //ammo hud
    }
    private void UpdateHud()
    {
        if (hunger > .1f)
        {
            hunger -= .025f;
        }
        if (temperature < 32)
        {
            heat -= .025f;
        }
        if (fatigue > .1f)
        {
            fatigue -= .00003f;
        }
        

        var healthquotient = .001f * (hunger / maxhunger) * (heat / maxheat) * (fatigue / maxfatigue);

        if (Health[0] < maxhealth)
        {
            Health[0] += healthquotient;
        }

        if (hunger < 1)
        {
            Health[0] -= .1f;
        }
        if (fatigue < 1)
        {
            Health[0] -= .1f;
        }
        if (heat < 96)
        {
            Health[0] -= .01f * (maxheat - heat);
        }


        //0=carry weight
        //1=health
        //2=hunger
        //3=heat
        //4=fatigue
        //5=ammo
        HudComponents[1].text = "Health: "+ Mathf.RoundToInt(Health[0]) + " / " + maxhealth;
        HudComponents[2].text = "Hunger: "+Mathf.RoundToInt(hunger) + " / " + maxhunger +"cal";
        HudComponents[3].text = "Heat: "+Mathf.RoundToInt(heat) + " / " + maxheat + "F";
        HudComponents[4].text = "Fatigue: "+Mathf.RoundToInt(fatigue) + " / " + maxfatigue + "hrs";
    }

    private void GameEntry()
    {
        Health[0] = 100;
        maxhealth = 100;
        hunger = 2700;
        maxhunger = 2700;
        heat = 98;
        maxheat = 98;
        fatigue = 6;
        maxfatigue = 6;
        Weaponselectednumber = 0;
        gunmeshobject.transform.localScale = new Vector3(gunscalex[equipment[Weaponselectednumber]], gunscaley[equipment[Weaponselectednumber]], gunscalez[equipment[Weaponselectednumber]]);
        guns[0].SetActive(true);
        for(int i =0; i < guns.Length; i++)
        {
            if (i != 0)
            {
                guns[i].SetActive(false);
            }
        }
        ammo[0] = 13;
        MagMax.Add(weaponmaxammo[0]);
        magazinerounds[0] = 12;
    }
    private void TreeSpawn()
    {
        for (int i = 1; i < numberoftrees; i++)
        {
            Vector3 localtree = new Vector3((Random.Range(0, 2000)), 60, (Random.Range(0, 2000)));
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
            TreeBranchMax[i] = Random.Range(0, 6);
            BranchesOnTrees[i] = TreeBranchMax[i];
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
            Vector3 localtreez = new Vector3((Random.Range(0, 2000)), 60, (Random.Range(0, 2000)));
            var xz = Mathf.RoundToInt(Random.Range(0, Resources.Length));
            GameObject Trei = Instantiate(Resources[xz]);
            Trei.transform.localEulerAngles = new Vector3(Random.Range(0, 6), Random.Range(0, 6), Random.Range(0, 6));
            Trei.transform.position = localtreez;
            items[i] = Trei;
            EntityLocations[i] = Trei.transform.position;
            items[i].isStatic = true;
            EntityClass[i] = 2;
            ResourceType[i] = xz;
            TreeBranchMax[i] = Random.Range(1, 6);
            BranchesOnTrees[i] = TreeBranchMax[i];
            replanted[i] = true;
            RaycastHit hit;
            Physics.Raycast(localtreez, -Trei.transform.up, out hit, 800);
            Trei.transform.position = hit.point;
            EntityLocations[i] = Trei.transform.position;

            if (BranchesOnTrees[i] < 1)
            {
                replanted[i] = false;
                GameObject geeked = Instantiate(DeadResources[ResourceType[i]]); //makes a copy of the plant where there are no berries
                geeked.transform.position = items[i].transform.position; //moves it to the plants initial location
                geeked.transform.rotation = items[i].transform.rotation; //and location
                Destroy(items[i]); //destroy berries image
                items[i] = geeked;
            }
        } 
    }
    private void enemyspawn()
    {
        var treeanre = NumberofResources + numberoftrees;
        var goy = treeanre+ numberofdroids;
        var scritch = goy + numberofScreechers;
        for (int i = treeanre; i < goy; i++) //droids
        {
            Vector3 NewDroid = new Vector3((Random.Range(0, 2000)), 60, Random.Range(0, 2000));
            GameObject Droid = Instantiate(enemies[0]);
            Droid.transform.position = NewDroid;
            items[i] = Droid;
            EntityLocations[i] = NewDroid;
            preliminarytimers[i] = 5;
            Defense[i] = .7f;
            Health[i] = 100;
            EntityClass[i] = 3;
            Rigidbodies[i] = Droid.GetComponent<Rigidbody>();
            EnemyType[i] = 0;

            RaycastHit hit;
            Physics.Raycast(NewDroid, -Droid.transform.up, out hit, 800);
            Droid.transform.position = new Vector3(hit.point.x, hit.point.y + 10, hit.point.z);
            EntityLocations[i] = Droid.transform.position;
           

        }
        for (int i = goy; i < scritch; i++) //screechers
        {
            Vector3 NewDroid = new Vector3((Random.Range(0, 2000)), 60, Random.Range(0, 2000));
            GameObject Droid = Instantiate(enemies[1]);
            Droid.transform.position = NewDroid;
            items[i] = Droid;
            EntityLocations[i] = NewDroid;
            preliminarytimers[i] = 5;
            Defense[i] = .7f;
            Health[i] = 150;
            EntityClass[i] = 3;
            Rigidbodies[i] = Droid.GetComponent<Rigidbody>();
            EnemyType[i] = 1;
            

           RaycastHit hit;
            Physics.Raycast(NewDroid, -Droid.transform.up, out hit, 800);
            Droid.transform.position = new Vector3(hit.point.x, hit.point.y + 10, hit.point.z);
            EntityLocations[i] = Droid.transform.position;

        }
    }
    

    // Update is called once per frame  
    void Update()
    {
        //character movement
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            MenuInput.ActivateInputField();
            speed = 0;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            speed = initialspeed;
            MenuInput.DeactivateInputField();
        }
        if (Input.GetKey(KeyCode.W))
        {
            Rigidbodies[0].velocity = items[0].transform.forward * speed;
            Rigidbodies[0].velocity -= items[0].transform.up * speed/2;
            gunanimator.SetTrigger("Walk");
        }

        else if (Input.GetKey(KeyCode.A))
        {
            Rigidbodies[0].velocity = items[0].transform.right * -speed;
            Rigidbodies[0].velocity -= items[0].transform.up * speed/2;
            gunanimator.SetTrigger("Walk");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Rigidbodies[0].velocity = items[0].transform.forward * -speed;
            Rigidbodies[0].velocity -= items[0].transform.up * speed/2;
            gunanimator.SetTrigger("Walk");
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rigidbodies[0].velocity = items[0].transform.right * speed;
            Rigidbodies[0].velocity -= items[0].transform.up * speed/2;
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
            if (equipment[Weaponselectednumber] == 0)
            {
                if (magazinerounds[Weaponselectednumber] > 0)
                {
                    //gunanimator.StopPlayback();
                    gunanimator.Play("gunfire", -1, 0f);
                    // gunanimator.SetTrigger("Fire");
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
                                    Chasing[i] = true;
                                    GameObject Garbage = Instantiate(HitMarkerHit); Destroy(Garbage, 1F);

                                    Debug.Log("lose health");
                                    if (Health[i] < 0)
                                    {
                                        Kill(i);
                                    }

                                }

                            }
                        }
                    }
                    magazinerounds[Weaponselectednumber] -= 1;
                    UpdateAmmo();
                }
            }


        }
        if (Input.GetMouseButton(0))
        {
            if (equipment[Weaponselectednumber] == 1)
            {
                firingpin -= 1;
                if (firingpin < 2)
                {


                    if (magazinerounds[Weaponselectednumber] > 0)
                    {
                        //gunanimator.StopPlayback();
                        gunanimator.Play("gunfire", -1, 0f);
                        // gunanimator.SetTrigger("Fire");
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
                                        Chasing[i] = true;
                                        GameObject Garbage = Instantiate(HitMarkerHit); Destroy(Garbage, 1F);
                                        Debug.Log("lose health");
                                        if (Health[i] < 0)
                                        {
                                            Kill(i);
                                        }

                                    }

                                }
                            }
                        }
                        magazinerounds[Weaponselectednumber] -= 1;
                        UpdateAmmo();
                        firingpin = 4;
                    }



                    
                }
            }
        }








        if (Input.GetKeyDown(KeyCode.R))
        {
            if (magazinerounds[Weaponselectednumber] < weaponmaxammo[Weaponselectednumber])
            {
                Reload();
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
            if (Weaponselectednumber < equipment.Count - 1)
            {
                Weaponselectednumber += 1;

                //gunmesh.mesh = weaponmeshes[equipment[Weaponselectednumber]];
                gunmeshobject.transform.localPosition = DefaultGunTransforms[equipment[Weaponselectednumber]];
                gunmeshobject.transform.localEulerAngles = DefaultGunRotations[equipment[Weaponselectednumber]];
               // gunmeshobject.transform.localScale = new Vector3(gunscalex[equipment[Weaponselectednumber]], gunscaley[equipment[Weaponselectednumber]], gunscalez[equipment[Weaponselectednumber]]);
                for (int i=0; i < guns.Length; i++)
                {
                    if (i == equipment[Weaponselectednumber])
                    {
                        guns[i].SetActive(true);
                    }
                    if (i != equipment[Weaponselectednumber])
                    {
                        guns[i].SetActive(false);
                   }
                }
                barrelmuzzle.transform.localPosition = Barrels[equipment[Weaponselectednumber]];
                chamberOpening.transform.localPosition = Chambers[equipment[Weaponselectednumber]];


            }
            else if (Weaponselectednumber >= equipment.Count - 1)
            {
                Weaponselectednumber = 0;

                 //gunmesh.mesh = weaponmeshes[equipment[Weaponselectednumber]];
                 gunmeshobject.transform.localPosition = DefaultGunTransforms[equipment[Weaponselectednumber]];
                gunmeshobject.transform.localEulerAngles = DefaultGunRotations[equipment[Weaponselectednumber]];
              //  gunmeshobject.transform.localScale = new Vector3(gunscalex[equipment[Weaponselectednumber]], gunscaley[equipment[Weaponselectednumber]], gunscalez[equipment[Weaponselectednumber]]);
                for (int i = 0; i < guns.Length; i++)
                {
                    if (i == equipment[Weaponselectednumber])
                    {
                        guns[i].SetActive(true);
                    }
                    if (i != equipment[Weaponselectednumber])
                    {
                        guns[i].SetActive(false);
                    }
                }
                barrelmuzzle.transform.localPosition = Barrels[equipment[Weaponselectednumber]];
                chamberOpening.transform.localPosition = Chambers[equipment[Weaponselectednumber]];


            }
            UpdateAmmo();
        }

        if (Input.GetKeyDown(KeyCode.E)) //fucking grabbing things
        {
            //inventory.Add(1);
            for (int i = 1; i<totalnumberofitems; i++)
            {
                DistanceToPlayer[i] = Vector3.Distance(EntityLocations[i], EntityLocations[0]);
                if (DistanceToPlayer[i] < 5)
                {
                    if (carryingweight < MaxCarryWeight)
                    {
                        if (EntityClass[i] == 2) //is a renewable or thing you can pick up 
                        {
                            if (ResourceType[i] == 0) //berries
                            {
                                if (BranchesOnTrees[i] >= 1)
                                {
                                    ItemGrabber.Play("GrabBerry"); //plays the grab branch animation from the object 
                                    InventoryAmounts[1] += 1; //adds a berry
                                    BranchesOnTrees[i] -= 1;
                                    PickUpItemSound.Play();
                                }


                                if (BranchesOnTrees[i] < 1)
                                {
                                    replanted[i] = false;
                                    GameObject geeked = Instantiate(DeadResources[ResourceType[i]]); //makes a copy of the plant where there are no berries
                                    geeked.transform.position = items[i].transform.position; //moves it to the plants initial location
                                    geeked.transform.rotation = items[i].transform.rotation; //and location
                                    Destroy(items[i]); //destroy berries image
                                    items[i] = geeked;
                                }
                            }

                        }
                        if (EntityClass[i] == 1)
                        {

                            if (BranchesOnTrees[i] >= 1)
                            {
                                ItemGrabber.Play("GrabStick"); //plays the grab branch animation from the object 
                                InventoryAmounts[0] += 1;
                                BranchesOnTrees[i] -= 1;
                                PickUpItemSound.Play();
                            }

                        }
                    }
                    CalculateCarryWeight();
                    HudComponents[0].text = "Carry Weight : " + carryingweight+ "/" + MaxCarryWeight + "kg"; //edit carry weight
                }


            }
            for(int i=0;i<DynamicItems.Count; i++)
            {
                DynamicItemsLocations[i] = DynamicItems[i].transform.position;
                DIstanceToPlayer2[i]= Vector3.Distance(DynamicItemsLocations[i], EntityLocations[0]);

                if (carryingweight < MaxCarryWeight)
                {
                    if (DIstanceToPlayer2[i] < 5)
                    {
                        if (DynamicItemsType[i] == 1)
                        {

                            equipment.Add(1); //1 for smg
                            magazinerounds.Add(15);
                            MagMax.Add(weaponmaxammo[1]);
                            ammotype.Add(1);
                            ammo[1] += 8;
                            Destroy(DynamicItems[i].gameObject);
                            Removal(i,i,i,i);
                            
                        }
                        if (DynamicItemsType[i] == 2)
                        {
                            ItemGrabber.Play("GrabMeat");
                            InventoryAmounts[2] += 1;
                            Destroy(DynamicItems[i].gameObject);
                            Removal(i, i, i, i);
                        }
                    }
                }

            }
            
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (MenuUp == false)
            {
                MenuScreen.SetActive(true);
            }
            else if (MenuUp == true)
            {
                MenuScreen.SetActive(false);
            }
            CallMenu();
        }



        EntityLocations[0] = items[0].transform.position;
        
  
        enemyupdatetimer -= 1;
        if (enemyupdatetimer < 1)
        {
            EnemyUpdate();
        }
        updatetimer -= .2f;
        if (updatetimer < 2)
        {
            GameUpdate();
            UpdateHud();
            UpdateTerrain();
            
        }
    }
    private void Removal(int sting, int sting2, int sting3, int sting4)
    {
        DIstanceToPlayer2.Remove(DIstanceToPlayer2[sting]);
        DynamicItems.Remove(DynamicItems[sting2]);
        DynamicItemsLocations.Remove(DynamicItemsLocations[sting3]);
        DynamicItemsType.Remove(DynamicItemsType[sting4]);

    }
    private void UpdateAmmo()
    { 
            HudComponents[5].text = "Ammo :" + magazinerounds[Mathf.RoundToInt(ammotype[Weaponselectednumber])] + "/" + (ammo[Mathf.RoundToInt(ammotype[Weaponselectednumber])]); //ammo hud
    }
    private void Reload()
    {
        var mag = magazinerounds[Weaponselectednumber];
        var max = MagMax[Weaponselectednumber];
        var ammu = ammo[Weaponselectednumber];

        
            if (ammo[Mathf.RoundToInt(ammotype[Weaponselectednumber])] != 0)
            {
                gunanimator.Play("Reload", -1, 0f);

                if (max < ammu)
                {
                
                    var pullfromreserve = max - mag;
                    magazinerounds[Weaponselectednumber] = max;
                ammo[Mathf.RoundToInt(ammotype[Weaponselectednumber])] -= pullfromreserve;
                }

                else if (max > ammu)
                {
                    var pullfromreserve = max-mag;

                     if (pullfromreserve < ammu)
                         {
                    
                    magazinerounds[Weaponselectednumber] += pullfromreserve;
                    ammo[Mathf.RoundToInt(ammotype[Weaponselectednumber])] -= pullfromreserve;                   
                         }

                      else if (pullfromreserve >= ammu)
                        {
                    
                    magazinerounds[Weaponselectednumber] += ammu;
                           ammo[Mathf.RoundToInt(ammotype[Weaponselectednumber])] = 0;
                         }

                }
            }
        



        HudComponents[5].text = "Ammo :" + magazinerounds[Weaponselectednumber] + "/" + (ammo[Mathf.RoundToInt(ammotype[Weaponselectednumber])]);
    }
    private void CalculateCarryWeight()
    {
        float gx = 0;
        for (int i = 0; i<equipment.Count; i++)
        {
            gx += gunweights[equipment[i]];
        }

        carryingweight=(InventoryAmounts[0] * CarryWeightPerItem[0])+(InventoryAmounts[1]+CarryWeightPerItem[1]) +(CarryWeightPerItem[2]*InventoryAmounts[2]) + gx;
        HudComponents[0].text = "Carry Weight : " + carryingweight + "/" + MaxCarryWeight + "kg";
    }
    private void CallMenu()
    {
        if (MenuUp == false)
        {
            MenuUp = true;
        }
        else if (MenuUp == true)
        {
            MenuUp = false;
        }
    }
    private void ResourceUpdate()
    {
        for (int i= 1; i < numberoftrees; i++)
        {
            if (BranchesOnTrees[i] < TreeBranchMax[i])
            {
                BranchesOnTrees[i] += .001f;
            }
        }
        for (int i = numberoftrees; i < NumberofResources; i++)
        {

            if (BranchesOnTrees[i] < TreeBranchMax[i])
            {
                BranchesOnTrees[i] += .001f; //grow more fruit 
            }
            if (BranchesOnTrees[i] > 1) //checks to see if new fruit 
            {
                if (replanted[i] == false)
                {
                    GameObject geeked = Instantiate(Resources[ResourceType[i]]); //makes a copy of the plant where there are no berries
                    geeked.transform.position = items[i].transform.position; //moves it to the plants initial location
                    geeked.transform.rotation = items[i].transform.rotation; //and location
                    Destroy(items[i]); //destroy berries imag
                    items[i] = geeked;
                    replanted[i] = true;
                }
            }
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
            if (DistanceToPlayer[i] < 600)
            {
                items[i].SetActive(true);
            }
            if (DistanceToPlayer[i] > 600)
            {
                items[i].SetActive(false);
            }
        }
        updatetimer = 5;
    }
    private void UpdateTerrain()
    {
        for (int i = 1; i < terrainlist.Length; i++)
        {
            DistanceToPlayer[i] = Vector3.Distance(terrainlist[i].transform.position, EntityLocations[0]);
            if (DistanceToPlayer[i] < 100)
            {
                items[i].SetActive(true);
            }
            if (DistanceToPlayer[i] > 100)
            {
                items[i].SetActive(false);
            }
        }
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

                if (DistanceToPlayer[i] < 40)
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
                        Health[0] -= .3f;
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
                        Health[0] -= .3f;
                        GunSound[1].Play();
                    }
                    if (preliminarytimers[i] < 1) 
                    {
                        preliminarytimers[i] = 5;
                    }
                }
                if (Chasing[i] == true)
                {
                    items[i].transform.LookAt(items[0].transform.position);
                    if (DistanceToPlayer[i] > 7)
                    {
                        items[i].transform.position += items[i].transform.forward * 40f * Time.deltaTime;
                        EntityLocations[i] = items[i].transform.position;
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
                if (DistanceToPlayer[i] > 7)
                {
                    items[i].transform.position += items[i].transform.forward * 40f * Time.deltaTime;
                    EntityLocations[i] = items[i].transform.position;
                }
                if (DistanceToPlayer[i] < 10)
                {

                    preliminarytimers[i] -= 1f;
                    if (preliminarytimers[i] < 2)
                    {
                        
                        Health[0] -= 1f;
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
    /// perlin generation shit for terrain data very slow / update: very effective& playing with scale allows for hills. credit to Brackey's tutorials on perlin noise and procedural terrain generation
    /// </summary>
    public void Kill(int x)
    {
        if (EntityClass[x] == 3)
        {
            GameObject geeked = Instantiate(deaddroid[EnemyType[x]]);
            geeked.transform.position = items[x].transform.position;
            geeked.transform.rotation = items[x].transform.rotation;
            items[x].transform.position = new Vector3(Random.Range(0, 1000), 60, Random.Range(0, 1000));
            EntityLocations[x] = items[x].transform.position;
            Health[x] = 100;
            Chasing[x] = false;

            RaycastHit hit;
            Physics.Raycast(EntityLocations[x], -items[x].transform.up, out hit, 800);
            items[x].transform.position = new Vector3(hit.point.x, hit.point.y + 10, hit.point.z);
            EntityLocations[x] = items[x].transform.position;

            if (EnemyType[x] == 0)
            {
                GameObject loot = Instantiate(PossibleDynamicItems[1]);
                loot.transform.position = geeked.transform.position;
                DynamicItems.Add(loot);
                DynamicItemsLocations.Add(loot.transform.position);
                DynamicItemsType.Add(1);
                DIstanceToPlayer2.Add(1);
            }
            if (EnemyType[x] ==1)
            {
                GameObject loot1 = Instantiate(PossibleDynamicItems[2]);
                GameObject loot2 = Instantiate(PossibleDynamicItems[2]);
                GameObject loot3 = Instantiate(PossibleDynamicItems[2]);
                loot1.transform.position = new Vector3(geeked.transform.position.x,geeked.transform.position.y+3,geeked.transform.position.z);
                loot2.transform.position = new Vector3(geeked.transform.position.x, geeked.transform.position.y +3, geeked.transform.position.z);
                loot3.transform.position = new Vector3(geeked.transform.position.x, geeked.transform.position.y + 3, geeked.transform.position.z);
                DynamicItems.Add(loot1);
                DynamicItems.Add(loot2);
                DynamicItems.Add(loot3);
                DynamicItemsLocations.Add(loot1.transform.position);
                DynamicItemsLocations.Add(loot2.transform.position);
                DynamicItemsLocations.Add(loot3.transform.position);
                DynamicItemsType.Add(2);
                DynamicItemsType.Add(2);
                DynamicItemsType.Add(2);
                DIstanceToPlayer2.Add(2);
                DIstanceToPlayer2.Add(2);
                DIstanceToPlayer2.Add(2);
            }
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
