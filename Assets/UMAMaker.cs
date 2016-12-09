/*
 * Keenan Johnstone - 11119412 - kbj182
 * Dec 8th 2016 - CMPT306
 */
using UnityEngine;
using System.Collections;
using UMA;
using UMA.PoseTools;

public class UMAMaker : MonoBehaviour
{
    public UMAGeneratorBase generator;
    public SlotLibrary slotLibrary;
    public OverlayLibrary overlayLibrary;
    public RaceLibrary raceLibrary;
    public RuntimeAnimatorController animController;

    private UMADynamicAvatar umaDynamicAvatar;
	private UMAData umaData;
	private UMADnaHumanoid umaDna;
	private UMADnaTutorial umaTutorialDNA;
	
	private int numberOfSlots = 20;

    //Expession stuff
    public UMAExpressionPlayer expressionPlayer;

    //Sliders for Runtime changes
    [Range(0.0f, 1.0f)]
    public float bodyMass = 0.5f;

    //Gender Change toggle
    public bool isMale = true;
    private bool lastMaleState = true;

    //Happiness range
    [Range(-1.0f, 1.0f)]
    public float happy = 0f;

    

    void Start()
    {
        GenerateUMA();
    }

    void Update()
    {
        //If the body mass is changing
        if (bodyMass != umaDna.upperMuscle)
        {
            SetBodyMass(bodyMass);
            umaData.isShapeDirty = true;
            umaData.Dirty();
        }
        if (isMale && !lastMaleState)
        {
            lastMaleState = isMale;
            LoadMaleAsset();
        }
        else if (!isMale && lastMaleState)
        {
            lastMaleState = isMale;
            LoadFemaleAsset();
        }

        //Emotion states
        if (happy != expressionPlayer.midBrowUp_Down)
        {
            expressionPlayer.midBrowUp_Down = happy;
            expressionPlayer.leftMouthSmile_Frown = happy;
            expressionPlayer.rightMouthSmile_Frown = happy;
        }
    }

        void GenerateUMA()
    {
        // Create a new game object and add UMA componenets to it
        GameObject g = new GameObject("MyUMA");
        umaDynamicAvatar = g.AddComponent<UMADynamicAvatar>();
        

        // Initialise Avatar and grab a reference to it's data component
        umaDynamicAvatar.Initialize();
        umaData = umaDynamicAvatar.umaData;
        umaData.OnCharacterCreated += CharacterCreatedCallback;

        // Attach our generator
        umaDynamicAvatar.umaGenerator = generator;
        umaData.umaGenerator = generator;

        // Set up slot Array
        umaData.umaRecipe.slotDataList = new SlotData[numberOfSlots];

        // Set up our Morph references
        umaDna = new UMADnaHumanoid();
        umaTutorialDNA = new UMADnaTutorial();
        umaData.umaRecipe.AddDna(umaDna);
        umaData.umaRecipe.AddDna(umaTutorialDNA);


        // >>> This is where the fun will happen <<<
        CreateMale();

        //Attach our animator Controller to the UMA
        umaDynamicAvatar.animationController = animController;

        // Generate our UMA
        umaDynamicAvatar.UpdateNewRace();

        //Tie the UMA as a child to the gameobject
        g.transform.parent = this.gameObject.transform;
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.identity;
    }

    void CreateMale()
    {
        // Grab a reference to our recipe
        var umaRecipe = umaDynamicAvatar.umaData.umaRecipe;
        umaRecipe.SetRace(raceLibrary.GetRace("HumanMale"));

        SetSlot(0, "MaleEyes");
        AddOverlay(0, "EyeOverlay");

        SetSlot(1, "MaleInnerMouth");
        AddOverlay(1, "InnerMouth");

        SetSlot(2, "MaleFace");
        AddOverlay(2, "MaleHead02");

        SetSlot(3, "MaleTorso");
        AddOverlay(3, "MaleBody02");

        SetSlot(4, "MaleHands");
        LinkOverlay(4, 3);

        SetSlot(5, "MaleLegs");
        LinkOverlay(5, 3);

        SetSlot(6, "MaleFeet");
        LinkOverlay(6, 3);

        AddOverlay(3, "MaleUnderwear01", Color.blue);
        AddOverlay(2, "MaleEyebrow01", Color.black);
    }

    void SetBodyMass(float mass)
    {
        umaDna.upperMuscle = mass;
        umaDna.upperWeight = mass;
        umaDna.lowerMuscle = mass;
        umaDna.lowerWeight = mass;
        umaDna.armWidth = mass;
        umaDna.forearmWidth = mass;
    }

    //Overlay Helpers
    void AddOverlay(int slot, string overlayName)
    {
        umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName));
    }

    void AddOverlay(int slot, string overlayName, Color color)
    {
        umaData.umaRecipe.slotDataList[slot].AddOverlay(overlayLibrary.InstantiateOverlay(overlayName, color));
    }

    void LinkOverlay(int slotNumber, int slotToLink)
    {
        umaData.umaRecipe.slotDataList[slotNumber].SetOverlayList(umaData.umaRecipe.slotDataList[slotToLink].GetOverlayList());
    }

    void RemoveOverlay(int slotNumber, string overlayName)
    {
        umaData.umaRecipe.slotDataList[slotNumber].RemoveOverlay(overlayName);
    }

    void ColorOverlay(int slotNumber, string overlayName, Color color)
    {
        umaData.umaRecipe.slotDataList[slotNumber].SetOverlayColor(color, overlayName);
    }

    //Slot HElpers
    void SetSlot(int slotnumber, string SlotName)
    {
        umaData.umaRecipe.slotDataList[slotnumber] = slotLibrary.InstantiateSlot(SlotName);
    }

    void RemoveSlot(int slotNumber)
    {
        umaData.umaRecipe.slotDataList[slotNumber] = null;
    }

    void LoadMaleAsset()
    {
        UMARecipeBase recipe = Resources.Load("Male") as UMARecipeBase;
        umaDynamicAvatar.Load(recipe);
    }

    void LoadFemaleAsset()
    {
        UMARecipeBase recipe = Resources.Load("Female") as UMARecipeBase;
        umaDynamicAvatar.Load(recipe);
    }

    void CharacterCreatedCallback(UMAData umaData)
    {
        UMAExpressionSet expSet = umaData.umaRecipe.raceData.expressionSet;
        expressionPlayer = umaData.gameObject.AddComponent<UMAExpressionPlayer>();
        expressionPlayer.expressionSet = expSet;
        expressionPlayer.umaData = umaData;
        expressionPlayer.Initialize();
        expressionPlayer.enableBlinking = true;
        expressionPlayer.enableSaccades = true;
    }
}
