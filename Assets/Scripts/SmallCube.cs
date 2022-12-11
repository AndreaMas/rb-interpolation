using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Component for all Small Cubes.
/// Has callable function to change color when interacted with.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ClientSmoothMovement))]
public class SmallCube : MonoBehaviour
{
    public bool touched = false;
    public bool touchedAnimPlaying = false;
    private Material thisMaterial;
    private Color colorIdle = Color.yellow;
    private Color colorTouched = Color.red;
    private float timeFadeDuration = 2f;
    private float timeFadePassed = 0f;
    private Material material;
    public Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        thisMaterial = GetComponent<Renderer>().material;
        thisMaterial.color = colorIdle;
    }

    public void Touched()
    {
        touched = true;
    }

    private void Update()
    {
        if (touched)
        {
            thisMaterial.color = colorTouched;
            timeFadePassed = 0f;
            if (touchedAnimPlaying == false)
            {
                touchedAnimPlaying = true;
                StartCoroutine(FadeColor());
            }
            StartCoroutine(RemainTouchedFor());
        }
    }

    private IEnumerator FadeColor()
    {
        while (timeFadePassed < timeFadeDuration)
        {
            timeFadePassed += Time.deltaTime;
            thisMaterial.color = Color.Lerp(colorTouched, colorIdle, timeFadePassed / timeFadeDuration);
            yield return null;
        }

        touchedAnimPlaying = false;
    }

    // NOTE: variable remains true for a second just to be (almost) sure info is sent
    private IEnumerator RemainTouchedFor()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            touched = false;
        }
    }

}

