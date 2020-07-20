#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public class WizardHighresMaterial : ScriptableWizard
{
	
	public bool LeafMaterial = false;

	[MenuItem ("Assets/Create Highres Optimized Tree Material")]
	static void CreateWizard ()
	{
		ScriptableWizard.DisplayWizard<WizardHighresMaterial>("Highres Optimized Tree Material", "Create");
		// Clear progress bar
		EditorUtility.ClearProgressBar();
	}
	
	
	void OnWizardCreate ()
	{
		Material newMat;
		if (LeafMaterial) {
			newMat = new Material(Shader.Find("Hidden/Nature/Tree Creator Leaves Optimized"));
		}
		else {
			newMat = new Material(Shader.Find("Hidden/Nature/Tree Creator Bark Optimized"));
		}
		// Save new Material
		string directory = Application.dataPath;
		string fileName = "";
		string filePath = EditorUtility.SaveFilePanel("Save new Material", directory, fileName, "mat");
		if (filePath != "") {
			filePath = filePath.Substring(Application.dataPath.Length-6);
			AssetDatabase.CreateAsset(newMat, filePath);
			AssetDatabase.Refresh();
		}	
	}
	
	
	void OnWizardUpdate ()
	{
		helpString = "\nCreate a highres tree creator material\n";
	} 
	
		/*
	 * When the user pressed the "Apply" button OnWizardOtherButton is called.
	 */
	void OnWizardOtherButton ()
	{
		//
	}
	
}

#endif
