using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;

public class AirStrikesScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;
    public KMSelectable[] Arrowage;
    public Material[] Druggage;
    public GameObject Radarage;
    public Material[] Borderage;
    public GameObject[] Emptyage;
    


    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;
    private bool wantRotation = true;
    private float elapsed = 0f;
    private bool flippedScreens;


    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        StartCoroutine(RadarRotation());

        flippedScreens = Rnd.Range(0, 2) == 0;
        if (flippedScreens) { 
        Emptyage[1].transform.localPosition = new Vector3(0f, 0f, 0.12f);
        Emptyage[0].transform.localPosition = new Vector3(0f, 0f, -0.04f);
        }
    }
    private IEnumerator RadarRotation()
    {
        while (wantRotation)
        {
            Radarage.transform.localEulerAngles = new Vector3(90f, elapsed * 90, 90f);
            yield return null;
            elapsed += Time.deltaTime;
        }
    }
}
