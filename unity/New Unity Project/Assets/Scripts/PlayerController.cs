using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //The event for referencing in the player controller (updating the record for example)
    [SerializeField]
    private EventManagerOrderedPath m_event;

    //The particle system
    [SerializeField]
    private GameObject m_psattached;

    public GameObject temp1 = null;
    public GameObject temp2 = null;

    //Used for guranteeing only once "shoots the raycast" when the player "press the button" (consider the speed of the frame updated during the time of the process of pressing the button)
    private bool if_hit_once = false;
    private bool if_hit_once_start = false;
    private bool if_hit_once_end = false;

    private int game_status = 0;
    //A temperary gameobject for storing the star pointed at
    private GameObject star_forPoint;

    [Tooltip("The text above the map on the left hand")]
    public GameObject textAboveMinimap;

    // Update is called once per frame
    void Update()
    {
        if (game_status == 0)
        {
            m_event.game_status = 0;
        }
        else
        {
            m_event.game_status = 1;
        }
        if (OVRInput.Get(OVRInput.Button.Two) && !if_hit_once_end)
        {
            if (game_status == 1)
            {
                submit();
                game_status = 0;

            }
            if_hit_once_end = true;
            
        }
        else if (OVRInput.Get(OVRInput.Button.Two) && if_hit_once_end) //Used for guranteeing only "one raycast per press"
        {

        }
        else
        {
            if_hit_once_end = false;
        }
        if (OVRInput.Get(OVRInput.Button.One) && !if_hit_once_start)
        {
            if (game_status == 0)
            {
                m_event.startGame();
                textAboveMinimap.GetComponent<TextMesh>().text = "Press B to Submit";
                game_status = 1;
            }
               
            if_hit_once_start = true;
        }
        else if (OVRInput.Get(OVRInput.Button.One) && if_hit_once_start) //Used for guranteeing only "one raycast per press"
        {

        }
        else
        {
            if_hit_once_start = false;
        }
        //Drawing lines on map
        for (int i = 0; i < m_event.num_dots; i++)
        {
            if (game_status == 1)
            {
                if (m_event.stars[i].GetComponent<Dot>().connect_to != -1)
                {
                    LineRenderer lineR_map = m_event.map.transform.GetChild(i).gameObject.GetComponent<LineRenderer>();
                    lineR_map.startWidth = 0.01f;
                    lineR_map.SetPosition(0, m_event.map.transform.GetChild(i).gameObject.transform.position);
                    lineR_map.SetPosition(1, m_event.map.transform.GetChild((m_event.stars[i].GetComponent<Dot>().connect_to)).gameObject.transform.position);
                }
            }
            
           
        }
        transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
        //Debug.Log("Test");

        //Enlarge the star to indicate it's been pointed to 
        RaycastHit hitinfo2;
        if (Physics.Raycast(transform.position, transform.forward, out hitinfo2))
        {
            if (hitinfo2.collider != null && hitinfo2.transform.tag == "Star")
            {
                if (star_forPoint != null)
                {
                    star_forPoint.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
                    //star_forPoint.transform.GetChild(1).transform.GetChild(1).GetComponent<ParticleSystem>().startColor = Color.yellow;
                }
                GameObject h_temp = hitinfo2.transform.gameObject;
                star_forPoint = h_temp;
                // h_temp.transform.GetChild(1).transform.GetChild(1).GetComponent<ParticleSystem>().startColor = Color.red;
                star_forPoint.transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);

            }
        }
        else //Change the size of the stars back to the normal size
        {
            if (star_forPoint != null)
            {
                // star_forPoint.transform.GetChild(1).transform.GetChild(1).GetComponent<ParticleSystem>().startColor = Color.yellow;
                star_forPoint.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
                star_forPoint = null;
            }
        }

        //The loop for detecting all behaviors interacting with the player (pressing button to select the stars, drawing lines between cards, etc)
        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
		{
            Debug.Log("Button pressed");
			RaycastHit hitInfo = new RaycastHit();
			if (Physics.Raycast(transform.position, transform.forward, out hitInfo) && hitInfo.transform.tag == "Star" && !if_hit_once)
			{
                if_hit_once = true;
                if (hitInfo.transform.childCount == 0)
                {
                    GameObject temp = Instantiate(m_psattached);
                    GameObject hitten = hitInfo.transform.gameObject;
                    temp.transform.parent = hitten.transform;
                    temp.transform.localPosition = Vector3.zero;
                    hitten.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.yellow * 1.5f);
                    hitten.transform.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
                    hitten.transform.GetChild(0).localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                
                Debug.Log("It's working");
                if (temp1 == null)
                {
                    temp1 = hitInfo.transform.gameObject;
                    hitInfo.transform.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<ParticleSystem>().startColor = Color.white;
                }
                else
                {
                    temp2 = hitInfo.transform.gameObject;
                    hitInfo.transform.gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<ParticleSystem>().startColor = Color.white;
                }
                if (temp1 != null && temp2!= null)
                {
                    LineRenderer lineRenderer = temp1.GetComponent<LineRenderer>();
                    Debug.Log(temp1.GetComponent<Dot>().order);
                    Debug.Log(lineRenderer == null);
                    //LineRenderer lineRenderer = GetComponent < LineRenderer>();
                    //lineRenderer.material.color = Color.green;
                    lineRenderer.SetPosition(0, temp1.transform.position);
                    lineRenderer.SetPosition(1, temp2.transform.position);
                    lineRenderer.startWidth = 0.5f;
                    LineRenderer lineR_map = m_event.map.transform.GetChild(temp1.GetComponent<Dot>().order).gameObject.GetComponent<LineRenderer>();
                    lineR_map.startWidth = 5f;
                    lineR_map.SetPosition(0, m_event.map.transform.GetChild(temp1.GetComponent<Dot>().order).gameObject.transform.position);
                    lineR_map.SetPosition(1, m_event.map.transform.GetChild(temp2.GetComponent<Dot>().order).gameObject.transform.position);
                    Debug.Log("start: "+m_event.map.transform.GetChild(temp1.GetComponent<Dot>().order).gameObject.transform.position);
                    Debug.Log("end: "+m_event.map.transform.GetChild(temp2.GetComponent<Dot>().order).gameObject.transform.position);
                    temp1.GetComponent<Dot>().connect_to = temp2.GetComponent<Dot>().order;
                    lineRenderer.enabled = true;
                    lineR_map.enabled = true;
                    Debug.Log("Drawing lines");
                    temp1.transform.GetChild(0).transform.GetChild(0).GetComponent<ParticleSystem>().startColor = Color.yellow;
                    temp2.transform.GetChild(0).transform.GetChild(0).GetComponent<ParticleSystem>().startColor = Color.yellow;
                    temp1 = null;
                    temp2 = null;
                }

                
            }
            
		}
        else if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && if_hit_once) //Used for guranteeing only "one raycast per press"
        {

        }
        else
        {
            if_hit_once = false;
        }
    }

    #region Helper Function 
    //Used for submiting the matching
    private void submit()
    {
        Debug.Log("Test for submit");
        bool success = m_event.checkSuccess();
        if (success)
        {
            m_event.record[0] += 1;
            textAboveMinimap.GetComponent<TextMesh>().text = "Correct! Press A to start again";
        }
        else
        {
            m_event.record[1] += 1;
            textAboveMinimap.GetComponent<TextMesh>().text = "Incorrect, press A to try again";
        }
        foreach (GameObject star in m_event.stars)
        {
            Destroy(star);
        }
        foreach (GameObject star_formap in m_event.stars_formap)
        {
            Destroy(star_formap);
        }
    }
    #endregion
}
