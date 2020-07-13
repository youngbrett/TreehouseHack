using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using VacuumShaders.AmbientOcclusionGenerator;


public class Example_2_CombineMeshes : MonoBehaviour 
{
    public AmbientOcclusion ambientOcclusion;


    // Use this for initialization
    void Start()
    {
        Mesh mesh;

        //Combine
        COMBINE_INFO combineInfo = Generator.CombineMeshes(gameObject.transform, out mesh);

        if (combineInfo == COMBINE_INFO.OK)
        {
            //Destroy hierarchy
            var children = new List<GameObject>();
            foreach (Transform child in gameObject.transform)
                children.Add(child.gameObject);
            children.ForEach(child => DestroyImmediate(child));


            //Setup new MeshFilter and MeshRenderer
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            MeshRenderer meshrenderer = gameObject.AddComponent<MeshRenderer>();
            meshrenderer.sharedMaterial = new Material(Shader.Find("VacuumShaders/Vertex Color/Base"));


            //Generate Ambient Occlusion for combined mesh
            mesh.colors = Generator.Generate(gameObject, ambientOcclusion, null);
        }
        else
        {
            //We have problems
            Debug.LogWarning(combineInfo);
        }
    }	
}
