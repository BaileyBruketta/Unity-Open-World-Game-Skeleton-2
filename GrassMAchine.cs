using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassMAchine : MonoBehaviour
{
    public int numberofgrass;
    public GameObject[] Grass;
        public Transform player;
    private Vector3[] grasslocations;
    private GameObject[] items;
    public float distance;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 4;
        grasslocations= new Vector3[numberofgrass];
        items = new GameObject[numberofgrass];
        for (int i =0; i<grasslocations.Length; i++)
        {
            Vector3 localtree = new Vector3((Random.Range(0, 2000)), 60, (Random.Range(0, 2000)));
            var x = Mathf.RoundToInt(Random.Range(0, Grass.Length));
            GameObject Trei = Instantiate(Grass[x]);
            Trei.transform.localEulerAngles = new Vector3(Random.Range(0, 0), Random.Range(0, 360), Random.Range(0, 0));
            Trei.transform.position = localtree;
            items[i] = Trei;
            grasslocations[i] = Trei.transform.position;

            RaycastHit hit;
            Physics.Raycast(localtree, -Trei.transform.up, out hit, 900);
            Trei.transform.position = hit.point;
            grasslocations[i] = Trei.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= .1f;
        if (timer < 2)
        {
            RenderUpdate();
        }
    }

    private void RenderUpdate()
    {
        for (int i = 0; i < grasslocations.Length; i++)
        {
            float distancez = Vector3.Distance(grasslocations[i], player.position);
            if (distancez < distance)
            {
                items[i].SetActive(true);

            }
            if (distancez > distance)
            {
                items[i].SetActive(false);
            }

        }
    }
}
