using UnityEngine;

public class Card_ID_Layout : MonoBehaviour
{
    public string name_cer;
    public string city;
    public GameObject img_layout;

    public void show()
    {
        this.img_layout.SetActive(true);
    }
}
