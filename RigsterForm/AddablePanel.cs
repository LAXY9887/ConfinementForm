using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static RigsterForm.MainForm;
namespace RigsterForm
{
    // 選項
    public enum targetRefType { Label , TextBox, IDTextBox , Button , ComboBox }

    public class AddablePanel
    {
        // 初始面板
        public Panel initial_Panel;

        // 所在GroupBox中
        public GroupBox group_Box;

        // 高度差異
        public const int highShift = 5;

        // 數量
        public int panel_nums { get; set; }

        // 列表
        public List<Panel> panel_list { get; set; }

        // 按鈕
        public List<Button> deleteButtons { get; set; }

        // 建構涵式
        public AddablePanel(Panel initialPanel,GroupBox groupBox) 
        {
            // 初始面板
            initial_Panel = initialPanel;

            // 所在群組
            group_Box = groupBox;

            // 加入列表
            panel_list = new List<Panel> { initial_Panel };
            deleteButtons = new List<Button>();

            // 加入第一科按鈕
            foreach (Control ctrl in initial_Panel.Controls)
            {
                if (ctrl is Button)
                {
                    deleteButtons.Add((Button)ctrl);
                }
            }

            // 計算數字
            panel_nums = panel_list.Count;
        }

        // 從初始版面中擷取參考
        public List<Control> GetPanelReference(targetRefType type)
        {
            List<Control> ctrl2return = new List<Control>();

            switch (type)
            {
                // Label
                case targetRefType.Label:
                    foreach (Control ctrl in initial_Panel.Controls)
                    {
                        if (ctrl is Label)
                        {
                            ctrl2return.Add(ctrl);
                        }
                    }
                    break;

                // 文字框
                case targetRefType.TextBox:
                    foreach (Control ctrl in initial_Panel.Controls)
                    {
                        if (ctrl is TextBox && ctrl.Name.Contains(ConstParameters.TextBoxNamePrefix))
                        {
                            ctrl2return.Add(ctrl);
                        }
                    }
                    break;

                // 身分證框
                case targetRefType.IDTextBox:
                    foreach (Control ctrl in initial_Panel.Controls)
                    {
                        if (ctrl is TextBox && ctrl.Name.Contains(ConstParameters.TextBoxIDPrefix))
                        {
                            ctrl2return.Add(ctrl);
                        }
                    }
                    break;

                // 按鈕
                case targetRefType.Button:
                    foreach (Control ctrl in initial_Panel.Controls)
                    {
                        if (ctrl is Button)
                        {
                            ctrl2return.Add(ctrl);
                        }
                    }
                    break;

                // 選單
                case targetRefType.ComboBox:
                    foreach (Control ctrl in initial_Panel.Controls)
                    {
                        if (ctrl is ComboBox)
                        {
                            ctrl2return.Add(ctrl);
                        }
                    }
                    break;
            }

            return ctrl2return;
        }

        // 調整UI
        public void adjustGUILayout(int shiftHeight, List<Control> GUI2BMoved) 
        {
            // 擴大群組框
            group_Box.Size = new Size(group_Box.Width, group_Box.Height + shiftHeight);

            // 擴大隱藏面板
            Panel collectionPanel = group_Box.Parent as Panel;
            collectionPanel.Size = new Size(collectionPanel.Width, collectionPanel.Height + shiftHeight);

            // 將被影響的GUI移動
            foreach (Control ctrl in GUI2BMoved)
            {
                ctrl.Top += shiftHeight;
            }
        }

        // 複製一個新Panel
        public Panel createPanel(Panel reference, string name, Color color) 
        {
            Panel newPannel = new Panel();
            newPannel.Location = new Point(reference.Location.X, reference.Location.Y + (reference.Height + highShift) * panel_nums);
            newPannel.Size = new Size(reference.Width, reference.Height);
            newPannel.BackColor = color;
            newPannel.Name = name + panel_nums.ToString();
            return newPannel;
        }

        // 複製一個新Label
        public Label createLabel(Label reference) 
        {
            Label newlabel = new Label();
            newlabel.Text = reference.Text;
            newlabel.Font = reference.Font;
            newlabel.Location = reference.Location;
            newlabel.Size = reference.Size;
            return newlabel;
        }

        // 複製一個新TextBox
        public TextBox createTextBox(TextBox reference, string name, string input_text) 
        {
            TextBox newTextBox = new TextBox();
            newTextBox.Location = reference.Location;
            newTextBox.Font = reference.Font;
            newTextBox.Size = reference.Size;
            newTextBox.Name = name + panel_nums.ToString();
            newTextBox.Text = input_text;
            return newTextBox;
        }

