using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_Peter : MonoBehaviour
{

    [SerializeField]
    private EventManager m_eventManager;

    //The selected cards in game3
    private GameObject[] game3;

    //To see if the player press the button, 
    //update to true only as player unpress the button. 
    //This variable exists for only hit once each press.
    private bool if_hit_once = false;

    //Temp Object to restore the object that's being hit to change color of it.
    private GameObject temp_hit;

    //Origin color
    private Color orig_color;

 
    /* Update is called once per frame
     * 1. Detect if the player pressed button
     * 2. Shoot a raycast when the player pressed the button
     * 3. Call corresponding function with the hit object (submit, start, cards)
     * 4. Update message in the game
     */
    void Update()
    {
        RaycastHit hitInfo;

        //TODO: Left Controller
        float x_position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTrackedRemote).x * 10;
        Debug.Log(x_position);
        float y_position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTrackedRemote).y * 20;
        transform.position = new Vector3(x_position, y_position + 2f, transform.position.z);
        transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);

        //Virtual raycast to indicate which object rn is being pointed colliding status
        RaycastHit hitInfo2;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo2) && !if_hit_once)
        {
            if (hitInfo2.collider != null)
            {
                GameObject h_point = hitInfo2.transform.gameObject;
                enter(h_point);
            }
         }
        else
        {
            if (temp_hit != null)
            {
                leave();
            }
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !if_hit_once)
        {  
            Debug.Log("Trigger Test");
            if (Physics.Raycast(transform.position, transform.forward, out hitInfo))
            {
                //if the player shoot, then make if_hit_once to true so that player won't shoot 
                //multiple times during the frames when the player hold the button
                if_hit_once = true;
                Debug.Log("hit");
                if (hitInfo.collider != null)
                {
                    GameObject hitten = hitInfo.transform.gameObject;
                    if (hitten.tag == "Card")
                    {
                        selecting_Cards(hitten);
                    }
                    else
                    {
                        trigger_Game(hitten);
                    }
                }
            }
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && if_hit_once)
        {

        }
        else
        {
            //Updating if_hit_once so that player can shoot next time
            if_hit_once = false;
        }
    }

    #region Helper Function

    private void trigger_Game(GameObject hit_Object)
    {
        if (hit_Object.tag == "Start1")
        {
            m_eventManager.startGame(1);
        }
        else if (hit_Object.tag == "Start2")
        {
            m_eventManager.startGame(2);
        }
        else if (hit_Object.tag == "Start3")
        {
            m_eventManager.startGame(3);
        }
        //Submitting the game
        else if (hit_Object.tag == "Submit")
        {
            hit_Object.GetComponent<submitButton>().m_EventManager.submit();
            if (m_eventManager.Game == 3)
            {
                destroy(game3);
                game3 = null;
            }
        }
    }

    private void selecting_Cards(GameObject hit_Object)
    {
        if (m_eventManager.Game == 3)
        {
            CardInfomation ci = hit_Object.GetComponent<CardInfomation>();
            bool s = ci.selected;
            Vector3 p = hit_Object.transform.position;
            if (!s)
            {
                if (m_eventManager.selected.Count == m_eventManager.num_Orig)
                {
                    m_eventManager.msg.text = "You can only select" + m_eventManager.num_Orig + "cards.";
                }
                else
                {
                    m_eventManager.selected.AddLast(ci.id);
                    hit_Object.GetComponent<CardInfomation>().selected = true;
                }
            }
            else
            {
                m_eventManager.selected.Remove(ci.id);
                hit_Object.GetComponent<CardInfomation>().selected = false;
            }
            //Show the selected cards in the order
            renderSelected(m_eventManager.num_Orig);
            //Debug.Log("It's working!");
        }
        else if (m_eventManager.Game == 2)
        {
            CardInfomation ci = hit_Object.GetComponent<CardInfomation>();
            bool s = ci.selected;
            Vector3 p = hit_Object.transform.position;
            if (!s)
            {

                m_eventManager.selected.AddLast(ci.id);
                Renderer rend = hit_Object.GetComponent<MeshRenderer>();
                rend.material.color = Color.green;
                hit_Object.GetComponent<CardInfomation>().selected = true;
            }
            else
            {
                Renderer rend = hit_Object.GetComponent<Renderer>();
                rend.material.color = Color.white;
                m_eventManager.selected.Remove(ci.id);
                hit_Object.GetComponent<CardInfomation>().selected = false;
            }
        }
        else
        {
            CardInfomation ci = hit_Object.GetComponent<CardInfomation>();
            bool s = ci.selected;
            Vector3 p = hit_Object.transform.position;
            if (!s)
            {
                if (m_eventManager.selected.Count == m_eventManager.num_Orig)
                {
                    m_eventManager.msg.text = "You can only select" + m_eventManager.num_Orig + "cards.";
                }
                else
                {
                    Renderer rend = hit_Object.GetComponent<MeshRenderer>();
                    rend.material.color = Color.green;
                    m_eventManager.selected.AddLast(ci.id);
                    hit_Object.GetComponent<CardInfomation>().selected = true;
                }
            }
            else
            {
                m_eventManager.selected.Remove(ci.id);
                Renderer rend = hit_Object.GetComponent<Renderer>();
                rend.material.color = Color.white;
                m_eventManager.selected.Remove(ci.id);
                hit_Object.GetComponent<CardInfomation>().selected = false;
            }
            Debug.Log("It's working!");
        }
    } 

    /* Used in game3 to show selected cards in order
     * 1. destroy previous spawned selected cards
     * 2. setscale for spawning selected cards, same as setScale function in Eventmanager
     * 3. spawned the selected cards in the order the player selects them
     * Input: int num_Orig, used for setting scale
     * Output: void
     */
    private void renderSelected(int num_Orig)
    {
        destroy(game3);
        float card_wid = 1.27f;
        float card_hei = 1.8f;
        float wid_space = 0.5f;
        float hei_space = 0.5f;
        float wid_index = 1;
        float hei_index = 1;
        float curWid = m_eventManager.m_Width * card_wid + (m_eventManager.m_Width - 1) * wid_space;
        wid_index = 10f / curWid;
        if (wid_index < 0.5f)
        {
            wid_index = 0.5f;
        }
        card_wid *= wid_index;
        wid_space *= wid_index;
        float curHei = m_eventManager.m_Height * card_hei + (m_eventManager.m_Height - 1) * hei_space;
        hei_index = 1.6f / curHei;
        if (hei_index < 0.5f)
        {
            hei_index = 0.5f;
        }
        card_hei *= hei_index;
        hei_space *= hei_index;
        GameObject[] result = new GameObject[m_eventManager.selected.Count];
        float offset = 0;
        if (num_Orig % 2 == 0)
        {
            offset = (card_wid + wid_space) * 0.5f;
        }
        int index = 0;
        foreach(int card in m_eventManager.selected)
        {
            GameObject a = Instantiate(m_eventManager.m_CardsSet[card], new Vector3(offset + wid_space * (index - num_Orig / 2) + card_wid * (index - num_Orig / 2), 5.3f), Quaternion.identity);
            a.transform.Rotate(0, 180, 0);
            a.transform.localScale += new Vector3(wid_index - 1, hei_index - 1, 0);
            result[index] = a;
            index++;
        }
        game3 = result;
    }

    /* Destroy the gameobjects passed by a, used in renderSelected (specificly for game3) 
     * Input: GameObject[], the array of gameobject to destroy
     * Output: void
     */
    private void destroy(GameObject[] a)
    {
        if (a != null)
        {
            for (int i = 0; i < a.Length; i++)
            {
                Destroy(a[i]);
            }
        }
    }

    private void enter(GameObject curr)
    {
        if (temp_hit != null)
        {
            leave();
        }
        if (curr.tag == "Card")
        {
            Renderer rend = curr.GetComponent<MeshRenderer>();
            orig_color = rend.material.color;
            rend.material.color = Color.yellow;
            temp_hit = curr;
        }
        else if(curr.tag == "Submit")
        {
            orig_color = curr.GetComponent<submitButton>().child.GetComponent<Image>().color;
            curr.GetComponent<submitButton>().child.GetComponent<Image>().color = Color.yellow;
            temp_hit = curr;
        }
        else
        {
            orig_color = curr.GetComponent<startCube>().child.GetComponent<Image>().color;
            curr.GetComponent<startCube>().child.GetComponent<Image>().color = Color.yellow;
            temp_hit = curr;
        }

    }

    private void leave()
    {
        if (temp_hit.tag == "Card")
        {
            Renderer rend = temp_hit.GetComponent<MeshRenderer>();
            if (temp_hit.GetComponent<CardInfomation>().selected)
            {
                rend.material.color = Color.green;
            }
            else
            {
                rend.material.color = Color.white;
            }
            temp_hit = null;
            orig_color = Color.white;
        }
        else if (temp_hit.tag == "Submit")
        {
            temp_hit.GetComponent<submitButton>().child.GetComponent<Image>().color = orig_color;
            temp_hit = null;
            orig_color = Color.white;
        }
        else
        {
            temp_hit.GetComponent<startCube>().child.GetComponent<Image>().color = orig_color;
            temp_hit = null;
            orig_color = Color.white;
        }

    }
    #endregion
}

