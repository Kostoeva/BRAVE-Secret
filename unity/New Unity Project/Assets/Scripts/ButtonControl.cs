using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour
{
    private bool on = false;
    private bool buttonPressed = false;
    public GameObject button;

    private float buttonOriginalY;
    private float buttonReturnSpeed = 0.001f;

    private float buttonHitAgainTime = 0.5f;
    private float canHitAgain;

    private GameObject screen;

    // Start is called before the first frame update
    void Start()
    {
        buttonOriginalY = button.transform.position.y;

        screen = GameObject.Find("ArcadeMachineScreen");
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonPressed)
        {

            buttonPressed = false;
            on = !on;

            button.transform.position = new Vector3(button.transform.position.x, button.transform.position.y - 0.05f, button.transform.position.z);

            if (on)
            {
                Debug.Log(screen.GetComponent<Renderer>().material.mainTexture.name.ToString());
                SceneManager.LoadScene(screen.GetComponent<Renderer>().material.mainTexture.name.ToString());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand") && canHitAgain < Time.time)
        {
            canHitAgain = Time.time + buttonHitAgainTime;
            buttonPressed = true;
        }
    }
}
