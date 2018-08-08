﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour {
    
    private Vector3[] players;
    private CameraMovement cm;

    private float fieldWidth;
    

    private void Start()
    {
        cm = GameObject.Find("HorizontalAxis").GetComponent<CameraMovement>();
        fieldWidth = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>().AllGameObjects.GetLength(0) - 4;
    }

    void LateUpdate()
    {
        players = cm.positions;
        CameraMoving();
    }

    void CameraMoving()
    {
        float zoom = Mathf.Lerp(3f, 20f, GetGreatestDistance() / 50f);
        //LookAt, SmoothFollow SmoothDirection
        //3fach verschachtelte Kamera, getrennt voneinander 

        // BUG Irgendwie zuckt hier was...
        float dist = Mathf.Lerp(transform.localPosition.y, zoom, 0.5f * Time.deltaTime);
        Vector3 offset = new Vector3(0, dist, 0);
        //offset = new Vector3(0, dist, cameraScroller.transform.position.z);
        transform.localPosition = offset;
    }

    float GetGreatestDistance()
    {
        int numPlayers = cm.numPlayers;
        float maxDist = 0f;

        if(numPlayers == 1)
        {
            return maxDist;
        } else
        {
            for (int i = 0; i < players.Length; i++)
            {
                for (int j = i + 1; j < players.Length; j++)
                {
                    if (players[i].y == 0.45f && players[j].y == 0.45f)
                    {
                        float dist = Vector3.Distance(players[i], players[j]);
                        if (dist > maxDist)
                        {
                            maxDist = dist;
                        }
                    }
                }
            }
            return maxDist;
        }   
    }
}
