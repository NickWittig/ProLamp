using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;


public class SolutionField : MonoBehaviour
{
    private readonly Dictionary<CorrectionValue, Sprite> correctionPatternImages = new();
    private readonly Dictionary<CorrectionValue, Color> correctionColors = new();
    [SerializeField] private Image solutionPatternImage;
    [SerializeField] private TMP_Text solutionText;
    [SerializeField] private int index;
    private Image solutionFieldImage;
    private CorrectionValue correctionValue;

    private void Awake()
    {
        Debug.Log("SolutionField Awakened");
        solutionFieldImage  = GetComponent<Image>();
        var truePatternImage = Resources.Load<Sprite>("Media/Pictograms/checkmark");
        Assert.IsNotNull(truePatternImage);
        correctionPatternImages.Add(CorrectionValue.TRUE, truePatternImage);
        var falsePatternImage = Resources.Load<Sprite>("Media/Pictograms/cross");
        Assert.IsNotNull(falsePatternImage);
        correctionPatternImages.Add(CorrectionValue.FALSE, falsePatternImage);
        correctionPatternImages.Add(CorrectionValue.EMPTY, null);

        ColorUtility.TryParseHtmlString("#A3CB38", out var trueColor);
        correctionColors.Add(CorrectionValue.TRUE, trueColor);
        ColorUtility.TryParseHtmlString("#EA2027", out var falseColor);
        correctionColors.Add(CorrectionValue.FALSE, falseColor);
        correctionColors.Add(CorrectionValue.EMPTY, new Color(0f, 0, 0f, 0f));
    }

    private void Start()
    {
        UpdateByCorrectionValue(CorrectionValue.EMPTY);
    }

    public int GetIndex()
    {
        return index;
    }

    public CorrectionValue GetCorrectionValue()
    {
        return correctionValue;
    }
    public void UpdateByCorrectionValue(CorrectionValue value)
    {
        correctionValue = value;
        solutionPatternImage.sprite = correctionPatternImages[value];
        solutionPatternImage.color = correctionColors[value];
        solutionFieldImage.color = correctionColors[value];
    }
}
