using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;


namespace TeamManager
{
    public class TeamMember
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string SurName { get; set; }

        public int Age { get; set; }

        public Gender Gender { get; set; }

        public string LinkedIn_link { get; set; }

        public string Notes { get; set; }

        public byte Record_State_Id { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }

    public static class TeamMembersLoad
    {
        public static List<TeamMember> ReadFile(string filepath)
        {
            var lines = File.ReadAllLines(filepath);

            var data = from l in lines.Skip(1)
                       let split = l.Split(',')
                       select new TeamMember
                       {
                           Id = int.Parse(split[0]),
                           FirstName = split[1],
                           SurName = split[2],
                           Age = int.Parse(split[3]),
                           Gender = (Gender)Enum.Parse(typeof(Gender), split[4]),
                           LinkedIn_link = split[5],
                           Notes = split[6],
                           Record_State_Id = byte.Parse(split[7]),
                           Phone = split[8],
                           Email = split[9]
                       };

            return data.ToList();
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            App.LanguageChanged += LanguageChanged;

            CultureInfo currLang = App.Language;

            //filling the languages menu:
            menuLanguage.Items.Clear();
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(currLang);
                menuLang.Click += ChangeLanguageClick;
                menuLanguage.Items.Add(menuLang);
            }
            DataContext = TeamMembersLoad.ReadFile(@"d:\TeamMembers.csv");
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {
            CultureInfo currLang = App.Language;

            //checking the active language menu Item
            foreach (MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }

        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                CultureInfo lang = mi.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }
        }

        private void bFirstRecord_Click(object sender, RoutedEventArgs e)
        {
            dgrMainData.SelectedItem = dgrMainData.Items[0];
        }

        private void bLastRecord_Click(object sender, RoutedEventArgs e)
        {
            dgrMainData.SelectedItem = dgrMainData.Items.Count;
        }

        private void bNextRecord_Click(object sender, RoutedEventArgs e)
        {
            if (dgrMainData.SelectedIndex < dgrMainData.Items.Count)
                dgrMainData.SelectedItem = dgrMainData.SelectedIndex + 1;
        }

        private void bPriorRecord_Click(object sender, RoutedEventArgs e)
        {
            if (dgrMainData.SelectedIndex > 1)
                dgrMainData.SelectedItem = dgrMainData.SelectedIndex - 1;
        }

