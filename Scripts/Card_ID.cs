
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Carrot;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Card_Item_Type {card_txt,card_img,card_qr,card_sex,card_text_space}
public class Card_ID : MonoBehaviour
{
    [Header("Obj Info")]
    public Sprite icon;
    public string s_name;
    public string s_tip;
    public string s_keyword_space = "";
    public int max_length_keyword_space = 63;
    public Card_ID_Layout[] layout;

    public int[] qr_index;

    [Header("obj Main")]
    private App app;

    [Header("Obj Card")]
    public GameObject panel_info;
    public GameObject[] obj_item;
    public Card_Item_Type[] obj_types;
    public string[] s_name_item;
    public string[] s_tip_item;
    public Sprite[] sp_icon_item;

    private Carrot_Window_Input box_input;
    private TextMeshProUGUI txt_infor_edit_temp = null;
    private Carrot_Box box = null;
    private Image img_infor_edit_temp = null;
    private Texture2D texture_avatar_temp = null;

    private IList<Carrot_Box_Item> list_item_edit_info_box;

    private Carrot_Box_Item item_temp = null;
    private Image img_qr_temp = null;
    private string s_data_qr_code = "";

    public void On_load(App app)
    {
        this.app= app;
        for(int i = 0; i < obj_item.Length; i++)
        {
            var index = i;
            if (!obj_item[i].GetComponent<Button>())
            {
                Button btn_edit = obj_item[i].AddComponent<Button>();
                btn_edit.onClick.AddListener(() => this.Show_edit_item(index));
            }
            else
            {
                Button btn_edit = obj_item[i].GetComponent<Button>();
                btn_edit.onClick.AddListener(() => this.Show_edit_item(index));
            }

            if (obj_types[i] == Card_Item_Type.card_text_space)
            {
                obj_item[i].GetComponent<TextMeshProUGUI>().paragraphSpacing += this.s_keyword_space.Length;
            }
        }
    }

    private string RemoveVietnameseCharacters(string input)
    {
        input = input.Replace("Á", "A");
        input = input.Replace("À", "A");
        input = input.Replace("Ả", "A");
        input = input.Replace("Ã", "A");
        input = input.Replace("Ạ", "A");
        input = input.Replace("Ă", "A");
        input = input.Replace("Ắ", "A");
        input = input.Replace("Ằ", "A");
        input = input.Replace("Ẳ", "A");
        input = input.Replace("Ẵ", "A");
        input = input.Replace("Ặ", "A");
        input = input.Replace("Â", "A");
        input = input.Replace("Ấ", "A");
        input = input.Replace("Ầ", "A");
        input = input.Replace("Ẩ", "A");
        input = input.Replace("Ẫ", "A");
        input = input.Replace("Ậ", "A");

        input = input.Replace("Đ", "D");

        input = input.Replace("É", "E");
        input = input.Replace("È", "E");
        input = input.Replace("Ẻ", "E");
        input = input.Replace("Ẽ", "E");
        input = input.Replace("Ẹ", "E");
        input = input.Replace("Ê", "E");
        input = input.Replace("Ế", "E");
        input = input.Replace("Ề", "E");
        input = input.Replace("Ể", "E");
        input = input.Replace("Ễ", "E");
        input = input.Replace("Ệ", "E");

        input = input.Replace("Í", "I");
        input = input.Replace("Ì", "I");
        input = input.Replace("Ỉ", "I");
        input = input.Replace("Ĩ", "I");
        input = input.Replace("Ị", "I");

        input = input.Replace("Ó", "O");
        input = input.Replace("Ò", "O");
        input = input.Replace("Ỏ", "O");
        input = input.Replace("Õ", "O");
        input = input.Replace("Ọ", "O");
        input = input.Replace("Ô", "O");
        input = input.Replace("Ố", "O");
        input = input.Replace("Ồ", "O");
        input = input.Replace("Ổ", "O");
        input = input.Replace("Ỗ", "O");
        input = input.Replace("Ộ", "O");
        input = input.Replace("Ơ", "O");
        input = input.Replace("Ớ", "O");
        input = input.Replace("Ờ", "O");
        input = input.Replace("Ở", "O");
        input = input.Replace("Ỡ", "O");
        input = input.Replace("Ợ", "O");

        input = input.Replace("Ú", "U");
        input = input.Replace("Ù", "U");
        input = input.Replace("Ủ", "U");
        input = input.Replace("Ũ", "U");
        input = input.Replace("Ụ", "U");
        input = input.Replace("Ư", "U");
        input = input.Replace("Ứ", "U");
        input = input.Replace("Ừ", "U");
        input = input.Replace("Ử", "U");
        input = input.Replace("Ữ", "U");
        input = input.Replace("Ự", "U");

        input = input.Replace("Ý", "Y");
        input = input.Replace("Ỳ", "Y");
        input = input.Replace("Ỷ", "Y");
        input = input.Replace("Ỹ", "Y");
        input = input.Replace("Ỵ", "Y");

        input = Regex.Replace(input, @"[ĂẮẰẲẴẶÂẤẦẨẪẬÁÀẢÃẠĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ]", "");

        return input;
    }

