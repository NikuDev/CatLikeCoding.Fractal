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
    /// Array to hold the shapes which will be used in the generation of the
    /// Fractal. (i.e. Cube, Sphere, Cylinder)
    /// </summary>
    public Mesh[] _meshes;

    /// <summary>
    /// To control the maximum amount of GameObject instantiated for the Fractal,
    /// we use this property
    /// </summary>
    public int MaxDepth;

    public float SpawnProbability;
        
    public float ChildScale;

    public float MaxRotationSpeed;

    public float MaxTwist;

    private int _depth;

    private float _rotationSpeed;

    private Material[,] _materials;

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

        if (this._materials == null)
        {
            this.InitializeMaterials();
        }

        GetComponent<MeshFilter>().mesh = this._meshes[Random.Range(0, this._meshes.Length)];

        GetComponent<MeshRenderer>().material = 
            this._materials[this._depth, Random.Range(0,2)];

        if(this._depth < MaxDepth)
        {
            // When you're creating a coroutine in Unity, what you're really doing 
            // is creating an iterator.When you pass it to the StartCoroutine method, it will get 
            // stored and gets asked for its next item every frame, until it is finished.
           StartCoroutine(this.CreateChildren());
        }

        // let's set a random rotationspeed for this instance
        this._rotationSpeed = Random.Range(-this.MaxRotationSpeed, this.MaxRotationSpeed);

        // let's also set a random rotation for this instance
        this.transform.Rotate(Random.Range(-this.MaxTwist, this.MaxTwist), 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0f, this._rotationSpeed * Time.deltaTime, 0f);
    }

    private void InitializeMaterials()
    {
        this._materials = new Material[this.MaxDepth + 1, 2];
        for (int i = 0; i <= this.MaxDepth; i++)
        {
            float t = (float)i / (this.MaxDepth - 1f);
            t *= t;

            this._materials[i,0] = new Material(this.Material);
            this._materials[i,0].color =
                Color.Lerp(Color.white, Color.red, t);

            this._materials[i,1] = new Material(this.Material);
            this._materials[i,1].color =
                Color.Lerp(Color.white, Color.yellow, t);
        }

        this._materials[MaxDepth, 0].color = Color.green;
        this._materials[MaxDepth, 1].color = Color.cyan;
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
            if(Random.value < this.SpawnProbability)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));

                new GameObject("Fractal Child")
                    .AddComponent<Fractal>()
                    .InitializeChild(this, i);
            }
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
        this.SpawnProbability = parent.SpawnProbability;
        this.MaxRotationSpeed = parent.MaxRotationSpeed;
        this._meshes = parent._meshes;
        this._materials = parent._materials;
        this._depth = parent._depth + 1;

        // The parent–child relationship between game objects is 
        // defined by their transformation hierarchy.
        this.transform.parent = parent.transform;

        this.transform.localScale = Vector3.one * this.ChildScale;

        this.transform.localPosition = _childDirections[childIndex] * (0.5f + 0.5f * this.ChildScale);

        this.transform.rotation = _childOrientations[childIndex];
    }
}
