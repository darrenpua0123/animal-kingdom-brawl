using Firebase.Auth;
using Firebase.Database;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    private string userID;
    private int playerCurrency;
    private List<string> unlockedAnimalHeroes = new List<string>();

    [Header("Shop UI")]
    public TMP_Text coinText;
    private readonly string x_icon = " � ";

    [Header("Firebase")]
    public FirebaseAuth auth;
    public DatabaseReference firebaseDBReference;

    private Piggion piggion;
    [Header("Piggion")]
    public TMP_Text piggionHeroName;
    public TMP_Text piggionHealth, piggionShield, piggionActionPoint, piggionHeroCost;
    public Button piggionBuyButton;

    private Catomic catomic;
    [Header("Catomic")]
    public TMP_Text catomicHeroName;
    public TMP_Text catomicHealth, catomicShield, catomicActionPoint, catomicHeroCost;
    public Button catomicBuyButton;

    private Pandragon pandragon;
    [Header("Pandragon")]
    public TMP_Text pandragonHeroName;
    public TMP_Text pandragonHealth, pandragonShield, pandragonActionPoint, pandragonHeroCost;
    public Button pandragonBuyButton;

    private Beedle beedle;
    [Header("Beedle")]
    public TMP_Text beedleHeroName;
    public TMP_Text beedleHealth, beedleShield, beedleActionPoint, beedleHeroCost;
    public Button beedleBuyButton;

    [Header("Purchase Panel")]
    public GameObject purchasePanel;
    public TMP_Text purchasePanelText;

    void Awake()
    {
        InitialiseFirebase();

        userID = auth.CurrentUser.UserId;

        piggion = new Piggion();
        catomic = new Catomic();
        pandragon = new Pandragon();
        beedle = new Beedle();

        StartCoroutine(GetUserData());
    }

    void Start()
    {

    }

    private void InitialiseFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        firebaseDBReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private IEnumerator GetUserData() 
    {
        var userTask = firebaseDBReference.Child("users").Child(userID).GetValueAsync();
        yield return new WaitUntil(predicate: () => userTask.IsCompleted);

        if (userTask.Exception != null)
        {
            Debug.LogWarning($"Failed to complete user task with error: {userTask.Exception} at {this.name}");
        }
        else
        {
            DataSnapshot userSnapshot = userTask.Result;

            playerCurrency = int.Parse(userSnapshot.Child("currency").Value.ToString());

            var playerAnimalHeroes = userSnapshot.Child("unlockedAnimalHeroes");
            foreach (var hero in playerAnimalHeroes.Children) 
            {
                unlockedAnimalHeroes.Add(hero.Value.ToString());
            }

            SetShopHeroUI();
        }
    }

    private void SetShopHeroUI() {
        #region Piggion
        piggionHeroName.text = Piggion.HERO_NAME.FirstCharacterToUpper();
        piggionHealth.text = x_icon + piggion.defaultHealth.ToString();
        piggionShield.text = x_icon + piggion.defaultShield.ToString();
        piggionActionPoint.text = x_icon + piggion.startingActionPoint.ToString();
        piggionHeroCost.text = GameData.PiggionShopCost.ToString();
        #endregion

        #region Catomic
        catomicHeroName.text = Catomic.HERO_NAME.FirstCharacterToUpper();
        catomicHealth.text = x_icon + catomic.defaultHealth.ToString();
        catomicShield.text = x_icon + catomic.defaultShield.ToString();
        catomicActionPoint.text = x_icon + catomic.startingActionPoint.ToString();
        catomicHeroCost.text = GameData.CatomicShopCost.ToString();
        #endregion

        #region Pandragon
        pandragonHeroName.text = Pandragon.HERO_NAME.FirstCharacterToUpper();
        pandragonHealth.text = x_icon + pandragon.defaultHealth.ToString();
        pandragonShield.text = x_icon + pandragon.defaultShield.ToString();
        pandragonActionPoint.text = x_icon + pandragon.startingActionPoint.ToString();
        pandragonHeroCost.text = GameData.PandragonShopCost.ToString();
        #endregion

        #region Beedle
        beedleHeroName.text = Beedle.HERO_NAME.FirstCharacterToUpper();
        beedleHealth.text = x_icon + beedle.defaultHealth.ToString();
        beedleShield.text = x_icon + beedle.defaultShield.ToString();
        beedleActionPoint.text = x_icon + beedle.startingActionPoint.ToString();
        beedleHeroCost.text = GameData.BeedleShopCost.ToString();
        #endregion

        UpdateShopUI();
    }

    private void UpdateShopUI() 
    {
        coinText.text = playerCurrency.ToString();

        foreach (var hero in unlockedAnimalHeroes) {
            if (hero.Equals(Piggion.HERO_NAME))
            {
                piggionBuyButton.interactable = false;
                piggionHeroCost.text = "Owned";
            }
            else if (hero.Equals(Catomic.HERO_NAME)) 
            {
                catomicBuyButton.interactable = false;
                catomicHeroCost.text = "Owned";
            }
            else if (hero.Equals(Pandragon.HERO_NAME))
            {
                pandragonBuyButton.interactable = false;
                pandragonHeroCost.text = "Owned";
            }
            else if (hero.Equals(Beedle.HERO_NAME))
            {
                beedleBuyButton.interactable = false;
                beedleHeroCost.text = "Owned";
            }
        }
    }

    public void PurchasePiggionButton()
    {
        int piggionCost = GameData.PiggionShopCost;

        if (playerCurrency >= piggionCost)
        {
            playerCurrency -= piggionCost; 
            unlockedAnimalHeroes.Add(Piggion.HERO_NAME);

            UpdateCurrencyAndUnlockedHeroesToDB(playerCurrency, unlockedAnimalHeroes);

            SetPurchasePanelText("sufficient");
        }
        else 
        {
            SetPurchasePanelText("insufficient");
        }

        UpdateShopUI();
        ShowPurchasePanel();
    }

    public void PurchaseCatomicButton() 
    {
        int catomicCost = GameData.CatomicShopCost;

        if (playerCurrency >= catomicCost)
        {
            playerCurrency -= catomicCost;
            unlockedAnimalHeroes.Add(Catomic.HERO_NAME);

            UpdateCurrencyAndUnlockedHeroesToDB(playerCurrency, unlockedAnimalHeroes);

            SetPurchasePanelText("sufficient");
        }
        else
        {
            SetPurchasePanelText("insufficient");
        }

        UpdateShopUI();
        ShowPurchasePanel();
    }

    public void PurchasePandragonButton() 
    {
        int pandragonCost = GameData.PandragonShopCost;

        if (playerCurrency >= pandragonCost)
        {
            playerCurrency -= pandragonCost;
            unlockedAnimalHeroes.Add(Pandragon.HERO_NAME);

            UpdateCurrencyAndUnlockedHeroesToDB(playerCurrency, unlockedAnimalHeroes);

            SetPurchasePanelText("sufficient");
        }
        else
        {
            SetPurchasePanelText("insufficient");
        }

        UpdateShopUI();
        ShowPurchasePanel();
    }

    public void PurchaseBeedleButton()
    {
        int beedleCost = GameData.BeedleShopCost;

        if (playerCurrency >= beedleCost)
        {
            playerCurrency -= beedleCost;
            unlockedAnimalHeroes.Add(Beedle.HERO_NAME);

            UpdateCurrencyAndUnlockedHeroesToDB(playerCurrency, unlockedAnimalHeroes);

            SetPurchasePanelText("sufficient");
        }
        else
        {
            SetPurchasePanelText("insufficient");
        }

        UpdateShopUI();
        ShowPurchasePanel();
    }

    private void UpdateCurrencyAndUnlockedHeroesToDB(int currency, List<string> unlockHeroes) 
    {
        firebaseDBReference.Child("users").Child(userID).Child("currency").SetValueAsync(currency);
        firebaseDBReference.Child("users").Child(userID).Child("unlockedAnimalHeroes").SetValueAsync(unlockHeroes);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ShowPurchasePanel() 
    {
        purchasePanel.SetActive(true);
    }

    private void SetPurchasePanelText(string condition) 
    {
        string purchaseSuccessul = "Purchase Succesful! <br><br> Thank you!";
        string purchaseUnsuccessful = "Purchase Unsuccessful! <br><br> Insufficient Fund!";
        string unknown = "UNKNOWN";

        switch (condition)
        {
            case "sufficient":
                purchasePanelText.text = purchaseSuccessul;
                break;

            case "insufficient":
                purchasePanelText.text = purchaseUnsuccessful;
                break;

            default:
                purchasePanelText.text = unknown;
                break;
        }
    }

    public void ClosePurchasePanel() 
    {
        purchasePanel.SetActive(false);
    }
}
