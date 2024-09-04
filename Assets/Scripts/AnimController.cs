using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnimController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Button closeButton;
    private bool _switcher = true;


    private void Start()
    {
        closeButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        anim.SetBool("IsClosed", _switcher);
        _switcher = !_switcher;
        print(_switcher);
    }
}