using SmartAlarmClock.Properties;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    /// <summary>
    /// Реализует класс элемента управления текстового поля с выпадающим списком 
    /// и автоматическим восстановлением предыдущего сеанса    
    /// </summary>
    public class MemoryComboBox : ComboBox
    {
        public MemoryComboBox():base()
        {
        }

        public new string Name
        {
            get { return base.Name; }
            set 
            { 
                base.Name = value;
                MainForm.RegisterSetting(Name, new List<string>());
                LoadSettings();
            }
        }

        public void LoadSettings()
        {
            try
            {
                Items.Clear();
                var col = SmartAlarmClock.Properties.Settings.Default[Name] as List<string>;

                for (int i = col.Count - 1; i >= 0; i--)
                {
                    AddToList(col[i]);
                }
                SelectedIndex = -1;
            }
            catch { }
        }

        public void SaveParametrs()
        {
            List<string> s = new List<string>();

            if (Text.Trim() != "")
                s.Insert(0, Text.Trim());


            foreach (string item in Items)
            {
                if (s.Count < Settings.Default.MaxComboBoxListCapacity)
                {
                    if (item.Trim() != Text.Trim())
                        s.Add(item.Trim());
                }
                else break;
            }
            
            MainForm.SetSettingsValue(Name, s);
        }

        public void AddToList(string s)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (s == Items[i].ToString())
                {
                    Items.RemoveAt(i);
                    break;
                }
            }

            Items.Insert(0, s);
        }
    }
}
