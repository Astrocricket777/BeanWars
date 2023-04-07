using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsScreen : MonoBehaviour
{
    public Toggle FullscreenTog, VsyncTog;
    public TMP_Text ResoulutionText;

    public List<ResItem> Resolutions = new List<ResItem>();

    private int SelectedRes;
    

    void Start()
    {
        FullscreenTog.isOn = Screen.fullScreen;

        if (QualitySettings.vSyncCount == 0)
        {
            VsyncTog.isOn = false;
        }
        else
        {
            VsyncTog.isOn = true;
        }
    }

    public void ResLeft()
    {
        SelectedRes--;
        if (SelectedRes < 0)
        {
            SelectedRes = 0;
        }

        UpdateResLabel();
    }

    public void ResRight()
    {
        SelectedRes++;
        if (SelectedRes > Resolutions.Count - 1)
        {
            SelectedRes = Resolutions.Count - 1;
        }

        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        ResoulutionText.text = Resolutions[SelectedRes].Horizontal.ToString() + "x" + Resolutions[SelectedRes].Vertical.ToString();
    }

    public void ApplyGraphics()
    {
        if (VsyncTog.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            VsyncTog.isOn = false;
        }

        Screen.SetResolution(Resolutions[SelectedRes].Horizontal, Resolutions[SelectedRes].Vertical, FullscreenTog.isOn);
    }

    
}

[System.Serializable]
public class ResItem
{
    public int Horizontal, Vertical;
}
