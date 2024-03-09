using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandGripManager : MonoBehaviour
{
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private TwoBoneIKConstraint rightIK;
    [SerializeField] private TwoBoneIKConstraint leftIK;
    [SerializeField] private Transform rightGrip;
    [SerializeField] private Transform leftGrip;

    void Start()
    {
        SetGrips();
    }

    public void SetGrips()
    {
        rightIK.data.target = rightGrip;
        leftIK.data.target = leftGrip;
        rigBuilder.Build();
    }
}
