using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Uk.Org.Adcock.Parallel;
using OpenCvSharp;

public class Circuit_change_shader : MonoBehaviour
{
    public List<Material> Mat;
    public Renderer rend;
    void Start()
    {
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.U)){
           foreach(Material mat in Mat){
               mat.color -= new Color(0, 0, 0, .1f);
           }
        }
        else if (Input.GetKey(KeyCode.I)){
            foreach(Material mat in Mat){
                mat.color += new Color(0, 0, 0, .1f);
           }
        }
    }
}
