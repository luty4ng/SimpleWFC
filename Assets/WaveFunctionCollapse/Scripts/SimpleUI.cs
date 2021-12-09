using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUI : MonoBehaviour
{
    public Dropdown dropdown;
    public Toggle showProcess;
    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;
    public Button generateButton;
    public Text valueX;
    public Text valueY;
    public Text valueZ;
    private ColorBlock activeColorBlock;
    private ColorBlock deactiveColorBlock;

    void Start()
    {
        activeColorBlock = generateButton.colors;
        deactiveColorBlock = generateButton.colors;
        deactiveColorBlock.normalColor = Color.gray;
        sliderX.value = WFCGenerator.instance.mapSize.x;
        sliderY.value = WFCGenerator.instance.mapSize.y;
        sliderZ.value = WFCGenerator.instance.mapSize.z;
        showProcess.isOn = WFCGenerator.instance.showProcess;
        dropdown.value = (int)WFCGenerator.instance.collapseMode;
    }

    private void Update()
    {
        bool canUse = !WFCGenerator.instance.isGenerating;
        dropdown.enabled = canUse;
        showProcess.enabled = canUse;
        sliderX.enabled = canUse;
        sliderY.enabled = canUse;
        sliderZ.enabled = canUse;
        generateButton.enabled = canUse;
        dropdown.colors = canUse ? activeColorBlock : deactiveColorBlock;
        showProcess.colors = canUse ? activeColorBlock : deactiveColorBlock;
        sliderX.colors = canUse ? activeColorBlock : deactiveColorBlock;
        sliderY.colors = canUse ? activeColorBlock : deactiveColorBlock;
        sliderZ.colors = canUse ? activeColorBlock : deactiveColorBlock;
        generateButton.colors = canUse ? activeColorBlock : deactiveColorBlock;
        valueX.text = sliderX.value.ToString();
        valueY.text = sliderY.value.ToString();
        valueZ.text = sliderZ.value.ToString();
    }

    public void OnDropDownChange()
    {
        int value = dropdown.value;
        switch (value)
        {
            case 0:
                WFCGenerator.instance.collapseMode = CollapseMode.LowestEntropy;
                break;
            case 1:
                WFCGenerator.instance.collapseMode = CollapseMode.HighestEntropy;
                break;
            case 2:
                WFCGenerator.instance.collapseMode = CollapseMode.Random;
                break;
            default:
                break;
        }
    }

    public void OnToggleChange()
    {
        WFCGenerator.instance.showProcess = showProcess.isOn;
    }

    public void OnSliderXChange()
    {
        int value = (int)sliderX.value;
        int Y = WFCGenerator.instance.mapSize.y;
        int Z = WFCGenerator.instance.mapSize.z;
        WFCGenerator.instance.mapSize = new Vector3Int(value, Y, Z);
    }

    public void OnSliderYChange()
    {
        int value = (int)sliderY.value;
        int X = WFCGenerator.instance.mapSize.x;
        int Z = WFCGenerator.instance.mapSize.z;
        WFCGenerator.instance.mapSize = new Vector3Int(X, value, Z);
    }

    public void OnSliderZChange()
    {
        int value = (int)sliderZ.value;
        int X = WFCGenerator.instance.mapSize.x;
        int Y = WFCGenerator.instance.mapSize.y;
        WFCGenerator.instance.mapSize = new Vector3Int(X, Y, value);
    }

    public void Generate()
    {
        WFCGenerator.instance.ClearSlot();
        WFCGenerator.instance.GenerateBlock();
    }
}
