using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleDependentObject : MonoBehaviour
{
    [SerializeField] private Role role;
    private void Awake()
    {   
        gameObject.SetActive(SessionManager.GetRole() == role);
    }
}
