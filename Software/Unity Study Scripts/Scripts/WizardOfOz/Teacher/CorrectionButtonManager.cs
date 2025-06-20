using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public struct CorrectionButton
{
    public CorrectionValue value;
    public Button button;
}

public class CorrectionButtonManager : MonoBehaviour
{
    private SolutionField solutionField;
    [SerializeField] private List<CorrectionButton> correctionButtons;
    private SheetManager sheetManager;

    public void SetSolutionField(SolutionField  solutionField)
    {
        this.solutionField = solutionField;
        Initialize();
    }


    private void Initialize()
    {
        foreach (var correctionButton in correctionButtons)
        {
            correctionButton.button.onClick.AddListener(delegate { OnCorrectionButtonClick(correctionButton); });
            if (correctionButton.value == CorrectionValue.EMPTY)
            {
                ManageButtonGroupColors(correctionButton);
            }
        }
    }

    private void OnCorrectionButtonClick(CorrectionButton button)
    {
        sheetManager.UpdateCurrentSheetSolutionField(solutionField.GetIndex(), (int)button.value);
        ManageButtonGroupColors(button);
    }

    private void ManageButtonGroupColors(CorrectionButton button)
    {
        var currentColor = button.button.gameObject.GetComponent<Image>().color;
        button.button.gameObject.GetComponent<Image>().color =
            new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        foreach (var otherButton in correctionButtons.Where(otherButton => otherButton.value != button.value))
        {
            currentColor = otherButton.button.gameObject.GetComponent<Image>().color;
            otherButton.button.gameObject.GetComponent<Image>().color =
                new Color(currentColor.r, currentColor.g, currentColor.b, .333f);
        }
    }

    public void ResetColors()
    {
        foreach (var button in correctionButtons)
        {
            var currentColor = button.button.gameObject.GetComponent<Image>().color;
            if (button.value == CorrectionValue.EMPTY)
            {
                button.button.gameObject.GetComponent<Image>().color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
                continue;
            }
            button.button.gameObject.GetComponent<Image>().color =
                new Color(currentColor.r, currentColor.g, currentColor.b, .333f);
        }
    }

    public void SetSheetManager(SheetManager sheetManager)
    {
        this.sheetManager = sheetManager;
    }
}