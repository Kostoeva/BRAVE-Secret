using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject the_one;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(the_one);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
