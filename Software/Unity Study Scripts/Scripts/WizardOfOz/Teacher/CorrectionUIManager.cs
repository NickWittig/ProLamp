using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CorrectionUIManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text sheetInfoText;
    [SerializeField] private SheetManager sheetManager;
    [SerializeField] private Image teacherSheetImage;
    [SerializeField] private Button resetButton;
    private List<CorrectionButtonManager> currentButtonManagers = new();

    private void Start()
    {
        SessionManager.Instance.OnServerScenesLoaded += OnScenesLoaded_Start;

        if (resetButton == null) return;
        resetButton.onClick.AddListener(delegate {
            foreach (var manager in currentButtonManagers) manager.ResetColors();
            sheetManager.ResetSolutionFields(sheetManager.currentSheet);
        });
    }

    private void OnScenesLoaded_Start()
    {
        if (SessionManager.GetRole() == Role.Teacher) return;

        Assert.IsNotNull(MarkerHandler.Instance);
        MarkerHandler.Instance.OnMarkerChanged += OnMarkerChanged_UpdateTeacherUI;
    }

    private void OnMarkerChanged_UpdateTeacherUI(Marker marker)
    {
        Debug.Log("teacher ui updated from client");
        UpdateTeacherUIServerRPC(marker.id);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateTeacherUIServerRPC(int markerId)
    {
        UpdateTeacherUI(markerId);
    }
    public void UpdateTeacherUI(int markerId)
    {
        var sheet = sheetManager.TryGetSheetByMarkerId(markerId);
        if (sheet == null) return;
        sheetInfoText.text = $"Blatt: {sheet.GetMarkerId()}";
        CorrectionButtonPool.Instance.DeactivateAllObjects();
        currentButtonManagers.Clear();
        foreach (var solutionField in sheet.GetSolutionFields())
        {
            var correctionButtonManager = CorrectionButtonPool.Instance.GetNextObject().GetComponent<CorrectionButtonManager>();
            correctionButtonManager.SetSolutionField(solutionField.GetComponent<SolutionField>());
            correctionButtonManager.SetSheetManager(sheetManager);
            currentButtonManagers.Add(correctionButtonManager);
        }
        teacherSheetImage.sprite = sheet.GetSolutionSprite();
    }


}