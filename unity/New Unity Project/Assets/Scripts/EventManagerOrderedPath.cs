using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManagerOrderedPath : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("The width of the grid")]
    private int grid_width;

    [SerializeField]
    [Tooltip("The height of the grid")]
    private int grid_height;

    [SerializeField]
    [Tooltip("The number of the dots")]
    public int num_dots;

    [SerializeField]
    private Boolean success;

    [Tooltip("The map on the left hand")]
    public GameObject map;
    #endregion

    #region Private Varibales
    //The radius of the dot
    private float dot_diameter;

    //The space between dots on x-axis
    private float x_space;

    //The space between dots on y-axis
    private float y_space;

    //The scale of the dot
    private float dots_scale;

    //Stored all the spawned stars
    public GameObject[] stars;

    public GameObject[] stars_formap;
    #endregion

    #region Public Variables
    // 0 stores times of success, 1 stores times of failures
    public int[] record = new int[2];

    public int game_status = 0;
    #endregion

    void Start()
	{
        //startGame();
	}

    private void Update()
    {
        success = checkSuccess();
    }

    #region Start Game
    public void startGame()
    {
        this.stars = spawnDots(generateDots());
    }
    #endregion

    #region Generate Dots
    private int[] generateDots()
    {
        int[] dots = new int[num_dots];
        for (int i = 0; i < num_dots; i++)
        {
            int index = UnityEngine.Random.Range(0, grid_width * grid_height);
            while (true)
            {
                int pos = Array.IndexOf(dots, index);
                if (pos == -1)
                {
                    if (checkDots(dots, i, index))
                    {
                        break;
                    }         
                }
                index = UnityEngine.Random.Range(0, grid_width * grid_height);
            }
            dots[i] = index;
        }
        return dots;
    }
    #endregion

    #region Spawn Dots
    private GameObject[] spawnDots(int[] dots)
    {
        setGridScale();
        GameObject[] result = new GameObject[num_dots];
        this.stars_formap = new GameObject[num_dots];
        for (int i = 0; i < num_dots; i++)
        {
            GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            temp.transform.position = new Vector3(x_space * (dots[i] % grid_width - grid_width / 2), ((int)(dots[i] / grid_width) - grid_height / 2) * y_space + 150f, 100f);
            //temp.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
            temp.AddComponent<Renderer>();
            temp.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.yellow * 0.8f);
            temp.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            temp.AddComponent<Dot>();
            temp.GetComponent<Dot>().order = i;
            temp.AddComponent<LineRenderer>();
            temp.GetComponent<LineRenderer>().material.SetColor("_EmissionColor", Color.white * 0.5f);
            temp.GetComponent<LineRenderer>().material.EnableKeyword("_EMISSION");
            temp.AddComponent<BoxCollider>();
            temp.tag = "Star";
            temp.transform.localScale += new Vector3(1f, 1f, 0);
            result[i] = temp;
            /*
            GameObject formap = new GameObject();
            formap.transform.position = new Vector3(x_space * (dots[i] % grid_width - grid_width / 2), ((int)(dots[i] / grid_width) - grid_height / 2) * y_space + 70f, 100f);
            formap.AddComponent<TextMesh>();
            formap.GetComponent<TextMesh>().text = i + " ";
            formap.transform.localScale += new Vector3(10f, 10f, 0);
            formap.GetComponent<TextMesh>().fontSize = 12;
            formap.AddComponent<LineRenderer>();
            formap.GetComponent<LineRenderer>().material.SetColor("_EmissionColor", Color.white * 0.5f);
            formap.GetComponent<LineRenderer>().material.EnableKeyword("_EMISSION");
            formap.layer = 9;

            formap.transform.parent = temp.transform;*/

            //For map
            GameObject formap2 = new GameObject();
            formap2.transform.parent = this.map.transform;
            formap2.transform.localPosition = new Vector3((x_space / 125f) * (dots[i] % grid_width - grid_width / 2), ((int)(dots[i] / grid_width) - grid_height / 2) * (y_space / 125f) + 0.1f, 0.1f);
            formap2.transform.localRotation = Quaternion.Euler(0, 0, 0);

            formap2.AddComponent<TextMesh>();
            formap2.GetComponent<TextMesh>().text = i + " ";
            //formap2.transform.localScale += new Vector3(10f, 10f, 0);
            formap2.GetComponent<TextMesh>().fontSize = 12;
            formap2.GetComponent<TextMesh>().characterSize = 0.1f;
            formap2.GetComponent<TextMesh>().color = Color.white;
            formap2.AddComponent<LineRenderer>();
            formap2.GetComponent<LineRenderer>().material.SetColor("_EmissionColor", Color.white * 0.5f);
            formap2.GetComponent<LineRenderer>().material.EnableKeyword("_EMISSION");
            stars_formap[i] = formap2;
        }
        /*
        for (int j = 0; j < grid_height; j++)
        {
            for (int k = 0; k < grid_width; k++)
            {
                GameObject a = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                a.transform.position = new Vector3(x_space * (k - grid_width / 2), (j - grid_height / 2) * y_space + 70f, 100f);
                a.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
                a.transform.localScale += new Vector3(1.5f, 1.5f, 0);
            }
        }*/
        return result;
        //For test
        
        

    }
    #endregion

    #region Helper Function
    private void setGridScale(float grid_width_size = 200f)
    {
        dot_diameter = 1f;
        x_space = 0.5f;
        y_space = 0.5f;
        float curr_width_size = grid_width * dot_diameter + (grid_width - 1) * x_space;
        float curr_height_size = grid_height * dot_diameter + (grid_height - 1) * y_space;
        dots_scale = grid_width_size / curr_width_size;
        dot_diameter = dots_scale * dot_diameter;
        x_space = x_space * dots_scale;
        y_space = y_space * dots_scale * grid_width / grid_height;
        Debug.Log(dot_diameter);

    }

    private Boolean checkDots(int[] dots, int curr_len, int cur)
    {
        Boolean result = true;
        for (int i = 0; i < curr_len; i++)
        {
            result = result && checkCol(dots[i], cur) && checkRow(dots[i], cur);
        }
        return result;
    }

    private Boolean checkCol(int check_on, int cur)
    {
        return Math.Abs(check_on - cur) % grid_width != 0;
    }

    private Boolean checkRow(int check_on, int cur)
    {
        return (int)(check_on / grid_width) != (int)(cur / grid_width);
    }
    #endregion

    #region Submit
    public Boolean checkSuccess()
    {
        if (stars.Length != 0 && game_status == 1)
        {
            for (int i = 0; i < num_dots; i++)
            {
                Dot stats = stars[i].GetComponent<Dot>();
                if (stats.order != num_dots - 1 && stats.order != stats.connect_to - 1)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
        
        
    }
    #endregion
}

