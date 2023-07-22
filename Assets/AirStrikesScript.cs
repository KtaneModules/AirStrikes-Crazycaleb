using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;
using KModkit;

public class AirStrikesScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;

    public KMSelectable[] TriangleArrows;
    public KMSelectable[] ChevronArrows;
    public KMSelectable CrosshairScreen;
    public GameObject[] MainComponents;
    public GameObject Crosshair;
    public GameObject CrosshairScreenBorder;
    public GameObject MessageScreenBorder;
    public GameObject ModuleBackground;
    public GameObject Radar;

    public TextMesh MessageScreenText;
    public Material[] ContentColors;
    public Material[] BorderColors;


    private static string[] locations = new string[] 
    { 
        "Simonsborough", "Cycle Hills", "Brush Oaks", "Mystic Square", 
        "Talkington", "Blind Valley", "Digit Springs", "Fort Tean", 
        "Question Park", "Flashing Heights", "Alarmburg", "English Crest", 
        "Sueet Falls", "Match Acres", "Black Knoll", "Passwood"
    };
    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;
    private bool wantRotation = true;
    private bool soundOnClick;
    private float elapsed = 0f;
    private int currentLocation;
    private int startingLocation;

    private void Awake()
    {
        //Assign Arrow buttons
        for (int i = 0; i < 4; i++)
        {
            int j = i;
            TriangleArrows[j].OnInteract += delegate () { return HandleMovement(TriangleArrows[j], j); };            
             ChevronArrows[j].OnInteract += delegate () { return HandleMovement( ChevronArrows[j], j); };
        }
        //Assign Screen button
        CrosshairScreen.OnInteract += delegate () { return ResetLocation(); };
    }
    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        StartCoroutine(RadarRotation());

        GenerateModule();
    }
    private IEnumerator RadarRotation()
    {
        while (wantRotation)
        {
            Radar.transform.localEulerAngles = new Vector3(90f, elapsed * 90, 90f);
            yield return null;
            elapsed += Time.deltaTime; 
        } 
    }

    private void GenerateModule()
    {
        startingLocation = Random.Range(0, 16);
        currentLocation = startingLocation;
        bool[] startingArray = GenerateBoolArray(startingLocation);
        soundOnClick = startingArray[10];
        DisplayModule(startingArray);

        Debug.LogFormat("[Air Strikes #{0}] Starting Location: {1}", _moduleId, locations[startingLocation]);

    }

    //Arrow OnInteractHandler
    private bool HandleMovement(KMSelectable arrow, int index)
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, arrow.transform);
        arrow.AddInteractionPunch(.25f);
        switch (index)
        {
            case 0: //Left
                if (currentLocation % 4 == 0) currentLocation += 3;
                else currentLocation--;
                break;
            case 1: //Up
                currentLocation = (currentLocation + 12) % 16;
                break;
            case 2: //Right
                if (currentLocation % 4 == 3) currentLocation -= 3;
                else currentLocation++;
                break;
            default: //Down
                currentLocation = (currentLocation + 4) % 16;
                break;
        }
        Debug.Log(currentLocation); //DEBUG CURRENT LOCATION
        return false;
    }

    //Screen onInteractHandler
    private bool ResetLocation()
    {
        currentLocation = startingLocation;
        Debug.Log(currentLocation); //DEBUG CURRENT LOCATION
        if (soundOnClick) { }; // Onclick custom sound

        return false;
    }

    //Boolean array for 
    private bool[] GenerateBoolArray(int initialPosition)
    {
        bool[] arr = new bool[16];
        string boolStr = "";
        for (int i = 0; i < 16; i++) 
        { 
            arr[i] = Random.Range(0, 2) != 0;
            //Guarantee specified is false
            if (i%4 == initialPosition % 4 || i/4 == initialPosition / 4 ) { arr[i] = false; }
            boolStr = boolStr + arr[i].ToString() + " ";
        }
        //Guarantee specified is true
        for (int i = initialPosition; i < 2 * initialPosition; i += 3) arr[i % 16] = true;

        Debug.Log("Conditions: " + boolStr + " ");
        return arr;
    }

    private void DisplayModule(bool[] arr)
    {
        //Arrow types are triangular and chevron. 
        bool arrowTypeIsTriangular = arr[4];
        //Content colors are Red, Yellow, Blue, Purple.
        int arrowColorIndex
            = arr[0] && arr[3] ? 2 : (arr[0] ? 0 : (arr[3] ? 1 : 3));
        //Content colors are Red, Yellow, Blue, Purple.
        int crosshairColorIndex
            = arr[9] && arr[12] ? 3 : (arr[9] ? 1 : (arr[12] ? 0 : 2));
        //Border colors are Black and White.
        int crosshairBorderColorIndex
            = arr[7] ? 0 : 1;
        //Shows 1-6, or 7-12 minute even or odd, PM or AM
        string timestamp
            = (arr[1] ? Random.Range(1, 7) : Random.Range(7, 13)).ToString() + ":" +
              (arr[2] ? 2 * Random.Range(0, 30) : 1 + 2 * Random.Range(0, 30)).ToString("00") + " " +
              (arr[6] ? "PM" : "AM");
        //Border colors are Black and White.
        int messageBorderColorIndex
            = arr[13] ? 0 : 1;
        //Status Light directional are TR, TL, BR, BL. TR,BR,BL,TL
        int statusLightPosition
            = arr[8] && arr[15] ? 0 : (arr[8] ? 3 : (arr[15] ? 1 : 2));
        //Plays sound when center screen is clicked.
        bool flippedScreens
            = arr[14];

        //TODO: Usernames

        if (arrowTypeIsTriangular)
        {
            foreach (KMSelectable arrow in ChevronArrows) arrow.gameObject.SetActive(false);
            foreach (KMSelectable arrow in TriangleArrows) arrow.gameObject.GetComponent<MeshRenderer>().material = ContentColors[arrowColorIndex];
        }
        else
        {
            foreach (KMSelectable arrow in TriangleArrows) arrow.gameObject.SetActive(false);
            foreach (KMSelectable arrow in ChevronArrows) arrow.gameObject.GetComponent<MeshRenderer>().material = ContentColors[arrowColorIndex];
        }
        Crosshair.GetComponent<MeshRenderer>().material = ContentColors[crosshairColorIndex];

        var crosshairRenderer = CrosshairScreenBorder.GetComponent<MeshRenderer>();
        var crosshairMaterials = crosshairRenderer.materials;
        crosshairMaterials[1] = BorderColors[crosshairBorderColorIndex];
        crosshairRenderer.materials = crosshairMaterials;

        var messageRenderer = MessageScreenBorder.GetComponent<MeshRenderer>();
        var messageMaterials = messageRenderer.materials;
        messageMaterials[1] = BorderColors[messageBorderColorIndex];
        messageRenderer.materials = messageMaterials;
        MessageScreenText.text = "<Username>" + ": " + timestamp + "\n" + "<Message>";

        ModuleBackground.transform.rotation = Quaternion.Euler(new Vector3(0, statusLightPosition * 90, 0));

        if (flippedScreens)
        {
            MainComponents[1].transform.localPosition = new Vector3(0f, 0f, 0.12f);
            MainComponents[0].transform.localPosition = new Vector3(0f, 0f, -0.04f);
        }
    }

}
