﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        Destroy(gameObject, 0.4f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale += new Vector3(4f * Time.deltaTime, 4f * Time.deltaTime, 4f * Time.deltaTime);
	}
}