        private void dgrMainData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //I bag my pardon, wpf data grid modifiers are different, did not find proper event to operate datasourcce changes in proper way, hare we have overdue 
            if (dgrMainData.Items.Count > 0 && dgrMainData.SelectedIndex >= 0)
            {
                bDeleteSelectedUserData.IsEnabled = true;

                bFirstRecord.IsEnabled = !(dgrMainData.SelectedIndex == 0);
                bPriorRecord.IsEnabled = !(dgrMainData.SelectedIndex == 0);

                bLastRecord.IsEnabled = !(dgrMainData.SelectedIndex == dgrMainData.Items.Count - 1);
                bNextRecord.IsEnabled = !(dgrMainData.SelectedIndex == dgrMainData.Items.Count - 1);
                bEditSelectedUserData.IsEnabled = true;
            }
            else
            {
                bFirstRecord.IsEnabled = false;
                bLastRecord.IsEnabled = false;
                bPriorRecord.IsEnabled = false;
                bNextRecord.IsEnabled = false;
                bEditSelectedUserData.IsEnabled = false;
                bDeleteSelectedUserData.IsEnabled = false;
            }
        }

        private void bNewUserData_Click(object sender, RoutedEventArgs e)
        {
            TeamMember TeamMemberOutput = new TeamMember();
            PersonViewWindow fPVW = new TeamManager.PersonViewWindow();
            fPVW.pId.Text = (dgrMainData.Items.Count + 1).ToString();
            if (fPVW.ShowDialog() == DialogResult)
            {
                TeamMemberOutput.Id = int.Parse(fPVW.pId.Text);
                TeamMemberOutput.FirstName = fPVW.pFirstName.Text;
                TeamMemberOutput.SurName = fPVW.pSurname.Text;
                TeamMemberOutput.Age = int.Parse(fPVW.pAge.Text);
                TeamMemberOutput.Gender = (Gender)Enum.Parse(typeof(Gender), fPVW.pGender.Text);
                TeamMemberOutput.LinkedIn_link = fPVW.pLinkedIn_link.Text;
                TeamMemberOutput.Notes = fPVW.pNotes.Text;
                TeamMemberOutput.Phone = fPVW.pPhone.Text;
                TeamMemberOutput.Email = fPVW.pEmail.Text;

                //InsertNewRow;
            }
        }

        private void bEditSelectedUserData_Click(object sender, RoutedEventArgs e)
        {
            TeamMember TeamMemberInput = (TeamMember)dgrMainData.SelectedItem;
            TeamMember TeamMemberOutput = new TeamMember();
            PersonViewWindow fPVW = new TeamManager.PersonViewWindow();
            fPVW.pId.Text = TeamMemberInput.Id.ToString();
            fPVW.pFirstName.Text = TeamMemberInput.FirstName.ToString();
            fPVW.pSurname.Text = TeamMemberInput.SurName.ToString();
            fPVW.pAge.Text = TeamMemberInput.Age.ToString();
            fPVW.pGender.Text = TeamMemberInput.Gender.ToString();
            fPVW.pLinkedIn_link.Text = TeamMemberInput.LinkedIn_link.ToString();
            fPVW.pNotes.Text = TeamMemberInput.Notes.ToString();
            fPVW.pPhone.Text = TeamMemberInput.Phone.ToString();
            fPVW.pEmail.Text = TeamMemberInput.Email.ToString();
            if (fPVW.ShowDialog() == DialogResult)
            {
                TeamMemberOutput.Id = int.Parse(fPVW.pId.Text);
                TeamMemberOutput.FirstName = fPVW.pFirstName.Text;
                TeamMemberOutput.SurName = fPVW.pSurname.Text;
                TeamMemberOutput.Age = int.Parse(fPVW.pAge.Text);
                TeamMemberOutput.Gender = (Gender)Enum.Parse(typeof(Gender), fPVW.pGender.Text);
                TeamMemberOutput.LinkedIn_link = fPVW.pLinkedIn_link.Text;
                TeamMemberOutput.Notes = fPVW.pNotes.Text;
                TeamMemberOutput.Phone = fPVW.pPhone.Text;
                TeamMemberOutput.Email = fPVW.pEmail.Text;

                //check and update data;
                if (TeamMemberInput != TeamMemberOutput)
                {
                    //update data
                }
            }
        }

        private void bDeleteSelectedUserData_Click(object sender, RoutedEventArgs e)
        {
            if (dgrMainData.SelectedItem != null)
                if (MessageBox.Show("This test application is made up on request of cityshob.com", "About this test app", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    dgrMainData.Items.Remove(dgrMainData.SelectedItem);
        }

        private void miAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This test application is made up on request of cityshob.com", "About this test app", MessageBoxButton.OK, MessageBoxImage.Question);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dgrMainData.SelectAllCells();

            dgrMainData.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, dgrMainData);

            dgrMainData.UnselectAllCells();

            string result = (string)System.Windows.Clipboard.GetData(System.Windows.DataFormats.CommaSeparatedValue);

            File.WriteAllText(@"d:\TeamMembers.csv", result, UnicodeEncoding.UTF8);
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            CultureInfo currLang = App.Language;

            //checking the active language menu Item
            foreach (MenuItem i in menuLanguage.Items)
            {
                CultureInfo ci = i.Tag as CultureInfo;
                i.IsChecked = ci != null && ci.Equals(currLang);
            }
        }
    }
}