    public void Show_edit_all_item()
    {
        this.app.carrot.play_sound_click();
        this.box = this.app.carrot.Create_Box();
        this.box.set_title("Edit info");
        this.box.set_icon(this.app.carrot.user.icon_user_edit);

        Carrot_Box_Btn_Item btn_export=this.box.create_btn_menu_header(this.app.icon_export, true);
        btn_export.set_act(() => this.Act_export_data_json());

        Carrot_Box_Btn_Item btn_import = this.box.create_btn_menu_header(this.app.icon_import, true);
        btn_import.set_act(()=>this.Act_import_data_json());

        this.list_item_edit_info_box = new List<Carrot_Box_Item>();

        for (int i = 0; i < obj_item.Length; i++)
        {
            Carrot_Box_Item item_info = this.box.create_item("item_info_" + i);
            item_info.set_icon(this.sp_icon_item[i]);
            item_info.set_title(this.s_name_item[i]);
            item_info.set_tip(this.s_tip_item[i]);
            if (obj_types[i] == Card_Item_Type.card_txt)
            {
                item_info.set_type(Box_Item_Type.box_value_input);
                item_info.set_val(this.obj_item[i].GetComponent<TextMeshProUGUI>().text.Trim());


                Carrot_Box_Btn_Item btn_uppercase = item_info.create_item();
                btn_uppercase.set_icon(this.app.icon_uppercase);
                btn_uppercase.set_act(() => Act_uppercase(item_info));
                btn_uppercase.set_color(app.carrot.color_highlight);
            }

            if (obj_types[i] == Card_Item_Type.card_text_space)
            {
                item_info.set_type(Box_Item_Type.box_value_input);
                item_info.set_val(this.obj_item[i].GetComponent<TextMeshProUGUI>().text.Trim());

                Carrot_Box_Btn_Item btn_uppercase = item_info.create_item();
                btn_uppercase.set_icon(this.app.icon_uppercase);
                btn_uppercase.set_act(() => Act_uppercase(item_info));
                btn_uppercase.set_color(app.carrot.color_highlight);
            }

            if (obj_types[i] == Card_Item_Type.card_sex)
            {
                item_info.set_type(Box_Item_Type.box_value_dropdown);
                item_info.dropdown_val.ClearOptions();
                item_info.dropdown_val.options.Add(new Dropdown.OptionData("Nam"));
                item_info.dropdown_val.options.Add(new Dropdown.OptionData("Nữ"));

                if (this.obj_item[i].GetComponent<TextMeshProUGUI>().text == "Nữ")
                    item_info.set_val("1");
                else
                    item_info.set_val("0");
            }
            
            if (obj_types[i] == Card_Item_Type.card_img)
            {
                item_info.set_act(() => Act_select_avatar(item_info));

                Carrot_Box_Btn_Item btn_camera = item_info.create_item();
                btn_camera.set_icon(this.app.icon_camera);
                btn_camera.set_color(this.app.carrot.color_highlight);
                btn_camera.set_act(() => this.app.carrot.camera_pro.Show_camera(Act_camera_for_avatar));
            }

            item_info.check_type();
            this.list_item_edit_info_box.Add(item_info);

            if (this.obj_item[i].name== "txt_info_date_of_birth")
            {
                InputField inp = item_info.inp_val;
                inp.onValueChanged.RemoveAllListeners();
                inp.onValueChanged.AddListener(On_change_txt_info_date_of_birth);
            }
        }

        Carrot_Box_Btn_Panel panel_btn = this.box.create_panel_btn();
        Carrot_Button_Item btn_done = panel_btn.create_btn("btn_done");
        btn_done.set_icon_white(this.app.carrot.icon_carrot_done);
        btn_done.set_label("Done");
        btn_done.set_label_color(Color.white);
        btn_done.set_bk_color(this.app.carrot.color_highlight);
        btn_done.set_act_click(Act_edit_info_done);

        Carrot_Button_Item btn_clear = panel_btn.create_btn("btn_clear");
        btn_clear.set_icon_white(this.app.carrot.sp_icon_del_data);
        btn_clear.set_label("Clear All");
        btn_clear.set_label_color(Color.white);
        btn_clear.set_bk_color(this.app.carrot.color_highlight);
        btn_clear.set_act_click(Act_clear_all_info);

        Carrot_Button_Item btn_cancel = panel_btn.create_btn("btn_cancel");
        btn_cancel.set_icon_white(this.app.carrot.icon_carrot_cancel);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_bk_color(this.app.carrot.color_highlight);
        btn_cancel.set_act_click(Act_edit_info_cancel);
    }

