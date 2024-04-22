using System.Collections.Generic;
using Carrot;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    [Header("Obj Main")]
    public bool is_app_sell;
    public Carrot.Carrot carrot;
    public Tool tool;
    public float steep_zoom = 0.5f;

    [Header("Obj App")]
    public Sprite[] list_sp_bk;
    public IList<Sprite> list_bk_img;
    public Image image_btn_visible;
    public GameObject obj_btn_setting;
    public GameObject obj_menu_bottom;
    public GameObject obj_menu_right;
    public GameObject obj_menu_left;
    public Transform tr_panel_card;

    [Header("Cards")]
    public GameObject[] cards_prefab;

    [Header("Icons")]
    public Sprite icon_camera;
    public Sprite icon_cardID;
    public Sprite icon_uppercase;
    public Sprite icon_qr;
    public Sprite icon_open_file;
    public Sprite icon_city;
    public Sprite icon_replace;
    public Sprite icon_insert;
    public Sprite icon_export;
    public Sprite icon_import;

    [Header("Obj Info")]
    public Transform arean_all_card;
    public Image img_bk_card;

    private Carrot_Box box;
    private Carrot_Box box_layout;
    private Text txt_infor_edit_temp = null;
    private Carrot_Window_Input box_input;
    private Card_ID card_id_cur = null;
    private int index_style_select = 0;
    private int index_bk_select = 0;

    void Start()
    {
        QualitySettings.antiAliasing = 0;
        this.carrot.Load_Carrot(this.Check_exit_app);
        if (this.is_app_sell)
        {
            this.obj_btn_setting.SetActive(false);
            this.carrot.ads.set_status_ads(false);
        }
        else
            this.obj_btn_setting.SetActive(true);

        this.image_btn_visible.sprite = this.carrot.icon_carrot_visible_off;

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"), new FileBrowser.Filter("Pain", ".bmp", ".tiff", ".tga"));
        FileBrowser.SetDefaultFilter(".jpg");

        if (PlayerPrefs.GetInt("index_style_select", -1) != -1) this.index_style_select = PlayerPrefs.GetInt("index_style_select", 0);
        if (PlayerPrefs.GetInt("index_bk_select", -1) != -1) this.index_bk_select = PlayerPrefs.GetInt("index_bk_select", 0);

        this.Load_style_card(this.index_style_select);


        this.list_bk_img = new List<Sprite>();
        for (int i = 0; i < this.list_sp_bk.Length; i++) this.list_bk_img.Add(this.list_sp_bk[i]);
        this.Load_bk_img(this.index_bk_select);
        this.tool.On_load();
    }

    private void Check_exit_app()
    {
        if (this.is_app_sell)
        {
            this.carrot.app_exit();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void Btn_edit_info()
    {
        this.card_id_cur.Show_edit_all_item();
    }

    public void Btn_export()
    {
        this.Btn_screenshots();
    }

    public void Btn_change_bk()
    {
        this.box = this.carrot.show_grid();
        this.box.set_icon(carrot.icon_carrot_avatar);
        this.box.set_title("Change Background");

        Carrot_Box_Btn_Item btn_camera_photo = this.box.create_btn_menu_header(this.icon_camera);
        btn_camera_photo.set_act(Act_camera_for_bk);

        Carrot_Box_Btn_Item btn_add_photo = this.box.create_btn_menu_header(this.carrot.icon_carrot_add);
        btn_add_photo.set_act(Act_add_file_photo_for_bk);

        for (int i = 0; i < this.list_bk_img.Count; i++)
        {
            var index = i;
            this.Add_item_bk_for_list(this.list_bk_img[i], index);
        }
    }

    private void Add_item_bk_for_list(Sprite bk, int index)
    {
        Carrot_Box_Item item_bk = this.box.create_item("item_bk_" + index);
        item_bk.set_icon_white(bk);
        item_bk.set_act(() => this.Act_select_bk(index));
    }

    private void Act_add_file_photo_for_bk()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"), new FileBrowser.Filter("Pain", ".bmp", ".tiff", ".tga"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.ShowLoadDialog(Act_done_file_select_bk, null, FileBrowser.PickMode.Files);
    }

    private void Act_done_file_select_bk(string[] path)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(path[0]);

        Texture2D texture = new(2, 2);
        if (ImageConversion.LoadImage(texture, fileData))
        {
            Sprite sp_new = carrot.get_tool().Texture2DtoSprite(texture);
            this.Add_item_bk_for_list(sp_new, this.list_sp_bk.Length);
            this.list_bk_img.Add(sp_new);
        }
    }

    private void Act_camera_for_bk()
    {
        this.carrot.camera_pro.Show_camera(Act_camera_for_bk_done);
    }

    private void Act_camera_for_bk_done(Texture2D tex)
    {
        carrot.play_sound_click();
        this.img_bk_card.sprite = this.carrot.get_tool().Texture2DtoSprite(tex);
        box?.close();
    }

    private void Act_select_bk(int index)
    {
        PlayerPrefs.SetInt("index_bk_select", index);
        carrot.play_sound_click();
        if (this.box != null) this.box.close();
        this.Load_bk_img(index);
    }

    private void Load_bk_img(int index)
    {
        if (index >= this.list_sp_bk.Length)
        {
            this.img_bk_card.sprite = this.list_bk_img[0];
        }
        else
        {
            if (this.list_bk_img[index] != null)
                this.img_bk_card.sprite = this.list_bk_img[index];
            else
                this.img_bk_card.sprite = this.list_bk_img[0];
        }
    }

    public Texture2D Change_bk_color(Texture2D originalTexture)
    {
        Texture2D newTexture = new(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
        Color32 color_white = new Color(1.000f, 1.000f, 1.000f, 1.000f);
        for (int y = 0; y < originalTexture.height; y++)
        {
            for (int x = 0; x < originalTexture.width; x++)
            {
                Color pixelColor = originalTexture.GetPixel(x, y);
                if (pixelColor == color_white)
                {
                    newTexture.SetPixel(x, y, Color.clear);
                }
                else
                {
                    newTexture.SetPixel(x, y, pixelColor);
                }
            }
        }

        newTexture.Apply();
        return newTexture;
    }

    public void Btn_visible()
    {
        carrot.play_sound_click();
        if (this.card_id_cur.panel_info.activeInHierarchy)
        {
            this.card_id_cur.panel_info.SetActive(false);
            this.image_btn_visible.sprite = this.carrot.icon_carrot_visible_off;
        }
        else
        {
            this.card_id_cur.panel_info.SetActive(true);
            this.image_btn_visible.sprite = this.carrot.icon_carrot_visible_on;
        }
    }

    public void Btn_screenshots()
    {
        this.obj_menu_bottom.SetActive(false);
        this.obj_menu_right.SetActive(false);
        this.obj_menu_left.SetActive(false);
        carrot.play_sound_click();

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"), new FileBrowser.Filter("Pain", ".bmp", ".tiff", ".tga"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.ShowSaveDialog(Act_save_screenshots_done, Act_save_screenshots_cancel, FileBrowser.PickMode.Files);
    }

    private void Act_save_screenshots_cancel()
    {
        this.obj_menu_bottom.SetActive(true);
        this.obj_menu_right.SetActive(true);
        this.obj_menu_left.SetActive(true);
    }

    private void Act_save_screenshots_done(string[] paths)
    {
        int width = Screen.width;
        int height = Screen.height;

        RenderTexture rt = new(width, height, 24);
        Camera.main.targetTexture = rt;
        Texture2D screenShot = new(width, height, TextureFormat.RGB24, false);
        Camera.main.Render();

        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();

        this.carrot.Show_msg("Save image", "Save Image success!\nAt:" + paths[0], Msg_Icon.Success);

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"), new FileBrowser.Filter("Pain", ".bmp", ".tiff", ".tga"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowserHelpers.WriteBytesToFile(paths[0], bytes);

        this.obj_menu_bottom.SetActive(true);
        this.obj_menu_right.SetActive(true);
        this.obj_menu_left.SetActive(true);
    }

    public void Btn_setting()
    {
        carrot.Create_Setting();
    }

    public void Btn_zoom_out()
    {
        this.tr_panel_card.transform.localScale = new Vector3(this.tr_panel_card.transform.localScale.x - this.steep_zoom, this.tr_panel_card.transform.localScale.y - this.steep_zoom, 1f);
    }

    public void Btn_zoom_in()
    {
        this.tr_panel_card.transform.localScale = new Vector3(this.tr_panel_card.transform.localScale.x + this.steep_zoom, this.tr_panel_card.transform.localScale.y + this.steep_zoom, 1f);
    }

    public void Btn_zoom_reset()
    {
        this.tr_panel_card.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void Show_edit_info_item(Text txt)
    {
        carrot.play_sound_click();
        this.txt_infor_edit_temp = txt;
        this.box_input = this.carrot.show_input("Edit info Item", "Edit info Item", txt.text);
        this.box_input.set_act_done(Act_done_edit_info_item);
    }

    private void Act_done_edit_info_item(string val)
    {
        carrot.play_sound_click();
        this.txt_infor_edit_temp.text = val;
        if (this.box_input != null) this.box_input.close();
    }

    public void Btn_list_style_card_id()
    {
        carrot.play_sound_click();
        this.box = carrot.Create_Box();
        this.box.set_icon(this.icon_cardID);
        this.box.set_title("Select style ID card");

        for (int i = 0; i < this.cards_prefab.Length; i++)
        {
            Card_ID id_card = this.cards_prefab[i].GetComponent<Card_ID>();
            var index = i;
            Carrot_Box_Item item_card = this.box.create_item("card_" + i);
            item_card.set_icon_white(id_card.icon);
            item_card.set_title(id_card.s_name);
            item_card.set_tip(id_card.s_tip);
            item_card.set_act(() => this.Select_style_card(index));

            if (i == this.index_style_select)
            {
                Carrot_Box_Btn_Item btn_search = item_card.create_item();
                btn_search.set_icon(this.carrot.icon_carrot_done);
                btn_search.set_color(this.carrot.color_highlight);
            }

            if (id_card.layout.Length > 0)
            {
                Carrot_Box_Btn_Item btn_list = item_card.create_item();
                btn_list.set_icon(this.carrot.icon_carrot_all_category);
                btn_list.set_color(this.carrot.color_highlight);
                Destroy(btn_list.GetComponent<Button>());
            }

            Carrot_Box_Btn_Item btn_city = item_card.create_item();
            btn_city.set_icon(this.icon_city);
            btn_city.set_color(this.carrot.color_highlight);
            btn_city.set_act(() => this.tool.Show_box_select_city_for_card(index));
        }
    }

    private void Select_style_card(int index)
    {
        Card_ID id_card = this.cards_prefab[index].GetComponent<Card_ID>();
        if (id_card.layout.Length > 0)
        {
            this.box_layout = carrot.Create_Box();
            this.box_layout.set_title("Select Layout");
            this.box_layout.set_icon(this.icon_city);

            var index_card = index;
            for (int i = 0; i < id_card.layout.Length; i++)
            {
                var index_layout = i;
                Carrot_Box_Item item_layout = this.box_layout.create_item();
                item_layout.set_icon(this.icon_cardID);
                item_layout.set_title(id_card.layout[i].city);
                item_layout.set_tip(id_card.layout[i].name_cer);
                item_layout.set_act(() => Act_sel_layout(index_card, index_layout));
            }
        }
        else
        {
            this.index_style_select = index;
            carrot.play_sound_click();
            this.Load_style_card(index);
        }
    }

    private void Act_sel_layout(int index_card, int index_layout)
    {
        this.index_style_select = index_card;
        PlayerPrefs.SetInt("index_style_select", index_card);
        carrot.clear_contain(this.arean_all_card);
        GameObject obj_card = Instantiate(this.cards_prefab[index_card]);
        obj_card.transform.SetParent(this.arean_all_card);
        obj_card.transform.localPosition = Vector3.zero;
        obj_card.transform.localScale = new Vector3(this.cards_prefab[index_card].transform.localScale.x, this.cards_prefab[index_card].transform.localScale.y, 1f);
        this.card_id_cur = obj_card.GetComponent<Card_ID>();
        this.card_id_cur.On_load(this);
        for (int i = 0; i < this.card_id_cur.layout.Length; i++)
        {
            this.card_id_cur.layout[i].img_layout.SetActive(false);
        }
        this.card_id_cur.layout[index_layout].show();
        box?.close();
        box_layout?.close();
    }

    private void Load_style_card(int index)
    {
        PlayerPrefs.SetInt("index_style_select", index);
        this.index_style_select = index;
        carrot.clear_contain(this.arean_all_card);
        GameObject obj_card = Instantiate(this.cards_prefab[index]);
        obj_card.transform.SetParent(this.arean_all_card);
        obj_card.transform.localPosition = Vector3.zero;
        obj_card.transform.localScale = new Vector3(this.cards_prefab[index].transform.localScale.x, this.cards_prefab[index].transform.localScale.y, 1f);
        this.card_id_cur = obj_card.GetComponent<Card_ID>();
        this.card_id_cur.On_load(this);
        box?.close();
    }

    public int get_index_card()
    {
        return index_style_select;
    }

    public void Btn_bk_zoom_out()
    {
        this.img_bk_card.transform.localScale = new Vector3(this.img_bk_card.transform.localScale.x - this.steep_zoom, this.img_bk_card.transform.localScale.y - this.steep_zoom, 1f);
    }

    public void Btn_bk_zoom_in()
    {
        this.img_bk_card.transform.localScale = new Vector3(this.img_bk_card.transform.localScale.x + this.steep_zoom, this.img_bk_card.transform.localScale.y + this.steep_zoom, 1f);
    }

    public void Btn_bk_zoom_reset()
    {
        this.img_bk_card.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
