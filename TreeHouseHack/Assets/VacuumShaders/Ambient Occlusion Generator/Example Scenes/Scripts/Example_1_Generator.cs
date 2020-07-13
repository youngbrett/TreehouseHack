using UnityEngine;
using System.Collections;

using VacuumShaders.AmbientOcclusionGenerator;


public class Example_1_Generator : MonoBehaviour 
{
    public AmbientOcclusion ambientOcclusion;
    public IndirectLighting indirectLighting;


    // Use this for initialization
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.colors = Generator.Generate(gameObject, ambientOcclusion, indirectLighting);

        GetComponent<MeshFilter>().mesh = mesh;
    }	
}
