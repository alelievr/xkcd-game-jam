using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchGUI : MonoBehaviour
{
    public void SwitchToCraft()
    {
        SceneSwitcher.instance.ShowCraft();
    }
}
