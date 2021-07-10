using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshFlareDeformer : MonoBehaviour
{

    [Tooltip("Defines whether the Deformer is static or Dynamic, if true, the deformer will only be calculated once at Start, Iportant if you want to create a script to stack deformers")]
    public bool isStatic = false;
    public enum Axis { X, Y, Z };
    [Tooltip("Choose the Axis you want the deformer to work on")]
    public Axis DeformAxis = Axis.Y;
    [Tooltip("Use the curve to determin the influence of the deformer over the chosen Axis")]
    public AnimationCurve Refinecurve;
    [Tooltip("Multiply the over-all effect of the deformer")]
    public float Multiplier = 1.0f;
    private float minValue = 0f;
    public float maxValue = 1f;
    private float matShowDelay = 0.01f;
    private Renderer rend;
    public GameObject target;
    public Material changingMat;
    public Material defaultMat;

    // public float xThresh;
    // public float yThresh;
    // public float zThresh;
    // public float midZ;
    // public float midX;
    // public float midY;

    // private Vars
    Mesh deformingMesh;
    Mesh targetMesh;
    public Vector3[] originalVertices, displacedVertices;
    Vector3[] normalVerts;
    public Vector3[] targetVertices;
    public Vector3[] processedTargetVertices;
    float smallestY, largestY, smallestX, largestX, smallestZ, largestZ, normalized, curveValue = 0;
    private bool ISkinned = true;

    void Start()
    {


        if (GetComponent<SkinnedMeshRenderer>() == null)
        {
            Debug.Log("Please assign this script to a Skinned Mesh, you can find out which mesh is skinned by checking if it has a Skinned Mesh Renderer Attached to it");
            ISkinned = false;
        }
        else
        {
			

            targetMesh = target.GetComponent<SkinnedMeshRenderer>().sharedMesh;
            if (Refinecurve.length == 0)
            {
                Refinecurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            }
            deformingMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            originalVertices = deformingMesh.vertices;
            targetVertices = targetMesh.vertices;
            normalVerts = new Vector3[originalVertices.Length];
			setTargetVertices();

            displacedVertices = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                normalVerts[i] = Vector3.Normalize(deformingMesh.normals[i]);

                displacedVertices[i] = originalVertices[i];

                if (displacedVertices[i].y < smallestY)
                {
                    smallestY = displacedVertices[i].y;
                }
                if (displacedVertices[i].y > largestY)
                {
                    largestY = displacedVertices[i].y;
                }
                if (displacedVertices[i].x < smallestX)
                {
                    smallestX = displacedVertices[i].x;
                }
                if (displacedVertices[i].x > largestX)
                {
                    largestX = displacedVertices[i].x;
                }
                if (displacedVertices[i].z < smallestZ)
                {
                    smallestZ = displacedVertices[i].z;
                }
                if (displacedVertices[i].z > largestZ)
                {
                    largestZ = displacedVertices[i].z;
                }
            }
            flareUP();
        }
        StartCoroutine(ShapeChange());
        defaultMat = GetComponent<Renderer>().material;
        //setMatBlend();

        //setStartershape();
        Multiplier = 1f;
    }


    void setTargetVertices()
    {
		processedTargetVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
			float leastDiff = 9999;
			int index = -1;
			for(int j = 0; j < targetVertices.Length; j++) {
				float diff = Mathf.Abs(originalVertices[i].x - targetVertices[j].x) + 
				Mathf.Abs(originalVertices[i].y - targetVertices[j].y) + 
				Mathf.Abs(originalVertices[i].z - targetVertices[j].z);
				if(diff < leastDiff) {
					leastDiff = diff;
					index = j;
				}
			}
			processedTargetVertices[i] = targetVertices[index];

        }

    }
    public void setMatBlend()
    {
        rend = GetComponent<Renderer>();
        rend.material = changingMat;
    }
    public void setdefaultMat()
    {
        rend = GetComponent<Renderer>();
        rend.material = defaultMat;
    }

    private IEnumerator ShapeChange()
    {
        while (true)
        {

            yield return new WaitForSeconds(2f);
            isStatic = false;
            for (Multiplier = minValue; Multiplier <= maxValue; Multiplier = Multiplier + matShowDelay)
            {
                yield return new WaitForSeconds(0f);
            }
            yield return new WaitForSeconds(2f);
            for (Multiplier = maxValue; Multiplier >= minValue; Multiplier = Multiplier - matShowDelay)
            {
                yield return new WaitForSeconds(0f);
            }
        }
    }

    void FixedUpdate()
    {
        if (!isStatic && ISkinned)
        {
            flareUP();
        }
    }

    void flareUP()
    {
        for (int i = 0; i < originalVertices.Length; i++)
        {
            float x, y, z;
            x = originalVertices[i].x;
            y = originalVertices[i].y;
            z = originalVertices[i].z;

            float target_x = processedTargetVertices[i].x;
            float target_y = processedTargetVertices[i].y;
            float target_z = processedTargetVertices[i].z;

            float new_x = x;
            float new_y = y;
            float new_z = z;
            switch (DeformAxis)
            {
                case Axis.X:
                    normalized = (x - smallestX) / (largestX - smallestX);
                    curveValue = Refinecurve.Evaluate(normalized);


                    new_y = (y + normalVerts[i].y / 100000.0f) * Multiplier * curveValue;
                    new_z = (z + normalVerts[i].z / 100000.0f) * Multiplier * curveValue;
                    break;

                case Axis.Y:

                    // float midY = (largestY - smallestY) / 2 + smallestY;
                    // float midX = (largestX - smallestX) / 2 + smallestX;
                    // float midZ = (largestZ - smallestZ) / 2 + smallestZ;

                    // normalized = (y - smallestY) / (largestY - smallestY);
                    // curveValue = Refinecurve.Evaluate(normalized);
                    // // curveValue = 1;
                    // if (x > midX - xThresh && x < midX + xThresh && y > midY - yThresh && y < midY + yThresh
                    //     && z > midZ - zThresh && z < midZ + zThresh
                    // )
                    // {
                    //     new_x = (x + normalVerts[i].x / 100000.0f) * Multiplier * curveValue;
                    //     new_z = (z + normalVerts[i].z / 100000.0f) * Multiplier * curveValue;
                    // }
                    // else if (y < midY - yThresh || y > midY + yThresh)
                    // {
                    //     if (y < midY - yThresh)
                    //     {
                    //         new_x = Mathf.Lerp(x, (x + normalVerts[i].x / 100000.0f) * Multiplier * curveValue, normalized);
                    //         new_z = Mathf.Lerp(z, (z + normalVerts[i].z / 100000.0f) * Multiplier * curveValue, normalized);
                    //     }
                    //     else
                    //     {
                    //         new_x = Mathf.Lerp(x, (x + normalVerts[i].x / 100000.0f) * Multiplier * curveValue, 1 - normalized);
                    //         new_z = Mathf.Lerp(z, (z + normalVerts[i].z / 100000.0f) * Multiplier * curveValue, 1 - normalized);

                    //     }


                    // }

                    new_x = Mathf.Lerp(x, target_x, Multiplier);
                    new_y = Mathf.Lerp(y, target_y, Multiplier);
                    new_z = Mathf.Lerp(z, target_z, Multiplier);

					// print(x - target_x);



                    break;

                case Axis.Z:
                    normalized = (z - smallestZ) / (largestZ - smallestZ);
                    curveValue = Refinecurve.Evaluate(normalized);
                    new_x = (x + normalVerts[i].x / 100000.0f) * Multiplier * curveValue;
                    new_y = (y + normalVerts[i].y / 100000.0f) * Multiplier * curveValue;
                    break;
            }

            Vector3 newvertPos = new Vector3(new_x, new_y, new_z);
            displacedVertices[i] = newvertPos;
        }

        deformingMesh.vertices = displacedVertices;
    }

    void OnApplicationQuit()
    {
        Multiplier = 1f;
        Refinecurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        if (deformingMesh != null)
        {
            flareUP();
        }
    }

    void setStartershape()
    {
        deformingMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        //deformingMesh.UploadMeshData(true);
        //deformingMesh.UploadMeshData(false);
        Multiplier = 1f;
        //Refinecurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        ////if (deformingMesh != null)
        ////{
        //    flareUP();
        ////}
    }

    public void setDefault()
    {
        Multiplier = 1f;
        Refinecurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
        if (deformingMesh != null)
        {
            flareUP();
        }
    }
}
