using UnityEngine;
using System.Collections;

using VacuumShaders.AmbientOcclusionGenerator;


public class Example_3_CombineSubMeshes : MonoBehaviour 
{
    public AmbientOcclusion ambientOcclusion;


    // Use this for initialization
    void Start()
    {
        Mesh mesh;

        //Combine sub-meshes
        Generator.CombineSubMeshes(GetComponent<MeshFilter>().sharedMesh, out mesh);

        if (mesh != null)
        {
            //Generating ambient occlusion
            mesh.colors = Generator.Generate(gameObject, ambientOcclusion, null);


            //Assigning new mesh
            GetComponent<MeshFilter>().mesh = mesh;


            //Removing all sub-materials and assisngin one vertex color material
            Material vertexColor = new Material(Shader.Find("VacuumShaders/Vertex Color/Base"));
            GetComponent<MeshRenderer>().materials = new Material[]{ vertexColor };
        }
        else
        {
            Debug.LogWarning("GetComponent<MeshFilter>().sharedMesh == null");
        }
    }	
}
