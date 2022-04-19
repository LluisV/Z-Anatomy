using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnRecommendationClick : MonoBehaviour
{
    SearchEngine searchEngine;
    Button btn;
    [HideInInspector]
    public string realName;
    // Start is called before the first frame update
    void Start()
    {
        searchEngine = FindObjectOfType<SearchEngine>();
        btn = GetComponent<Button>();

        btn.onClick.AddListener(delegate { OnClick(); });
    }

    private void OnClick()
    {
        searchEngine.RecommendationClicked(btn.gameObject);
    }

}
