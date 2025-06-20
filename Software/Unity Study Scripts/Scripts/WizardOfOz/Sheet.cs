using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class Sheet : MonoBehaviour
{
    [SerializeField] private int markerId;
    [SerializeField] private List<SolutionField> solutionFields;
    [SerializeField] private Sprite solutionSprite;


    private void Start()
    {
        gameObject.SetActive(false);
    }

    public Sprite GetSolutionSprite()
    {
        return solutionSprite;
    }
    public int GetMarkerId()
    {
        return markerId;
    }

    public List<SolutionField> GetSolutionFields()
    {
        return solutionFields;
    }


    public void UpdateSolutionField(int index, int value)
    {
        solutionFields[index].UpdateByCorrectionValue((CorrectionValue)value);
    }

}
