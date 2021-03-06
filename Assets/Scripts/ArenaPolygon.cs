﻿using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class ArenaPolygon : MonoBehaviour
{
    [Range(3, 12)]
    public int sides;
    [Range(3f, 5f)]
    public float radius;
    public int minSides = 3;
    public int maxSides = 12;
    public float minRadius = 3f;
    public float maxRadius = 5f;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private PolygonCollider2D polygonCollider2D;
    [HideInInspector] public Vector3[] vertices;

    void Start ()
    {
	    mesh = new Mesh();
	    meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
	    polygonCollider2D = GetComponent<PolygonCollider2D>();
	    GenerateMesh();
    }

    void OnValidate()
	{
        if (mesh != null)
	        GenerateMesh();
	}

    [ContextMenu("Generate")]
    void GenerateMesh()
    {
        mesh.Clear();
        vertices = new Vector3[sides];
        int[] triangles = new int[3 * (sides - 2)];
        Vector3 point = Vector3.right * radius;
        float rotation = 360f / sides;
        for (int i = 0; i < sides; i++)
        {
            vertices[i] = point;
            point = Quaternion.Euler(0, 0, -rotation) * point;
        }
        for (int i = 0; i < sides - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        polygonCollider2D.points = vertices.Select(v => (Vector2)v).ToArray();
    }

    public void IncreaseSides()
    {
        sides++;
        GenerateMesh();
    }

    public void DecreaseSides()
    {
        sides--;
        GenerateMesh();
    }

    public void RandomSides()
    {
        sides = UnityEngine.Random.Range(3, 13);
        GenerateMesh();
    }
    
    private IEnumerator ChangeRadius(float newRadius)
    {
        newRadius = Mathf.Clamp(newRadius, minRadius, maxRadius);
        radius = Mathf.Lerp(radius, newRadius, 0.5f);
        GenerateMesh();
        yield return new WaitForSeconds(0.5f);
        radius = newRadius;
        GenerateMesh();
    }

    public void RadiusUp()
    {
        StartCoroutine(ChangeRadius(radius + 0.5f));
    }

    public void RadiusDown()
    {
        StartCoroutine(ChangeRadius(radius - 0.5f));
    }

    public void RandomRadius()
    {
        StartCoroutine(ChangeRadius(UnityEngine.Random.Range((int)minRadius, (int)maxRadius + 1)));
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ball")
        {
            Ball ballComponent = other.GetComponent<Ball>();
            OnBallExit(new BallExitArgs { LastHit = ballComponent.lastHit });
            ballComponent.Explode();
        }
    }

    //Events
    public event EventHandler<BallExitArgs> BallExit;
    protected virtual void OnBallExit(BallExitArgs e)
    {
        EventHandler<BallExitArgs> handler = BallExit;
        if (handler != null)
            handler(this, e);
    }
}

public class BallExitArgs : EventArgs
{
    public PlayerIndex LastHit;
}