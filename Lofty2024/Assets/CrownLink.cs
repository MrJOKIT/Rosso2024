using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CrownLink : MonoBehaviour
{
    public PlayerArtifact playerArtifact;
    public Animator crownAnimator;
    public Image iconImage;
    public bool crownComplete;
    public bool crownTwoComplete;
    public Sprite swordImage;
    public Sprite bladeImage;
    public Sprite shootImage;
    [Header("Card Image")]
    public Image card1;
    public Image card2;
    public Image card3;
    public Image card4;
    public Image card5;

    public GameObject swordKnightCharacter;
    public GameObject bladeMasterCharacter;
    public GameObject shootCasterCharacter;
    public TextRevealer textRevealer;

    private void Update()
    {
        if (playerArtifact.swordKnightPassiveOne == false && playerArtifact.bladeMasterPassiveOne == false && playerArtifact.shootCasterPassiveOne == false)
        {
            return;
        }

        if (crownTwoComplete)
        {
            return;
        }
        if (playerArtifact.swordKnightPassiveTwo)
        {
            crownAnimator.SetBool("Yellow",true);
            crownAnimator.SetBool("Red",false);
            crownAnimator.SetBool("Blue",false);
            iconImage.sprite = swordImage;
            card4.gameObject.SetActive(true);
            card5.gameObject.SetActive(true);
            card1.sprite = playerArtifact.swordKnightType[0].artifactImage;
            card2.sprite = playerArtifact.swordKnightType[1].artifactImage;
            card3.sprite = playerArtifact.swordKnightType[2].artifactImage;
            card4.sprite = playerArtifact.swordKnightType[3].artifactImage;
            card5.sprite = playerArtifact.swordKnightType[4].artifactImage;
            
            crownTwoComplete = true;
        }
        if (playerArtifact.bladeMasterPassiveTwo)
        {
            crownAnimator.SetBool("Red",true);
            crownAnimator.SetBool("Yellow",false);
            crownAnimator.SetBool("Blue",false);
            iconImage.sprite = bladeImage;
            card4.gameObject.SetActive(true);
            card5.gameObject.SetActive(true);
            card1.sprite = playerArtifact.bladeMasterType[0].artifactImage;
            card2.sprite = playerArtifact.bladeMasterType[1].artifactImage;
            card3.sprite = playerArtifact.bladeMasterType[2].artifactImage;
            card4.sprite = playerArtifact.bladeMasterType[3].artifactImage;
            card5.sprite = playerArtifact.bladeMasterType[4].artifactImage;
            
            crownTwoComplete = true;
        }
        if (playerArtifact.shootCasterPassiveTwo)
        {
            crownAnimator.SetBool("Blue",true);
            crownAnimator.SetBool("Red",false);
            crownAnimator.SetBool("Yellow",false);
            iconImage.sprite = shootImage;
            card4.gameObject.SetActive(true);
            card5.gameObject.SetActive(true);
            card1.sprite = playerArtifact.shootCasterType[0].artifactImage;
            card2.sprite = playerArtifact.shootCasterType[1].artifactImage;
            card3.sprite = playerArtifact.shootCasterType[2].artifactImage;
            card4.sprite = playerArtifact.shootCasterType[3].artifactImage;
            card5.sprite = playerArtifact.shootCasterType[4].artifactImage;
            
            crownTwoComplete = true;
        } 
        if (crownComplete)
        {
            return;
        }
        if (playerArtifact.swordKnightPassiveOne)
        {
            swordKnightCharacter.SetActive(true);
            bladeMasterCharacter.SetActive(false);
            shootCasterCharacter.SetActive(false);
            crownAnimator.SetBool("Red",true);
            crownAnimator.SetBool("Yellow",false);
            crownAnimator.SetBool("Blue",false);
            iconImage.sprite = swordImage;
            card4.gameObject.SetActive(false);
            card5.gameObject.SetActive(false);
            card1.sprite = playerArtifact.swordKnightType[0].artifactImage;
            card2.sprite = playerArtifact.swordKnightType[1].artifactImage;
            card3.sprite = playerArtifact.swordKnightType[2].artifactImage;
            
            crownComplete = true;
        }
        if (playerArtifact.bladeMasterPassiveOne)
        {
            bladeMasterCharacter.SetActive(true);
            swordKnightCharacter.SetActive(false);
            shootCasterCharacter.SetActive(false);
            crownAnimator.SetBool("Red",true);
            crownAnimator.SetBool("Yellow",false);
            crownAnimator.SetBool("Blue",false);
            iconImage.sprite = bladeImage;
            card4.gameObject.SetActive(false);
            card5.gameObject.SetActive(false);
            card1.sprite = playerArtifact.bladeMasterType[0].artifactImage;
            card2.sprite = playerArtifact.bladeMasterType[1].artifactImage;
            card3.sprite = playerArtifact.bladeMasterType[2].artifactImage;
            
            crownComplete = true;
        }
        if (playerArtifact.shootCasterPassiveOne)
        {
            shootCasterCharacter.SetActive(true);
            bladeMasterCharacter.SetActive(false);
            swordKnightCharacter.SetActive(false);
            crownAnimator.SetBool("Blue",true);
            crownAnimator.SetBool("Red",false);
            crownAnimator.SetBool("Yellow",false);
            iconImage.sprite = shootImage;
            card4.gameObject.SetActive(false);
            card5.gameObject.SetActive(false);
            card1.sprite = playerArtifact.shootCasterType[0].artifactImage;
            card2.sprite = playerArtifact.shootCasterType[1].artifactImage;
            card3.sprite = playerArtifact.shootCasterType[2].artifactImage;
            
            crownComplete = true;
        }
    }

    public void RevealTrack()
    {
        textRevealer.Reveal();
    }
    public void ShowCrown()
    {
        if (playerArtifact.swordKnightPassiveOne)
        {
            crownAnimator.SetBool("Red",true);
            crownAnimator.SetBool("Yellow",false);
            crownAnimator.SetBool("Blue",false);
        }
        if (playerArtifact.bladeMasterPassiveOne)
        {
            crownAnimator.SetBool("Red",true);
            crownAnimator.SetBool("Yellow",false);
            crownAnimator.SetBool("Blue",false);
        }
        if (playerArtifact.shootCasterPassiveOne)
        {
            crownAnimator.SetBool("Blue",true);
            crownAnimator.SetBool("Red",false);
            crownAnimator.SetBool("Yellow",false);
        }
    }
}
