using System.Collections;
using System.Collections.Generic;
using Carrot;
using UnityEngine;
using UnityEngine.UI;

public class Tool : MonoBehaviour
{
    [Header("Objs Main")]
    public App app;

    [Header("Tool Obj")]
    public Sprite sp_icon_city;
    private Carrot_Box_Item item_temp;
    private Carrot_Box box;
    private IList data_code_city;
    public void On_load()
    {
        var asset = Resources.Load("data_code") as TextAsset;
        this.data_code_city = (IList)Json.Deserialize(asset.text);
    }
    public void Show_box_select_city(Carrot_Box_Item item_sel)
    {
        app.carrot.play_sound_click();
        this.item_temp = item_sel;
  
        this.box = app.carrot.Create_Box();
        this.box.set_icon(app.icon_city);
        this.box.set_title("Change code ID city");

        for (int i = 0; i < data_code_city.Count; i++)
        {
            IDictionary data_city = (IDictionary)data_code_city[i];
            var s_code = data_city["code"].ToString();
            
            Carrot_Box_Item item_city = this.box.create_item("item_" + i);
            item_city.set_icon(this.sp_icon_city);
            item_city.set_title(data_city["name"].ToString());
            item_city.set_tip(data_city["code"].ToString());
            item_city.set_act(() => Sel_item_code_city(s_code));

            Carrot_Box_Btn_Item btn_replace = item_city.create_item();
            btn_replace.set_icon(app.icon_replace);
            btn_replace.set_color(app.carrot.color_highlight);
            btn_replace.set_act(() => Replace_item_code_city(s_code));

            Carrot_Box_Btn_Item btn_insert = item_city.create_item();
            btn_insert.set_icon(app.icon_insert);
            btn_insert.set_color(app.carrot.color_highlight);
            btn_insert.set_act(() => Sel_item_code_city(s_code));
        }
    }

    private void Sel_item_code_city(string s_code)
    {
        app.carrot.play_sound_click();
        string s_code_old = this.item_temp.get_val();
        this.item_temp.set_val(s_code+s_code_old);
        box?.close();
    }

    private void Replace_item_code_city(string s_code)
    {
        app.carrot.play_sound_click();
        string s_code_old = this.item_temp.get_val();
        if (s_code_old.Length > 3)
        {
            string s_3_char = s_code_old.Substring(0, 3);
            this.item_temp.set_val(s_code_old.Replace(s_3_char, s_code));
        }
        else
        {
            this.item_temp.set_val(s_code);
        }
            
        box?.close();
    }

    public void Show_box_select_city_for_card(int index_card_sel)
    {
        app.carrot.play_sound_click();
        this.box = app.carrot.Create_Box();
        this.box.set_icon(app.icon_city);
        this.box.set_title("Set the city's personal code for the card type");

        string code_city_sel = PlayerPrefs.GetString("city_for_card_" + index_card_sel,"");

        for (int i = 0; i < data_code_city.Count; i++)
        {
            IDictionary data_city = (IDictionary)data_code_city[i];
            var s_code = data_city["code"].ToString();
            string code_city= data_city["code"].ToString();

            
            Carrot_Box_Item item_city = this.box.create_item("item_" + i);
            item_city.set_icon(this.sp_icon_city);
            item_city.set_title(data_city["name"].ToString());
            item_city.set_tip(data_city["code"].ToString());
            item_city.set_act(() => Select_index_code_for_card(index_card_sel,s_code));

            if (code_city == code_city_sel)
            {
                Carrot_Box_Btn_Item btn_sel = item_city.create_item();
                btn_sel.set_icon(app.carrot.icon_carrot_done);
                btn_sel.set_color(app.carrot.color_highlight);
                Destroy(btn_sel.GetComponent<Button>());
            }
        }
    }

    public void Select_index_code_for_card(int index_card,string s_code)
    {
        PlayerPrefs.SetString("city_for_card_" + index_card, s_code);
        box?.close();
    }

    public void change_txt_for_id_city(Carrot_Box_Item item_info)
    {
        string code_city_sel = PlayerPrefs.GetString("city_for_card_" + this.app.get_index_card(), "");
        app.carrot.play_sound_click();
        item_info.set_val(code_city_sel);
    }
}
