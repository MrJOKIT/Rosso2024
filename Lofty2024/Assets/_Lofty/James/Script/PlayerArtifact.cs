using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerArtifact : MonoBehaviour
{
    [Header("Artifact UI")] 
    public GameObject artifactInventory;
    public bool inventoryActive;

    [Space(10)] 
    [Header("Artifact Data")] 
    public int maxArtifact;
    public List<ArtifactData> artifactHaves;
    public List<ArtifactUI> artifactSlots;
    public ArtifactData testArtifactOne;
    public ArtifactData testArtifactTwo;
    public ArtifactData testArtifactThree;

    private void Awake()
    {
        maxArtifact = artifactSlots.Count;
    }

    private void Update()
    {
        //Test add Artifact
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddNewArtifact(testArtifactOne);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddNewArtifact(testArtifactTwo);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddNewArtifact(testArtifactThree);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            RemoveArtifact(testArtifactOne);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            RemoveArtifact(testArtifactTwo);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            RemoveArtifact(testArtifactThree);
        }
    }

    public void AddNewArtifact(ArtifactData newArtifact)
    {
        if (artifactHaves.Count == maxArtifact)
        {
            return;
        }

        if (artifactHaves.Contains(newArtifact)) 
        {
            return;
        }
        
        artifactHaves.Add(newArtifact);
        artifactSlots[artifactHaves.Count - 1].SetArtifactUI(newArtifact.artifactName,newArtifact.artifactImage);
        //Destroy(newArtifact.GameObject());
    }
    
    public void RemoveArtifact(ArtifactData removeArtifact)
    {
        if (!artifactHaves.Contains(removeArtifact))
        {
            return;
        }
        artifactSlots[artifactHaves.IndexOf(removeArtifact)].ClearArtifactSlot();
        artifactHaves.Remove(removeArtifact);
        SortingSlot();
    }

    private void SortingSlot()
    {
        foreach (ArtifactUI slots in artifactSlots)
        {
            slots.ClearArtifactSlot(); 
        }

        foreach (ArtifactData artifact in artifactHaves)
        {
            artifactSlots[artifactHaves.IndexOf(artifact)].SetArtifactUI(artifact.artifactName,artifact.artifactImage);
        }
    }

    public void InventoryAppear()
    {
        if (inventoryActive)
        {
            artifactInventory.SetActive(false);
            inventoryActive = false;
        }
        else
        {
            artifactInventory.SetActive(true);
            inventoryActive = true;
        }
    }
}
