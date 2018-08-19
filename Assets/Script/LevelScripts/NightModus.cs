﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightModus : MonoBehaviour {

	public bool nightModus;
	public bool isDay;
	private LevelGenerator levelGenerator;
	private PlayerSpawner playerSpawner;
	private CameraScroller cameraScroller;
	private GenerateDistanceLine generateDistanceLine;
	private int worldOffest;
	private int startRowPos;
	private bool update;
	private float duration;
	private float pastTime;
	private Light worldLight;
	private float worldLightOriginal;
	private float worldAmbientOriginal;

	void Awake()
	{
		isDay = true;
		update = true;
		levelGenerator = FindObjectOfType<LevelGenerator>();
		cameraScroller = FindObjectOfType<CameraScroller>();
		playerSpawner = FindObjectOfType<PlayerSpawner>();
		worldLight = FindObjectOfType<Light>();
		generateDistanceLine = FindObjectOfType<GenerateDistanceLine>();
	}

	void Start()
	{
		duration = Random.value * 50f;
		pastTime = 0f;
		worldOffest = levelGenerator.tiefeLevelStartBasis;
		worldLightOriginal = worldLight.intensity;
		worldAmbientOriginal = RenderSettings.ambientIntensity;
	}

	void Update()
	{
		if(!nightModus && isDay)
		{
			if(pastTime < duration)
			{
				pastTime += Time.deltaTime;
			} else {
				nightModus = true;
				pastTime = 0f;
				duration = Random.value * 50f;
			}
		} else if(!isDay) {

			if(pastTime < duration)
			{
				pastTime += Time.deltaTime;
			} else {
				nightModus = false;
				pastTime = 0f;
				duration = Random.value * 50f;
			}
		}

		if(nightModus && isDay)
		{
			if(update)
			{
				update = false;
				startRowPos = cameraScroller.rowPosition;
			}

			levelGenerator.generateGlowBalls = true;
			if(cameraScroller.rowPosition > startRowPos + worldOffest - 5)
			{
				isDay = false;
				update = true;
				switchToNight();
				playerLightOn();

				generateDistanceLine.generateGlowStangen = true;
				checkGlowStangen();
				//Debug.Log("Switch to Night");
			}
		}


		if(!nightModus && !isDay)
		{
			if(update)
			{
				update = false;
				startRowPos = cameraScroller.rowPosition;
			}

			levelGenerator.generateGlowBalls = false;
			if(cameraScroller.rowPosition > startRowPos + worldOffest - 5)
			{
				isDay = true;
				update = true;
				switchToDay();
				playerLightOff();

				generateDistanceLine.generateGlowStangen = false;
				checkGlowStangen();
				//Debug.Log("Switch to Day");
			}
		} 
	}

	public void checkGlowStangen()
	{
		if(isDay)
		{
			glowBallDimmOff();
		} else {
			glowBallDimmOn();
		}
	}

	private void playerLightOn()
	{
		for(int i = 0; i < playerSpawner.playerList.Count; i++)
		{
			GameObject player = playerSpawner.playerList[i].gameObject;
			StartCoroutine(playerGlowOn(player));
		}
	}

	private void glowBallDimmOn()
	{
		foreach(GameObject go in levelGenerator.DistanceLines)
		{
			if(go != null)
			{
				if(go.CompareTag("GlowMaterial"))
				{
					go.GetComponent<GlowScript>().glowDimmOn();
				}
			}
		}
	}

	private void glowBallDimmOff()
	{
		foreach(GameObject go in levelGenerator.DistanceLines)
		{
			if(go != null)
			{
				if(go.CompareTag("GlowMaterial"))
				{
					go.GetComponent<GlowScript>().glowDimmOff();
				}
			}
		}
	}

	private IEnumerator playerGlowOn(GameObject player)
	{
		float emission = 0f;
		Color baseColor = player.GetComponent<Renderer>().material.color;
		Material playerMaterial = player.GetComponent<Renderer>().material;
		Light playerLight = player.GetComponent<Light>();
		playerLight.intensity = 0f;
		playerLight.enabled = true;

		while(emission < 2.3f)
		{
			emission += Time.deltaTime * 0.2f;
			Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);
			playerMaterial.SetColor("_EmissionColor", finalColor);
			playerMaterial.EnableKeyword("_EMISSION");
			playerLight.intensity = emission;

			yield return new WaitForEndOfFrame();
		}
	}

	private void playerLightOff()
	{
		for(int i = 0; i < playerSpawner.playerList.Count; i++)
		{
			GameObject player = playerSpawner.playerList[i].gameObject;
			StartCoroutine(playerGlowOff(player));
		}
	}

	private IEnumerator playerGlowOff(GameObject player)
	{
		float emission = 2.3f;
		Color baseColor = player.GetComponent<Renderer>().material.color;
		Material playerMaterial = player.GetComponent<Renderer>().material;
		Light playerLight = player.GetComponent<Light>();
		playerLight.intensity = 0f;

		while(emission > 0f)
		{
			emission -= Time.deltaTime * 0.4f;
			Color finalColor = baseColor * Mathf.LinearToGammaSpace (emission);
			playerMaterial.SetColor("_EmissionColor", finalColor);
			playerLight.intensity = emission;

			yield return new WaitForEndOfFrame();
		}

		playerMaterial.DisableKeyword("_EMISSION");
		playerLight.enabled = false;
	}

	private void switchToNight()
	{
		StopAllCoroutines();
		StartCoroutine(dawnWorldAmbient(0.5f));
		StartCoroutine(dawnWorldLight(0f));
	}

	private void switchToDay()
	{
		StopAllCoroutines();
		StartCoroutine(riseWorldAmbient(2f));
		StartCoroutine(riseWorldLight(worldLightOriginal));
	}

	private IEnumerator dawnWorldLight(float value)
	{
		while(worldLight.intensity > value)
		{
			//worldLight.transform.Rotate(-Time.deltaTime * 4f, 0f, 0f);
			worldLight.intensity -= Time.deltaTime * 0.2f;
			yield return null;
		}
	}

	private IEnumerator riseWorldLight(float value)
	{
		while(worldLight.intensity < value)
		{
			//worldLight.transform.Rotate(Time.deltaTime * 4f, 0f, 0f);
			worldLight.intensity += Time.deltaTime * 0.2f;
			yield return null;
		}

		worldLight.intensity = value;
	}

	private IEnumerator dawnWorldAmbient(float value)
	{
		while(RenderSettings.ambientIntensity > value)
		{
			RenderSettings.ambientIntensity -= Time.deltaTime * 0.2f;
			yield return null;
		}
	}

	private IEnumerator riseWorldAmbient(float value)
	{
		while(RenderSettings.ambientIntensity < value)
		{
			RenderSettings.ambientIntensity += Time.deltaTime * 0.2f;
			yield return null;
		}

		RenderSettings.ambientIntensity = worldAmbientOriginal;
	}
}