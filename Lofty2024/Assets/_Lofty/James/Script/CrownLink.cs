using System;
using UnityEngine;
using UnityEngine.UI;

public class CrownLink : MonoBehaviour
{
    public PlayerArtifact playerArtifact;
    public Animator crownAnimator;
    public Image iconImage;
    public bool crownComplete;
    public bool crownTwoComplete;
    public Sprite swordImage, bladeImage, shootImage;
    
    [Header("Card Images")]
    public Image[] cards;
    
    [Header("Character Models")]
    public GameObject swordKnightCharacter;
    public GameObject bladeMasterCharacter;
    public GameObject shootCasterCharacter;
    
    public TextRevealer textRevealer;
    
    private void Update()
    {
        if (!HasAnyPassive()) return;
        if (!crownTwoComplete) CheckCrownTwoCompletion();
        if (!crownComplete) CheckCrownOneCompletion();
    }

    private bool HasAnyPassive() => 
        playerArtifact.swordKnightPassiveOne || playerArtifact.bladeMasterPassiveOne || playerArtifact.shootCasterPassiveOne;

    private void CheckCrownTwoCompletion()
    {
        if (playerArtifact.swordKnightPassiveTwo) ActivateCrown("Yellow", swordImage, playerArtifact.swordKnightType.ToArray());
        else if (playerArtifact.bladeMasterPassiveTwo) ActivateCrown("Red", bladeImage, playerArtifact.bladeMasterType.ToArray());
        else if (playerArtifact.shootCasterPassiveTwo) ActivateCrown("Blue", shootImage, playerArtifact.shootCasterType.ToArray());
        crownTwoComplete = true;
    }

    private void CheckCrownOneCompletion()
    {
        if (playerArtifact.swordKnightPassiveOne) ActivateCharacter(swordKnightCharacter, "Red", swordImage, playerArtifact.swordKnightType.ToArray());
        else if (playerArtifact.bladeMasterPassiveOne) ActivateCharacter(bladeMasterCharacter, "Red", bladeImage, playerArtifact.bladeMasterType.ToArray());
        else if (playerArtifact.shootCasterPassiveOne) ActivateCharacter(shootCasterCharacter, "Blue", shootImage, playerArtifact.shootCasterType.ToArray());
        crownComplete = true;
    }

    private void ActivateCrown(string color, Sprite icon, ArtifactData[] artifactTypes)
    {
        SetCrownColor(color);
        iconImage.sprite = icon;
        cards[3].gameObject.SetActive(true);
        cards[4].gameObject.SetActive(true);
        AssignCardSprites(artifactTypes);
    }

    private void ActivateCharacter(GameObject character, string color, Sprite icon, ArtifactData[] artifactTypes)
    {
        swordKnightCharacter.SetActive(character == swordKnightCharacter);
        bladeMasterCharacter.SetActive(character == bladeMasterCharacter);
        shootCasterCharacter.SetActive(character == shootCasterCharacter);
        
        SetCrownColor(color);
        iconImage.sprite = icon;
        cards[3].gameObject.SetActive(false);
        cards[4].gameObject.SetActive(false);
        AssignCardSprites(artifactTypes, true);
    }

    private void SetCrownColor(string color)
    {
        crownAnimator.SetBool("Red", color == "Red");
        crownAnimator.SetBool("Yellow", color == "Yellow");
        crownAnimator.SetBool("Blue", color == "Blue");
    }

    private void AssignCardSprites(ArtifactData[] artifactTypes, bool isCrownOne = false)
    {
        int cardCount = isCrownOne ? 3 : 5;
        for (int i = 0; i < cardCount; i++)
        {
            cards[i].sprite = artifactTypes[i].artifactImage;
        }
    }

    public void RevealTrack() => textRevealer.Reveal();
    
    public void ShowCrown()
    {
        if (playerArtifact.swordKnightPassiveOne) SetCrownColor("Red");
        else if (playerArtifact.bladeMasterPassiveOne) SetCrownColor("Red");
        else if (playerArtifact.shootCasterPassiveOne) SetCrownColor("Blue");
    }
}