        // 複製一個新的ComboBox
        public ComboBox createComboBox(ComboBox reference) 
        {
            ComboBox newComboBox = new ComboBox();
            newComboBox.Location = reference.Location;
            newComboBox.Font = reference.Font;
            newComboBox.Size = reference.Size;
            newComboBox.Text = reference.Text;
            foreach (var item in reference.Items)
            {
                newComboBox.Items.Add(item);
            }
            return newComboBox;
        }

        // 複製一個新Button
        public Button createButton(Button reference, string name) 
        {
            Button newBtn = new Button();
            newBtn.Location = reference.Location;
            newBtn.Text = reference.Text;
            newBtn.Size = reference.Size;
            newBtn.Font = reference.Font;
            newBtn.Cursor = reference.Cursor;
            newBtn.FlatStyle = reference.FlatStyle;
            newBtn.Name = name + panel_nums.ToString();
            return newBtn;
        }

        // 載入所有Control
        public void LoadControls2Group(Panel targetPanel,List<Control> refList, targetRefType type, string inputStr = "") 
        {
            if (refList.Count > 0)
            {
                foreach (Control ctrl in refList)
                {
                    switch (type)
                    {
                        case targetRefType.Label:
                            Label newLabel = createLabel((Label)ctrl);
                            targetPanel.Controls.Add(newLabel);
                            break;

                        case targetRefType.TextBox:
                            TextBox newTextBox = createTextBox((TextBox)ctrl, ConstParameters.TextBoxNamePrefix, inputStr);
                            targetPanel.Controls.Add(newTextBox);
                            break;

                        case targetRefType.IDTextBox:
                            TextBox newIDTextBox = createTextBox((TextBox)ctrl, ConstParameters.TextBoxIDPrefix, inputStr);
                            targetPanel.Controls.Add(newIDTextBox);
                            break;

                        case targetRefType.ComboBox:
                            ComboBox newComboBox = createComboBox((ComboBox)ctrl);
                            targetPanel.Controls.Add(newComboBox);
                            break;

                        case targetRefType.Button:
                            Button newButton = createButton((Button)ctrl, ConstParameters.ButtonNamePrefix);
                            targetPanel.Controls.Add(newButton);
                            deleteButtons.Add(newButton);
                            break;
                    }
                    
                }
            }
        }

        // 建立一個新的手機面板
        public void AddNewAppPanel(string input_phone_number)
        {
            // 新增一個Panel
            Panel newPhonePanel = createPanel(initial_Panel, ConstParameters.PanelNamePrefix, Color.Azure);

            // 加入所有Label
            List<Control> refLabel = GetPanelReference(targetRefType.Label);
            LoadControls2Group(newPhonePanel, refLabel, targetRefType.Label);

            // 加入所有TextBox
            List<Control> refTextBox = GetPanelReference(targetRefType.TextBox);
            LoadControls2Group(newPhonePanel, refTextBox, targetRefType.TextBox,inputStr: input_phone_number);

            // 加入所有Button
            List<Control> refButton = GetPanelReference(targetRefType.Button);
            LoadControls2Group(newPhonePanel, refButton, targetRefType.Button);

            // 將新Panel和Button加入列表
            panel_list.Add(newPhonePanel);
            panel_nums = panel_list.Count; // 更新數量

            // 將新Panel加入Group
            group_Box.Controls.Add(newPhonePanel);
        }

        // 刪除一個手機面板
        public void DeleteAppPanel(Panel panel2delete) 
        {
            // 尋找要被刪掉的引索和按鈕
            int deleteIDX = panel_list.IndexOf(panel2delete);
            Button btn2Removed = deleteButtons[deleteIDX];

            // 將該引索之後的面板全部上移並改名
            for (int i = 0; i < panel_list.Count; i++)
            {
                if (i > deleteIDX)
                {
                    // 移動高度
                    int shiftHeight = initial_Panel.Height + highShift;

                    // 上移
                    panel_list[i].Top -= shiftHeight;
                }
            }

            // 將該Panel和Button移除list
            panel_list.Remove(panel2delete);
            deleteButtons.Remove(btn2Removed);
            panel_nums = panel_list.Count;  // 更新數量

            // 將該Panel移除Group
            group_Box.Controls.Remove(panel2delete);
        }

