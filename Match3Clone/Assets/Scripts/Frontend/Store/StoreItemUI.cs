using System;
using System.Collections;
using System.Threading.Tasks;
using SaveData;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StoreItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image itemImage;

    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject buyPanel;

    private string itemDescription;
    private StoreItem storeItem;
    PlayerProfile playerProfile;
    private async void Start()
    {
        buyButton.onClick.AddListener(OnBuyButtonClicked);

        playerProfile = await LocalSaveManager.Instance.LoadDataAsync<PlayerProfile>("PlayerProfile");
        // InvokeRepeating(nameof(UpdateBuyButton), 0, 0.1f);
    }
    private void OnEnable() 
    {
        LoadImage(storeItem.Url);
    }

    private void OnBuyButtonClicked()
    {
        if(buyPanel != null)
        {
            var canvas = GameObject.Find("Canvas");

            var buyPanelUI = Instantiate(buyPanel, canvas.transform);
            buyPanelUI.SetActive(true);
            buyPanelUI.GetComponent<BuyPanelUI>().SetItemDetails(storeItem, itemImage);
        }
    }
    public void SetItem(StoreItem item)
    {
        storeItem = item;

        titleText.text = item.Title;
        itemDescription = item.Description;
    }

    private void LoadImage(string Url)
    {
        StartCoroutine(GetImage(Url));
    }

    private IEnumerator GetImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            itemImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
}
