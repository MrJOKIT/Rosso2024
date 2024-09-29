using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Artifact",fileName = "ArtifactData",order = 3)]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    public Sprite artifactImage;
}
