using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Text.RegularExpressions;
public class Guge : MonoBehaviour
{
    private Regex reg = new Regex("[一-龥]*你[一-龥]*好[一-龥]*");
	// Use this for initialization
	void Start () {
        string contetn = "你好";
        Debug.Log(contetn[0]);
       
	}
	
	// Update is called once per frame
	void Update () {
        
    }
}
