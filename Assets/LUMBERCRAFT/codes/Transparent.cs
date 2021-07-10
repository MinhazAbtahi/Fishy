using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//"LastTree and LastTreeTop gameobject hierarchy needs to be maintained for expected output :3"
public class Transparent : MonoBehaviour
{
    public GameObject player;

    GameObject lastTree;
    GameObject lastTreeTop;
    RaycastHit hit;

    int layerMask;

    public Material[] normalMats;
    public Material[] transparentMats;
    public bool isHidden;
    public Material regularMat;

    private void Start()
    {
        layerMask = 1 << 20;
    }


    void FixedUpdate()
    {

        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.parent.GetComponent<TreeController>().treeType == TreeType.normal)
            {

                lastTree = hit.transform.GetChild(0).GetChild(0).gameObject;
                lastTreeTop = hit.transform.GetChild(0).GetChild(1).gameObject;
            }
            if (hit.transform.parent.GetComponent<TreeController>().treeType == TreeType.adTree)
            {

                lastTree = hit.transform.gameObject;
                lastTreeTop = hit.transform.gameObject;
            }
            if (!isHidden)
                regularMat = lastTree.GetComponent<Renderer>().material;

            TreeTransparency(lastTree);
            TreeTransparency(lastTreeTop);
        }
        else
        {
            if (lastTree != null && lastTreeTop != null)
            {

                TreeReset(lastTree);
                TreeReset(lastTreeTop);
            }
        }
    }
    void TreeTransparency(GameObject x)
    {
        isHidden = true;
        if (hit.transform.parent.GetComponent<TreeController>().treeType == TreeType.normal)
            x.GetComponent<Renderer>().material = transparentMats[0];
        if (hit.transform.parent.GetComponent<TreeController>().treeType == TreeType.adTree)
            x.GetComponent<Renderer>().material = transparentMats[1];

        //Material mat = x.GetComponent<Renderer>().material;
        //Color color = mat.GetColor("_Color");
        //color.a = 0.5f;
        //mat.color = color;
        //mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //mat.SetInt("_ZWrite", 0);
        //mat.DisableKeyword("_ALPHATEST_ON");
        //mat.EnableKeyword("_ALPHABLEND_ON");
        //mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //mat.renderQueue = 3000;
    }
    void TreeReset(GameObject x)
    {
        isHidden = false;
        x.GetComponent<Renderer>().material = regularMat;
        //if (hit.transform.parent.GetComponent<TreeController>().treeType == TreeType.normal)
        //    x.GetComponent<Renderer>().material = normalMats[0];
        //if (hit.transform.parent.GetComponent<TreeController>().treeType == TreeType.adTree)
        //    x.GetComponent<Renderer>().material = normalMats[1];

        //Material mat = x.GetComponent<Renderer>().material;
        //mat.SetFloat("_Mode", 0);
        //mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        //mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        //mat.SetInt("_ZWrite", 1);
        //mat.DisableKeyword("_ALPHATEST_ON");
        //mat.DisableKeyword("_ALPHABLEND_ON");
        //mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //mat.renderQueue = -1;
        //x = null;
    }
}
