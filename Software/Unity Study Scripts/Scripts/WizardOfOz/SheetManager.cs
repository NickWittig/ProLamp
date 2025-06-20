using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SheetManager : NetworkBehaviour
{

    public Marker currentMarker { private set; get; }
    public Sheet currentSheet { private set; get; }
    private MarkerTracker markerTracker;


    [SerializeField] private List<Sheet> sheets;

    private void Start()
    {
        SessionManager.Instance.OnServerScenesLoaded += OnScenesLoaded_Start;
    }

    private void Update()
    {
        if (MarkerHandler.Instance == null) return;
        if (currentSheet == null) return;
        currentSheet.gameObject.SetActive(MarkerHandler.Instance.IsMarkerActive());

    }
    private void OnScenesLoaded_Start()
    {
        if (SessionManager.GetRole() == Role.Teacher) return;
        if (markerTracker != null) return;
        markerTracker = (MarkerTracker)FindObjectOfType(typeof(MarkerTracker));
        Assert.IsNotNull(markerTracker);
        MarkerHandler.Instance.OnMarkerChanged += OnMarkerChanged_UpdateCurrentSheet;
    }

    private void OnMarkerChanged_UpdateCurrentSheet(Marker marker)
    {
        Debug.Log($"Found Marker with ID {marker.id}");
        if (marker.id == 0) return;
        var sheet = TryGetSheetByMarkerId(marker.id);
        if (sheet == null) return;
        if (currentSheet != null) currentSheet.gameObject.SetActive(false);
        currentSheet = sheet;
        currentMarker = marker;
        UpdateTeacherSheetServerRPC(marker.id);
        currentSheet.gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetSolutionFieldsServerRPC()
    {
        ResetSolutionFields(currentSheet);
    }


    [ServerRpc(RequireOwnership = false)]
    public void UpdateTeacherSheetServerRPC(int markerId)
    {
        Debug.Log("sheet updated on server");
        if (currentSheet != null)
        {
            currentSheet.gameObject.SetActive(false);
        }
        currentSheet = TryGetSheetByMarkerId(markerId);
        currentSheet.gameObject.SetActive(true);
    }

    public void ResetSolutionFields(Sheet sheet)
    {
        foreach (var solutionField in sheet.GetSolutionFields())
        {
            UpdateCurrentSheetSolutionField(solutionField.GetIndex(), (int)CorrectionValue.EMPTY);
        }
    }

    public Sheet TryGetSheetByMarkerId(int markerId)
    {
        return sheets.FirstOrDefault(sheet => sheet.GetMarkerId() == markerId);
    }

    public void UpdateCurrentSheetSolutionField(int index, int value)
    {
        currentSheet.UpdateSolutionField(index, value);
        UpdateCurrentSheetSolutionFieldClientRPC(index, value);
    }
    [ClientRpc]
    private void UpdateCurrentSheetSolutionFieldClientRPC(int index, int value)
    {
        currentSheet.UpdateSolutionField(index, value);
    }
}
