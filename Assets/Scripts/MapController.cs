using System;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class MapController : MonoBehaviour
{
    //public float minScale = 0.5f;
    //public float maxScale = 2f;

    //public float minX = -5f;
    //public float maxX = 5f;

    //public float minY = -5f;
    //public float maxY = 5f;

    [SerializeField]
    float xPos;

    [SerializeField]
    float zPos;
    [SerializeField]
    [Range(2.5f, 5f)]
    float scale;

    [Header("Transforms")]
    [SerializeField]
    Transform mapObj;
    [SerializeField]
    Transform pivot;
    [SerializeField]
    Transform cameraPivot;

    [SerializeField]
    bool enableClipping;

    [SerializeField]
    Material[] shaders;


    [SerializeField]
    Slider zoomSlider, xslider, zslider, rotateSlider;

    [SerializeField]
    Transform[] countryArray;

    private void Start()
    {
        zoomSlider.onValueChanged.AddListener(delegate { ScaleAround(zoomSlider.value); });
        xslider.onValueChanged.AddListener(delegate { UpdateMapPos(new Vector3(xslider.value,0,zPos));});
        zslider.onValueChanged.AddListener(delegate { UpdateMapPos(new Vector3(xPos, 0, zslider.value));});
        rotateSlider.onValueChanged.AddListener(delegate { Rotate(rotateSlider.value); });
    }


    private void Update()
    {
        if (enableClipping)
        {
            foreach (var item in shaders)
            {
                item.SetFloat("_EnableClipping", 1);
            }
        }
        else
        {
            foreach (var item in shaders)
            {
                item.SetFloat("_EnableClipping", 0);
            }
        }

    }

    private void Rotate(float value)
    {
        var rotation = Quaternion.Euler(new Vector3(0, value, 0));

        cameraPivot.rotation = rotation;
    }

    private void UpdateMapPos(Vector3 newPos)
    {
        mapObj.transform.localPosition = newPos;
    }

    public void ScaleAround(float _newScale)
    {
        scale = _newScale;

        Vector3 newScale = new Vector3(_newScale, 1, _newScale);

        Vector3 A = mapObj.transform.localPosition;
        Vector3 B = pivot.position;

        // diff from object pivot to desired pivot/origin
        Vector3 C = A - B;

        float RS = newScale.x / mapObj.transform.localScale.x; // relative scale factor
        //Dont scale y dimension
        //newScale.y = 1f;

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        mapObj.transform.localScale = newScale;

        xPos = FP.x;
        zPos = FP.z;

        //xPos = Mathf.Clamp(xPos, minX, minY);

        UpdateMapPos(FP);
    }

    public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.transform.localPosition;
        Vector3 B = pivot;

        // diff from object pivot to desired pivot/origin
        Vector3 C = A - B; 

        float RS = newScale.x / target.transform.localScale.x; // relative scale factor
        //Dont scale y dimension
        newScale.y = 1f;
        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;

        xPos = FP.x;
        zPos = FP.z;

        //if(newScale.x <= 2.5f)
        //{

        //}

        //target.transform.localPosition = FP;
    }
}
