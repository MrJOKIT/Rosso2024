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
    
    [Header("Sword Image")] 
    public Sprite cardSword1;
    public Sprite cardSword2;
    public Sprite cardSword3;
    public Sprite cardSword4;
    public Sprite cardSword5;
    
    [Header("Blade Image")] 
    public Sprite cardBlade1;
    public Sprite cardBlade2;
    public Sprite cardBlade3;
    public Sprite cardBlade4;
    public Sprite cardBlade5;
    
    [Header("Shoot Image")] 
    public Sprite cardShoot1;
    public Sprite cardShoot2;
    public Sprite cardShoot3;
    public Sprite cardShoot4;
    public Sprite cardShoot5;

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
}
