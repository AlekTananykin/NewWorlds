using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MechFootSteps : MonoBehaviour
{

    public AudioClip audioFootStep;

    AudioSource ASFootStep;

    void Start()
    {
        ASFootStep = GetComponent<AudioSource>();
        ASFootStep.clip = audioFootStep;

    }

    void FootStep()
    {
        ASFootStep.Play();
    }
}

