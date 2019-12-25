﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    /// <summary>
    /// a Mesh is a construct used by the graphics hardware to draw complex stuff. 
    /// It's a 3D object that's either imported into Unity, one of Unity's default shapes, or generated by code.
    /// </summary>
    public Mesh Mesh;

    /// <summary>
    /// Materials are used to define the visual properties of objects. 
    /// They can range from very simple, like a constant color, to very complex.
    /// </summary>
    public Material Material;

    /// <summary>
    /// To control the maximum amount of GameObject instantiated for the Fractal,
    /// we use this property
    /// </summary>
    public int MaxDepth;
        
    public float ChildScale;

    private int _depth;

    private static Vector3[] _childDirections =
    {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    private static Quaternion[] _childOrientations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f, 0f, -90f),
        Quaternion.Euler(0f, 0f, 90f),
        Quaternion.Euler(90f, 0f, 0f),
        Quaternion.Euler(-90f, 0f, 0f)
    };


    // Start is called before the first frame update
    void Start()
    {
        // The AddComponent method creates a new component of a certain type, 
        // attaches it to the game object, and returns a reference to it.
        gameObject.AddComponent<MeshFilter>().mesh = this.Mesh;
        gameObject.AddComponent<MeshRenderer>().material = this.Material;

        if(this._depth < MaxDepth)
        {

            // When you're creating a coroutine in Unity, what you're really doing 
            // is creating an iterator.When you pass it to the StartCoroutine method, it will get 
            // stored and gets asked for its next item every frame, until it is finished.
           StartCoroutine(this.CreateChildren());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Think of coroutines as methods in which you can insert pause statements.
    /// While the method invocation is paused, the rest of the program continues.
    /// Though this point of view is too simplistic, it is all we need to make
    /// use of it right now.
    /// </summary>
    private IEnumerator CreateChildren()
    {
        // The yield statement is used by iterators to make life easy for them. 
        // To make enumeration possible, you'd need to keep track of your progress
        // This involves some boilerplate code that is essentially always the 
        // same. What you'd really want is to just write something like return 
        // firstItem; return secondItem; until you are done. The yield statement 
        // allows you to do exactly that.
        for(int i=0; i < _childDirections.Length; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            new GameObject("Fractal Child")
                .AddComponent<Fractal>()
                .InitializeChild(this, i);
        }
    }

    /// <summary>
    /// To control the amount of Fractals instantiated, we need to copy
    /// some values from the parent and increase the depth
    /// </summary>
    /// <param name="parent">GameObject to add a new Fractal child to</param>
    private void InitializeChild(Fractal parent, int childIndex)
    {
        this.Mesh = parent.Mesh;
        this.Material = parent.Material;
        this.MaxDepth = parent.MaxDepth;
        this.ChildScale = parent.ChildScale;
        this._depth = parent._depth + 1;

        // The parent–child relationship between game objects is 
        // defined by their transformation hierarchy.
        this.transform.parent = parent.transform;

        this.transform.localScale = Vector3.one * this.ChildScale;

        this.transform.localPosition = _childDirections[childIndex] * (0.5f + 0.5f * this.ChildScale);

        this.transform.rotation = _childOrientations[childIndex];
    }
}
