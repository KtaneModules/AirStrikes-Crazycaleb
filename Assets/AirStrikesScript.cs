using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;
using KModkit;
using UnityEngine.UI;

public class AirStrikesScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;
    public KMColorblindMode ColorblindMode;

    public KMSelectable[] TriangleArrows;
    public KMSelectable[] ChevronArrows;
    public KMSelectable CrosshairScreen;
    public KMSelectable MessageScreen;

    public GameObject[] MainComponents;
    public GameObject Crosshair;
    public GameObject CrosshairScreenBorder;
    public GameObject MessageScreenBorder;
    public GameObject ModuleBackground;
    public GameObject Radar;

    public Text MessageScreenText;
    public TextMesh[] ColorblindTexts;
    public Material[] ContentColors;
    public Material[] BorderColors;


    private static string[] locations = new string[] 
    { 
        "Simonsborough", "Cycle Hills", "Brush Oaks", "Mystic Square", 
        "Talkington", "Blind Valley", "Digit Springs", "Fort Tean", 
        "Question Park", "Flashing Heights", "Alarmburg", "English Crest", 
        "Sueet Falls", "Match Acres", "Black Knoll", "Passwood"
    };
    private static string[] names = new string[]
    {
        "1254", "Axodeau", "BlvdBroken", "Crazycaleb", "Heres_Fangy", "Ghastly",
        "Konoko", "Nshep", "Procyon", "Quinn Wuest", "Willeh", "a_galvantula", 
        "ghostsalt12", "meh", "redpenguiin", "vitzlo", "Usernam3"
    };
    private Dictionary<int, string[]> forTopics = new Dictionary<int, string[]>()
    {
        {  4, new string[] { "Dogs are superior to cats, simply because they are not cats.", 
                             "Cats are spawns of hell.", 
                             "Dogs love you unconditionally. Cats would kill you and take your house." } },
        { 11, new string[] { "You have UNO on your Xbox!", 
                             "Look, I can see RIGHT THERE in your Xbox library that you have UNO.", 
                             "Why play DOS on your XCUBE when you can play UNO on your XBOX?" } },
        { 12, new string[] { "It's big, it's round, so Pluto's a planet.", 
                             "Pluto go spin around big yellow star, this means Pluto be planet." } },
        { 10, new string[] { "Milk first leads to an uncomfortable mix of dry and wet cereal. Let the wetness equalize.",
                             "You absolute cretins. Who adds milk before cereal?",
                             "Cereal tastes better than milk, therefore cereal goes in before the milk." } },
        {  6, new string[] { "C# says that Sunday is the first day of the week, so that's what it is.",
                             "The first part of the day is the sun. The first part of the week is the Sunday.",
                             "First day of the week is absolutely Sunday. Did you think they made it up for calendar sales or something?",
                             "Last time I checked my calendar, Sunday was in the first column. Let that sink in." } },
        {  3, new string[] { "Of course you ignore \"The\" with alphabetical ordering. Who doesn't?" } },
        { 13, new string[] { "One. Three. Five. Nine. All other odd numbers are variants of these, and they all have E.",
                             "One has an E. Three has an E. That's all the odd numbers! They all have E!",
                             "The letter E is the most common letter in the language, included in all odd numbers.",
                             "How did you pass First Grade without knowing how to spell numbers properly? OnE, thrEE, fivE, sEvEn, ninE, the end." } },
        {  1, new string[] { "I just like pineapple, okay? Is that a bad thing to you? Really?", 
                             "My friend is Italian and they approved pineapple on pizza. Argument closed.", 
                             "Pineapple pizza is actually pretty good!" } },
        {  7, new string[] { "If it's socially acceptable to pick up a bowl of soup and drink from it, then I claim that soup is a drink.", 
                             "What do you do with the last bit of soup in the bowl? That’s right, you DRINK it.",
                             "If it's a liquid, it's a drink. Soup is definitely a liquid and therefore a drink." } },
        {  2, new string[] { "Guys, Santa is real. I just saw him at the mall recently!",
                             "If Santa's not real, then who's giving me all the gifts? A burglar?",
                             "If Santa Claus didn't exist, then why is there a Wikipedia article about him?",
                             "Of course Santa is real! Who else is that flying in the sky with those weird horses? Jesus? George Washington? My dad?",
                             "If you seriously think that Santa is real then yes you are absolutely correct.",
                             "As a Santa believer, he's real. He ate the cookies I gave him last year!" } },
        { 14, new string[] { "Sandy socks will be a long day.",
                             "Socks and sandals has to be the most uncomfortable thing ever. Just go barefoot."  } },
        {  0, new string[] { "Air Strikes is the only manual I read from Round 2 of KWSNE. Therefore it is the best one and must win.", 
                             "A message to those who thinks Air Strikes shouldn't win round 2 of KWSNE: You guys are just wrong."  } },
        { 15, new string[] { "Pepsi? PEPSI?? You lunatic. You buffoon. How do you prefer Pepsi over Coca-Cola??",
                             "Pepsi takes like nothing. Why would I take that over Coke?" } },
        {  5, new string[] { "Well, if you think about it, hot dogs are kind of sandwiches. Just... kind of." } },
        {  9, new string[] { "Stupid. How did we get the egg before the chicken? We got the chicken first.",  
                             "Well, of course the chicken came before the egg. The egg can't raise itself..." } },
        {  8, new string[] { "Winter is better than summer? Okay. Have fun freezing to death.", 
                             "There's school in winter. Bad. There's no school in summer. Good.", 
                             "Longer break in summer gives me more time for projects. Approved." } },
    };
    private Dictionary<int, string[]> againstTopics = new Dictionary<int, string[]>()
    {
        {  5, new string[] { "Dogs are inferior to cats, simply because they are dogs.",
                             "Cats are clearly better than dogs, just like how catgirls are better than doggirls.",
                             "Cats are better than dogs. I prefer my toddlers unbitten." } },
        { 15, new string[] { "Who the hell has an Xbox that comes with Uno? Mine didn't.",
                             "I own Xbox and I can confirm Uno is not on Xbox." } },
        {  4, new string[] { "Pluto has no right to be considered a planet. It's too tiny.",
                             "People. Pluto is a DWARF PLANET. Not a real one.",
                             "I trust the real science and it says that Pluto isn't real (planet), therefore it shouldn't be real (planet).",
                             "If Pluto's a planet, what's the limit? It's taking a bullet for science.",
                             "Pluto is TOO SMALL to be a planet, just like your BRAIN is TOO SMALL to be in your HEAD." } },
        {  7, new string[] { "Soggy cereal is the best cereal, and it's soggier with milk before cereal." } },
        { 14, new string[] { "The weekEND includes Sunday. Checkmate.",
                             "In chinese monday is roughly translated as \"the first day of the week\" so y'all are wrong.",
                             "It's called the weekEND for a reason, not the weekBEGINNING." } },
        {  2, new string[] { "The module is called \"The Cube\", not \"Cube, The\". Why would you ignore the \"The\"?",
                             "You must include \"The\" when sorting alphabetically! \"The\" is a word in the English language! Why would you ever ignore words?" } },
        {  9, new string[] { "Come on, there's gotta be an odd number out there without an E in it!" } },
        { 11, new string[] { "WHAT MONSTER CREATED PINEAPPLE PIZZA??", "My Italian friend does not approve pineapple on pizza therefore it is bad.",
                             "Nobody wants a pineapple-only pizza. Pineapple can die." } },
        { 12, new string[] { "Drinks can be drunk with straws, and drinking soup with a straw is wrong. Ergo, not a drink.",
                             "If I ask for a drink and you hand me soup, you deserve the electric chair.",
                             "Soup is usually salty in flavour, and drinks are flavoured anything but salty, so idk how soup can be a drink.",
                             "Have you ever seen soup come in a cup? No? Of course not. Because soup is not a drink." } },
        { 13, new string[] { "If you seriously think santa is real then you need a reality check.",
                             "Santa can't be real. Ghosts are, but Santa? No way." } },
        {  3, new string[] { "I still don't get the hate for socks with sandals. It's quite comfortable, imo." } },
        {  1, new string[] { "Air Strikes is the only manual I read from Round 2 of KWSNE. Therefore it is the worst one and must not win.",
                             "This manual sucks, it wouldn't even win a consolation prize let alone Round 2.",
                             "Picketing was so much better than Air Strikes. That should win instead." } },
        { 10, new string[] { "Did you just try to tell me that Coca-Cola is better than Pepsi? Deadass??",
                             "Coca-Cola comes in a red can. Red is a bad color. Pepsi comes in a blue can. Blue is a good color. Drink Pepsi, kids." } },
        {  0, new string[] { "No. No no no no no. Hot dogs are far from sandwiches. Very, very far.",
                             "Nah, hot dogs are tacos. Fight me." } },
        {  8, new string[] { "If I understand correctly, eggs have been around way longer than chickens.", 
                             "You never said \"chicken\" eggs, did you? Dinosaurs laid eggs way before chickens existed.",
                             "All chickens come from eggs, even the first one. The egg must have come before the chicken!",
                             "The egg mentioned in the problem didn't specify that it was a *chicken* egg now did it? Therefore, the egg came first." } },
        {  6, new string[] { "Winter allows me to stay inside and avoid talking to people. I hate trying to appear civilized. Let me play with my dollhouse!",
                             "I get to throw snowballs at people during winter! Why wouldn't I love winter?",
                             "Hoodies are so much more cozier to wear than barely anything. Winter is for me." } }
    };
    private static string[] colors = new string[] { "Red", "Yellow", "Blue", "Purple", "Black", "White" };
    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;
    private bool wantRotation = true;
    private bool soundOnClick;
    private bool _animating;
    private bool colorblindMode;
    private float elapsed = 0f;
    private int currentLocation;
    private int startingLocation;
    private int finalLocation;


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
        CrosshairScreen.OnInteract += delegate () { return SubmitLocation(); };
          MessageScreen.OnInteract += delegate () { return ResetLocation(); };
    }
    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        StartCoroutine(RadarRotation());
        colorblindMode = ColorblindMode.ColorblindModeActive;

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
        finalLocation = Random.Range(0, 16);
        currentLocation = startingLocation;
        bool[] startingArray = GenerateBoolArray(startingLocation);
        DisplayModule(startingArray, finalLocation);

        Debug.LogFormat("[Air Strikes #{0}] Starting Location: {1}", _moduleId, locations[startingLocation]);
        Debug.LogFormat("[Air Strikes #{0}] Final Location: {1}", _moduleId, locations[finalLocation]);
    }

    //Arrow OnInteractHandler
    private bool HandleMovement(KMSelectable arrow, int index)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, arrow.transform);
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
        return false;
    }

    //Message Screen onInteract Handler
    private bool ResetLocation()
    {
        currentLocation = startingLocation;
        if (soundOnClick) { Audio.PlaySoundAtTransform("HingeTap", transform); }; // Onclick custom sound
        return false;
    }

    //Crosshair Screen onInteract Handler
    private bool SubmitLocation()
    {
        if (!_animating) StartCoroutine(SubmitSequence(currentLocation));
        //TODO: Solve/Strike Anim + Correct Delays
        return false;
    }

    private IEnumerator SubmitSequence(int ans)
    {
        _animating = true;
        Debug.LogFormat("[Air Strikes #{0}] Submitted Location: {1}", _moduleId, locations[ans]);
        Audio.PlaySoundAtTransform("airStrikesSubmitSound", transform); // Onclick custom sound
        yield return new WaitForSeconds(15f);
        if (ans != finalLocation)   
        {
            Module.HandleStrike();
            Audio.PlaySoundAtTransform("TargetMissed", transform); // custom strike sound
            currentLocation = startingLocation;
        }
        else
        {
            Module.HandlePass();
            Audio.PlaySoundAtTransform("TargetNeutralized", transform); // custom solve sound
        }
        _animating = false;
    }

    //Boolean array
    private bool[] GenerateBoolArray(int initialPosition)
    {
        var arr = new bool[16];
        var targetX = initialPosition % 4;
        var targetY = initialPosition / 4;
        var rows = new bool[4];
        var cols = new bool[4];
        while (rows.Where(i => i == true).Count() != 3 || cols.Where(i => i == true).Count() != 3)
        {
            var randomPos = Enumerable.Range(0, 16).Where(i => i % 4 != targetX && i / 4 != targetY && !arr[i]).PickRandom();
            arr[randomPos] = true;
            rows[randomPos / 4] = true;
            cols[randomPos % 4] = true;
        }
        Debug.LogFormat("[Air Strikes #{0}] The conditions, in reading order are: {1}", _moduleId, arr.Select(i => i ? "T" : "F").Join(" "));
        return arr;
    }

    //Generate module based on the boolean array.
    private void DisplayModule(bool[] arr, int ans)
    {
        //Arrow types are triangular and chevron. 
        bool arrowTypeIsTriangular = arr[4];
        //Content colors are Red, Yellow, Blue, Purple.
        int arrowColorIndex
            = (arr[0] && arr[3]) ? 2 : (arr[0] ? 0 : (arr[3] ? 1 : 3));
        string arrowColorName = colors[arrowColorIndex];
        //Content colors are Red, Yellow, Blue, Purple.
        int crosshairColorIndex
            = (arr[9] && arr[12]) ? 3 : (arr[9] ? 1 : (arr[12] ? 0 : 2));
        string crosshairColorName = colors[crosshairColorIndex];
        //Border colors are Black and White.
        int crosshairBorderColorIndex
            = arr[7] ? 0 : 1;
        string crosshairBorderColorName = colors[crosshairBorderColorIndex + 4];
        //Select one random name based on condition
        List<string> selectedNames = new List<string>();
        foreach (string n in names)
        {
            if ((arr[5] ? n.Length >= 10 : n.Length < 10) && (arr[11] ? n.ToLower().ContainsIgnoreCase("e") : !n.ToLower().ContainsIgnoreCase("e")))
            {
                selectedNames.Add(n);
            }
        }
        string selectedName = selectedNames[Random.Range(0, selectedNames.Count)];
        //Shows 1-6, or 7-12 minute even or odd, PM or AM
        string timestamp
            = (arr[1] ? Random.Range(1, 7) : Random.Range(7, 13)).ToString() + ":" +
              (arr[2] ? 2 * Random.Range(0, 30) : 1 + 2 * Random.Range(0, 30)).ToString("00") + " " +
              (arr[6] ? "PM" : "AM");
        //Border colors are Black and White.
        int messageBorderColorIndex
            = arr[13] ? 0 : 1;
        string messageBorderColorName = colors[messageBorderColorIndex + 4];
        //Status Light directional are TR, BR, BL, TL
        int statusLightPosition
            = (arr[8] && arr[15]) ? 0 : (arr[8] ? 3 : (arr[15] ? 1 : 2));
        //Flips the crosshair and message screens.
        bool flippedScreens
            = arr[14];
        soundOnClick = arr[10];

        string[] messages = Random.Range(0, 2) == 0 ? forTopics[ans] : againstTopics[ans];
        string message = messages[Random.Range(0, messages.Length)];

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

        MessageScreenText.text = selectedName + ": " + timestamp + "\n" + message;

        ModuleBackground.transform.localRotation = Quaternion.Euler(new Vector3(0, statusLightPosition * 90, 0));

        if (flippedScreens)
        {
            MainComponents[1].transform.localPosition = new Vector3(0f, 0f, 0.12f);
            MainComponents[0].transform.localPosition = new Vector3(0f, 0f, -0.04f);
        }

        //Colorblind texts
        ColorblindTexts[0].text = crosshairBorderColorName + "/" + arrowColorName;
        ColorblindTexts[1].text = crosshairColorName;
        ColorblindTexts[2].text = messageBorderColorName;
        ColorblindTexts[0].color = arr[7] ? new Color32(0x00, 0x00, 0x00, 0xFF) : new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        ColorblindTexts[2].color = arr[13] ? new Color32(0x00, 0x00, 0x00, 0xFF) : new Color32(0xFF, 0xFF, 0xFF, 0xFF);

        if (colorblindMode) foreach (TextMesh t in ColorblindTexts) t.gameObject.SetActive(true);
        else foreach (TextMesh t in ColorblindTexts) t.gameObject.SetActive(false);

        //Logging
        Debug.LogFormat("[Air Strikes #{0}] The arrow buttons are in {1} and are {2}.", _moduleId, arrowColorName, arrowTypeIsTriangular ? "triangles" : "not triangles");
        Debug.LogFormat("[Air Strikes #{0}] The crosshair is in {1} with {2} frame.", _moduleId, crosshairColorName, crosshairBorderColorName);
        Debug.LogFormat("[Air Strikes #{0}] The message is in {1} frame and is {2} the crosshair.", _moduleId, messageBorderColorName, arr[14] ? "above" : "below");
        Debug.LogFormat("[Air Strikes #{0}] The message reads: {1}", _moduleId, selectedName + ": " + timestamp + " - " + message);
        Debug.LogFormat("[Air Strikes #{0}] The status light is in {1}.", _moduleId, (arr[8] && arr[15]) ? "top right" : (arr[8] ? "top left" : (arr[15] ? "bottom right" : "bottom left")));
        Debug.LogFormat("[Air Strikes #{0}] Clicking the screen {1} a sound.", _moduleId, arr[10] ? "makes" : "does not make");

    }
    #pragma warning disable 414
    string TwitchHelpMessage = "Use !{0} l/u/r/d // reset // submit // colorblind.";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string validInputs = "lurd";
        command = command.ToLowerInvariant().Trim();
        Match m = Regex.Match(command, @"^(?:([lurd]+)|(submit|reset|colorblind))$");
        /*Capture Groups:
            1.  directional
            2.  submit/clear
        */
        if (!m.Success)
            yield break;
        yield return null;
        if (m.Groups[1].Success)
        {
            char[] directions = m.Groups[1].Value.ToCharArray();
            foreach (char d in directions)
            {
                yield return new WaitForSeconds(0.1f);
                if (TriangleArrows[validInputs.IndexOf(d)].gameObject.activeSelf) TriangleArrows[validInputs.IndexOf(d)].OnInteract();
                else ChevronArrows[validInputs.IndexOf(d)].OnInteract();
            }
        }
        else if (m.Groups[2].Success)
        {
            switch (m.Groups[2].Value)
            {
                case "submit":
                    CrosshairScreen.OnInteract();
                    break;
                case "reset":
                    MessageScreen.OnInteract();
                    break;
                case "colorblind":
                    foreach (TextMesh t in ColorblindTexts) t.gameObject.SetActive(true);
                    break;
            }
        }
        else { Debug.LogFormat("[Air Strikes #{0}] Error in TP Handling, please contact developer.", _moduleId); };
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
        while (currentLocation/4 != finalLocation/4)
        {
            yield return new WaitForSeconds(.1f);
            if (TriangleArrows[3].gameObject.activeSelf) TriangleArrows[3].OnInteract();
            else ChevronArrows[3].OnInteract();
        };
        while (currentLocation != finalLocation)
        {   
            yield return new WaitForSeconds(.1f);
            if (TriangleArrows[2].gameObject.activeSelf) TriangleArrows[2].OnInteract();
            else ChevronArrows[2].OnInteract();
        };
        CrosshairScreen.OnInteract();
    }
}
