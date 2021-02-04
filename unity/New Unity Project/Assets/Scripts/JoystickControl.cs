using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickControl : MonoBehaviour
{
    // Transform of the top of the joystick.
    public Transform topOfJoystick;

    // Angle of rotation of the joystick around x axis.
    private float sideToSideTilt = 0;

    // List of Exercise image texture.
    public Texture2D[] exerciseImages = new Texture2D[3];
    // Reference to the arcade machine screen GO.
    private GameObject screen;
    // Index of the current Exercise image texture displayed on the screen.
    private int exerciseImage = 0;

    // Joystick returned to neutral state after a selection.
    // Used to prevent super fast image switching on the screen.
    private bool returnedToOriginal = true;
    // Determine if joystick is within rotation angle to 
    // prevent it from colliding with arcade machine GO.
    private bool withinLimits = true;

    /*
     * Set screen to first exercise image.
     */
    void Start()
    {
        screen = GameObject.Find("ArcadeMachineScreen");
        screen.GetComponent<Renderer>().material.mainTexture = exerciseImages[exerciseImage];
    }

    /*
     * 
     */
    void Update()
    {
        sideToSideTilt = transform.rotation.x;
        if (sideToSideTilt > -0.55 && sideToSideTilt < -0.54)
        {
            if (returnedToOriginal) { 
                if (exerciseImage - 1 < 0)
                {
                    exerciseImage = exerciseImages.Length - 1;
                } else
                {
                    exerciseImage -= 1;
                }
            screen.GetComponent<Renderer>().material.mainTexture = exerciseImages[exerciseImage];
            }
            returnedToOriginal = false;
        } else if (sideToSideTilt > -0.83 && sideToSideTilt < -0.82)
        {
            if (returnedToOriginal)
            {
                if (exerciseImage + 1 >= exerciseImages.Length && returnedToOriginal)
                {
                    exerciseImage = 0;
                }
                else
                {
                    exerciseImage += 1;
                }
                screen.GetComponent<Renderer>().material.mainTexture = exerciseImages[exerciseImage];
            }
            returnedToOriginal = false;
        } else if (sideToSideTilt < -0.63 && sideToSideTilt > -0.77)
        {
            returnedToOriginal = true;
        }

        if (!withinLimits)
        {
            if (sideToSideTilt > -0.55)
            {
                transform.rotation = Quaternion.Euler(-90f, 0, 0);
            }
            else if (sideToSideTilt < -0.83)
            {
                transform.rotation = Quaternion.Euler(-90f, 0, 0); 
            }
        }
    }

    /*
     * If joystick is within rotation range, let it follow player's hand on collision.
     */
    private void OnTriggerStay(Collider other)
    {
        if (sideToSideTilt > -0.55 || sideToSideTilt < -0.83)
        {
            withinLimits = false;
        } else
        {
            withinLimits = true;
        }
       
        if (withinLimits)
        {
            if (other.CompareTag("PlayerHand"))
            {
                transform.LookAt(other.transform.position, transform.up);
            }
        } 
    }
}