    private void On_change_txt_info_date_of_birth(string s_val)
    {
        if (s_val.Length >= 10)
        {
            System.DateTime ngay = System.DateTime.ParseExact(s_val, "dd/MM/yyyy", null);
            System.DateTime ngayMoi = ngay.AddYears(40);
            this.list_item_edit_info_box[8].set_val(ngayMoi.ToString("dd/MM/yyyy"));
        }
    }

    private void Act_edit_info_cancel()
    {
        this.app.carrot.play_sound_click();
        box?.close();
    }

    private void Act_clear_all_info()
    {
        this.app.carrot.play_sound_click();
        this.texture_avatar_temp = null;
        for (int i = 0; i < this.obj_item.Length; i++)
        {
            if (this.obj_types[i]==Card_Item_Type.card_sex)
                this.list_item_edit_info_box[i].set_val("0");
            else
                this.list_item_edit_info_box[i].set_val("");
        }
    }

    private void Act_edit_info_done()
    {
        this.app.carrot.play_sound_click();

        for(int i = 0; i < this.obj_item.Length; i++)
        {
            if (this.obj_types[i] == Card_Item_Type.card_txt)
            {
                this.obj_item[i].GetComponent<TextMeshProUGUI>().text = this.list_item_edit_info_box[i].get_val();
                PlayerPrefs.SetString(this.obj_item[i].name, this.list_item_edit_info_box[i].get_val());
            }

            if (this.obj_types[i] == Card_Item_Type.card_sex)
            {
                if(this.list_item_edit_info_box[i].get_val()=="0")
                    this.obj_item[i].GetComponent<TextMeshProUGUI>().text ="Nam";
                else
                    this.obj_item[i].GetComponent<TextMeshProUGUI>().text = "Nữ";

                PlayerPrefs.SetString(this.obj_item[i].name, this.obj_item[i].GetComponent<TextMeshProUGUI>().text);
            }


            if (this.obj_types[i] == Card_Item_Type.card_img)
            {
                if (this.texture_avatar_temp != null)
                {
                    this.obj_item[i].GetComponent<Image>().sprite = this.app.carrot.get_tool().Texture2DtoSprite(this.texture_avatar_temp);
                    this.obj_item[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    this.obj_item[i].GetComponent<Image>().sprite = null;
                    this.obj_item[i].GetComponent<Image>().color = Color.clear;
                }
            }

            if (obj_types[i] == Card_Item_Type.card_text_space)
            {
                this.obj_item[i].GetComponent<TextMeshProUGUI>().text = this.s_keyword_space+ AddLeadingSpaces(this.list_item_edit_info_box[i].get_val(),this.max_length_keyword_space);
                PlayerPrefs.SetString(this.obj_item[i].name, this.list_item_edit_info_box[i].get_val().Trim());
            }

            if (obj_types[i] == Card_Item_Type.card_qr)
            {
                this.img_qr_temp = this.obj_item[i].GetComponent<Image>();
            }

        }

        if (this.qr_index.Length > 0)
        {
            
            this.app.carrot.show_loading();
            string url_create = "https://quickchart.io/qr?text=" + this.Create_Qr();
            this.app.carrot.get_img(url_create, Get_qr_image_done);
        }

        this.app.image_btn_visible.sprite = this.app.carrot.icon_carrot_visible_on;
        this.panel_info.SetActive(true);
        box?.close();
    }

    private string Create_Qr()
    {
        string s_qr = "";
        for(int i = 0; i < this.qr_index.Length; i++)
        {
            int index_emp = this.qr_index[i];
            if (index_emp == 3||index_emp==11)
            {
                s_qr += this.obj_item[index_emp].GetComponent<TextMeshProUGUI>().text.Trim().Replace("/","");
            }
            else
            {
                s_qr += this.obj_item[index_emp].GetComponent<TextMeshProUGUI>().text.Trim();
            }
            
            if(i<this.qr_index.Length-1) s_qr += "|";
        }
        this.s_data_qr_code = UppercaseFirstLetterAfterSpace(s_qr);
        Debug.Log(UppercaseFirstLetterAfterSpace(s_qr));
        return UppercaseFirstLetterAfterSpace(s_qr);
    }

    private void Act_select_avatar(Carrot_Box_Item item_sel)
    {
        this.item_temp = item_sel;
        this.app.carrot.play_sound_click();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"), new FileBrowser.Filter("Pain", ".bmp", ".tiff", ".tga"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.ShowLoadDialog(Act_done_select_avatar, null, FileBrowser.PickMode.Files);
    }

    private void Act_done_select_avatar(string[] path)
    {
        this.item_temp.set_val(path[0]);
        byte[] fileData = FileBrowserHelpers.ReadBytesFromFile(path[0]);

        Texture2D texture = new(2, 2);
        if (ImageConversion.LoadImage(texture, fileData))
        {
            this.texture_avatar_temp = texture;
        }
    }

    private void Act_camera_for_avatar(Texture2D tex)
    {
        this.texture_avatar_temp = tex;
        this.item_temp.set_icon_white(this.app.carrot.get_tool().Texture2DtoSprite(tex));
        this.app.carrot.play_sound_click();
    }

    public void Show_edit_item(int index)
    {
        this.app.carrot.play_sound_click();
        if (this.obj_types[index] == Card_Item_Type.card_txt|| this.obj_types[index] == Card_Item_Type.card_sex)
        {
            this.txt_infor_edit_temp = obj_item[index].GetComponent<TextMeshProUGUI>();
            this.box_input = this.app.carrot.Show_input(this.s_name_item[index], this.s_tip_item[index], txt_infor_edit_temp.text.Trim());
            this.box_input.set_icon(this.sp_icon_item[index]);
            this.box_input.set_act_done(Act_done_edit_info_item);
        } else if (this.obj_types[index] == Card_Item_Type.card_text_space) {
            this.txt_infor_edit_temp = obj_item[index].GetComponent<TextMeshProUGUI>();
            this.box_input = this.app.carrot.Show_input(this.s_name_item[index], this.s_tip_item[index], txt_infor_edit_temp.text.Trim());
            this.box_input.set_icon(this.sp_icon_item[index]);
            this.box_input.set_act_done(Act_done_edit_info_item_space_text);
        }
        else if (this.obj_types[index] == Card_Item_Type.card_img)
        {
            this.Show_edit_photo_item(obj_item[index].GetComponent<Image>());
        } else if (this.obj_types[index] == Card_Item_Type.card_qr)
        {
            this.Show_edit_qr_box(obj_item[index].GetComponent<Image>());
        }
    }

    private void Show_edit_qr_box(Image img_edit)
    {
        app.carrot.play_sound_click();
        if (this.s_data_qr_code == "")
        {
            s_data_qr_code = "";
            for (int i = 0; i < this.obj_item.Length; i++)
            {
                if (this.obj_types[i] == Card_Item_Type.card_txt)
                {
                    s_data_qr_code += this.obj_item[i].GetComponent<TextMeshProUGUI>().text.Trim() + "|";
                }
            }

            s_data_qr_code = s_data_qr_code.Substring(0, s_data_qr_code.Length - 1);
            s_data_qr_code = UppercaseFirstLetterAfterSpace(s_data_qr_code);

        }

        this.img_qr_temp = img_edit;
        this.box = this.app.carrot.Create_Box("QR Code Editor");
        this.box.set_icon(this.app.icon_qr);

        Carrot_Box_Item item_qr_code = this.box.create_item("item_qr_code");
        item_qr_code.set_icon(app.carrot.icon_carrot_write);
        item_qr_code.set_title("Content QR code");
        item_qr_code.set_tip("Text for content qr code");
        item_qr_code.set_type(Box_Item_Type.box_value_txt);
        item_qr_code.check_type();
        item_qr_code.set_val(this.s_data_qr_code);

        Carrot_Box_Item item_qr_file = this.box.create_item("item_qr_file");
        item_qr_file.set_icon(app.icon_open_file);
        item_qr_file.set_title("Select file qr code Image");
        item_qr_file.set_tip("Select the pre-designed qr code image file");
        item_qr_file.set_act(() => Act_show_sel_qr_file());

        Carrot_Box_Item item_qr_create = this.box.create_item("item_qr_create");
        item_qr_create.set_icon(app.icon_qr);
        item_qr_create.set_title("Create Qr code by data content");
        item_qr_create.set_tip("Generate qr code with entered data");
        item_qr_create.set_act(() => Act_show_create_qr_data());
    }

    private void Act_show_sel_qr_file()
    {
        this.app.carrot.play_sound_click();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"), new FileBrowser.Filter("Pain", ".bmp", ".tiff", ".tga"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.ShowLoadDialog(Act_show_sel_qr_file_done, null, FileBrowser.PickMode.Files);
    }

    private void Act_show_sel_qr_file_done(string[] path)
    {
        byte[] fileData = FileBrowserHelpers.ReadBytesFromFile(path[0]);

        Texture2D texture = new(2, 2);
        if (ImageConversion.LoadImage(texture, fileData))
        {
            this.img_qr_temp.sprite = this.app.carrot.get_tool().Texture2DtoSprite(texture);
            this.img_qr_temp.color = Color.white;
        }
        box?.close();
    }

    private void Act_show_create_qr_data()
    {
        this.box_input = this.app.carrot.Show_input("Create Data Qr code","Enter data text to create QR Code");
        box_input.set_act_done(act_done_create_qr_code);
    }

    private void act_done_create_qr_code(string s_data)
    {
        this.s_data_qr_code = s_data;
        string url_create = "https://quickchart.io/qr?text=" + s_data_qr_code;
        this.app.carrot.get_img(url_create, Get_qr_image_done);
    }

    private void Act_done_edit_info_item(string val)
    {
        this.app.carrot.play_sound_click();
        this.txt_infor_edit_temp.text = val;
        if (this.box_input != null) this.box_input.close();
    }

    private void Act_done_edit_info_item_space_text(string val)
    {
        this.app.carrot.play_sound_click();
        this.txt_infor_edit_temp.text = this.s_keyword_space + AddLeadingSpaces(val, this.max_length_keyword_space);
        if (this.box_input != null) this.box_input.close();
    }

    private void Show_edit_photo_item(Image img)
    {
        this.app.carrot.play_sound_click();
        this.img_infor_edit_temp = img;
        this.box = this.app.carrot.Create_Box();
        this.box.set_title("Change Image");
        this.box.set_icon(this.app.carrot.icon_carrot_avatar);

        Carrot_Box_Item item_file_photo = this.box.create_item("item_file_photo");
        item_file_photo.set_icon(this.app.carrot.icon_carrot_avatar);
        item_file_photo.set_title("Select file photo");
        item_file_photo.set_tip("Select file photo from device");
        item_file_photo.set_act(() => this.Act_sel_photo_file(Act_sel_photo_file_done));

        Carrot_Box_Item item_camera_photo = this.box.create_item("item_camera_photo");
        item_camera_photo.set_icon(this.app.icon_camera);
        item_camera_photo.set_title("Take a photo");
        item_camera_photo.set_tip("Use photos taken from your camera");
        item_camera_photo.set_act(() => this.app.carrot.camera_pro.Show_camera(Act_sel_photo_camera));

    }

    private void Act_sel_photo_file(FileBrowser.OnSuccess act_done)
    {
        this.app.carrot.play_sound_click();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png", ".jpeg"), new FileBrowser.Filter("Pain", ".bmp", ".tiff", ".tga"));
        FileBrowser.SetDefaultFilter(".jpg");
        FileBrowser.ShowLoadDialog(act_done, null, FileBrowser.PickMode.Files);
    }

    private void Act_sel_photo_file_done(string[] path)
    {
        byte[] fileData = FileBrowserHelpers.ReadBytesFromFile(path[0]);

        Texture2D texture = new(2, 2);
        if (ImageConversion.LoadImage(texture, fileData))
        {
            texture.filterMode = FilterMode.Bilinear;
            this.img_infor_edit_temp.sprite = app.carrot.get_tool().Texture2DtoSprite(texture);
            this.img_infor_edit_temp.color = Color.white;
        }
        box?.close();
    }


    private void Act_sel_photo_camera(Texture2D tex)
    {
        this.img_infor_edit_temp.sprite = this.app.carrot.get_tool().Texture2DtoSprite(tex);
        box?.close();
    }

    private void Get_qr_image_done(Texture2D tex)
    {
        this.app.carrot.hide_loading();
        Texture2D qr_text = this.app.Change_bk_color(tex);
        this.img_qr_temp.sprite = this.app.carrot.get_tool().Texture2DtoSprite(qr_text);
        this.img_qr_temp.color = Color.white;
        if (box_input != null) box_input.close();
        if(box!=null) box.close();
    }

    private void Act_uppercase(Carrot_Box_Item item_txt)
    {
        this.app.carrot.play_sound_click();
        item_txt.set_val(item_txt.get_val().ToUpper());
    }

    private void Act_import_data_json()
    {
        app.carrot.play_sound_click();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json Data", ".json", ".jsons"), new FileBrowser.Filter("Text Data", ".txt"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.ShowLoadDialog(Act_import_data_json_done, null, FileBrowser.PickMode.Files);
    }

    private void Act_import_data_json_done(string[] path)
    {
        IList list_data = (IList) Json.Deserialize(FileBrowserHelpers.ReadTextFromFile(path[0]));
        for (int i = 0; i < list_data.Count; i++)
        {
            if (this.box.area_all_item.GetChild(i).GetComponent<Carrot_Box_Item>() != null)
            {
                this.box.area_all_item.GetChild(i).GetComponent<Carrot_Box_Item>().set_val(list_data[i].ToString());
            }
        }
        app.carrot.Show_msg("Import data","Import Data success");
    }

    private void Act_export_data_json()
    {
        app.carrot.play_sound_click();
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Json Data", ".json", ".jsons"), new FileBrowser.Filter("Text Data", ".txt"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.ShowSaveDialog(Act_export_data_json_done, null, FileBrowser.PickMode.Files);
    }

    private void Act_export_data_json_done(string[] path)
    {
        IList<string> list_data= new List<string>();
        for (int i = 0; i < this.box.area_all_item.childCount; i++)
        {
            if (this.box.area_all_item.GetChild(i).GetComponent<Carrot_Box_Item>() != null)
            {
                string s_text = this.box.area_all_item.GetChild(i).GetComponent<Carrot_Box_Item>().get_val();
                list_data.Add(s_text);
            }
        }
        FileBrowserHelpers.WriteTextToFile(path[0], Json.Serialize(list_data));
        Debug.Log("Export data:"+Json.Serialize(list_data));
        app.carrot.Show_msg("Export data", "Export Data success!\nAt:" + path[0]);
    }

    public string UppercaseFirstLetterAfterSpace(string input)
    {
        input=input.ToLower();
        char[] chars = input.ToCharArray();
        bool capitalizeNext = true;
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ' ')
            {
                capitalizeNext = true;
            }
            else if (chars[i] == '|')
            {
                capitalizeNext = true;
                chars[i + 1] = char.ToUpper(chars[i + 1]);
            }
            else if (capitalizeNext)
            {
                chars[i] = char.ToUpper(chars[i]);
                capitalizeNext = false;
            }
        }

        return new string(chars);
    }

    public string AddLeadingSpaces(string input, int maxLength)
    {
        string result = input;
        int extraCharacters = input.Length - maxLength;

        if (extraCharacters > 0)
        {
            string leadingSpaces = new string(' ', extraCharacters);
            result = leadingSpaces + input;
            Debug.Log($"Added {extraCharacters} leading spaces.");
        }

        return result;
    }
}