        // 按鈕點擊後觸發完整新增行動
        public void AddButtonClicked(List<Control> controlsList, string input_phone_number) 
        {
            // 移動高度
            int shiftHeight = initial_Panel.Height + highShift;

            // 移動調整GUI
            adjustGUILayout(shiftHeight, controlsList);

            // 加入一個新的 phone_pannel
            AddNewAppPanel(input_phone_number);
        }

        // 按鈕點擊後觸發完整刪除行動
        public void DeleteButtonClicked(Panel panel_to_delete, List<Control> controlsList) 
        {
            // 更新數量
            panel_nums = panel_list.Count;
            
            // 剩一個不能按
            if (panel_nums > 1)
            {
                // 移動高度
                int shiftHeight = -(initial_Panel.Height + highShift);

                // 移動調整GUI
                adjustGUILayout(shiftHeight, controlsList);

                // 刪除一個 phone_pannel
                DeleteAppPanel(panel_to_delete);
            }
        }

        // 初始化 Panels (只保留一個, 其他全部刪除)
        public virtual void InitializePanels(List<Control> controlsList) 
        {
            while (panel_nums > 1)
            {
                Panel panel2Delete = panel_list[0];
                DeleteButtonClicked(panel2Delete, controlsList);
            }
        }

        // 根據資料庫內容加入電話, 用於修改舊資料模式
        public void LoadPhones(List<Control> controlsList, List<string> phones) 
        {
            foreach (string phone in phones)
            {
                AddButtonClicked(controlsList, phone);
            }

            // 刪掉第一個空的
            DeleteButtonClicked(panel_list[0], controlsList);
        }
    }

    // 新生兒面板
    public class newBornPanel : AddablePanel 
    {
        // Control 的名字
        private const string newBorn_panel_namePrefix = "_newBorn_panel_";

        // 建構式
        public newBornPanel(Panel initialPanel, GroupBox groupBox) : base(initialPanel, groupBox)
        {
            // 加入列表
            panel_list = new List<Panel> { initial_Panel };
            deleteButtons = new List<Button>();

            // 加入第一科按鈕
            foreach (Control ctrl in initial_Panel.Controls)
            {
                if (ctrl is Button)
                {
                    deleteButtons.Add((Button)ctrl);
                }
            }

            // 計算數字
            panel_nums = panel_list.Count;
        }

        // 新增一個新生兒面板
        public void AddNewBornPanel(string Name="", string ID = "") 
        {
            // 新增一個Panel
            Panel newBornPanel = createPanel(initial_Panel, newBorn_panel_namePrefix, Color.Honeydew);

            // 加入所有Label
            List<Control> refLabel = GetPanelReference(targetRefType.Label);
            LoadControls2Group(newBornPanel, refLabel, targetRefType.Label);

            // 加入所有TextBox
            List<Control> refTextBox = GetPanelReference(targetRefType.TextBox);
            LoadControls2Group(newBornPanel, refTextBox, targetRefType.TextBox, inputStr:Name);

            // 加入所有身分證TextBox
            List<Control> refIDTextBox = GetPanelReference(targetRefType.IDTextBox);
            LoadControls2Group(newBornPanel, refIDTextBox, targetRefType.IDTextBox, inputStr: ID);

            // 加入所有ComboBox
            List<Control> refComboBox = GetPanelReference(targetRefType.ComboBox);
            LoadControls2Group(newBornPanel, refComboBox, targetRefType.ComboBox);

            // 加入所有Button
            List<Control> refButton = GetPanelReference(targetRefType.Button);
            LoadControls2Group(newBornPanel, refButton, targetRefType.Button);

            // 將新Panel和Button加入列表
            panel_list.Add(newBornPanel);
            panel_nums = panel_list.Count; // 更新數量

            // 將新Panel加入Group
            group_Box.Controls.Add(newBornPanel);
        }

        // 按鈕點擊後觸發完整新增行動
        public void AddNBButtonClicked(List<Control> controlsList, string Name = "", string ID = "")
        {
            // 移動高度
            int shiftHeight = initial_Panel.Height + highShift;

            // 移動調整GUI
            adjustGUILayout(shiftHeight, controlsList);

            // 加入一個新的
            AddNewBornPanel(Name, ID);
        }

        // 根據資料庫內容加入電話, 用於修改舊資料模式
        public void LoadnewBorns(List<Control> controlsList, List<newBornInfo> nbInfos)
        {
            foreach (newBornInfo info in nbInfos)
            {
                AddNBButtonClicked(controlsList,info.nbNames,info.newbornID);
            }

            // 刪掉第一個空的
            DeleteButtonClicked(panel_list[0], controlsList);
        }
    }
}
