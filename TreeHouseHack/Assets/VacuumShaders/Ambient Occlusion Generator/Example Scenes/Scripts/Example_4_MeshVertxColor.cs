using UnityEngine;
using System.Collections;

using VacuumShaders.AmbientOcclusionGenerator;


public class Example_4_MeshVertxColor : MonoBehaviour 
{
    public AmbientOcclusion ambientOcclusion;

    public bool meshVertexColor;

    // Use this for initialization
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Color[] generatedColors = Generator.Generate(gameObject, ambientOcclusion, null);


        //Blend with original mesh vertex colors
        if (meshVertexColor)
        {
            Color[] mColors = mesh.colors;
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                mColors[i] *= generatedColors[i];
            }
            mesh.colors = mColors;
        }
        else
            mesh.colors = generatedColors;


        GetComponent<MeshFilter>().mesh = mesh;
    }	
}
