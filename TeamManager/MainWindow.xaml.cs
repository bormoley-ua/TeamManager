using System;
using System.Collections.Generic;
using System.Data;
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

            DataContext = TeamMembersLoad.ReadFile(@"d:\TeamMembers.csv");

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
            if (dgrMainData.Items.Count > 0)
            {
                bEditSelectedUserData.IsEnabled = true;
                bDeleteSelectedUserData.IsEnabled = true;

                bFirstRecord.IsEnabled = !(dgrMainData.SelectedIndex == 1);
                bPriorRecord.IsEnabled = !(dgrMainData.SelectedIndex == 1);

                bLastRecord.IsEnabled = !(dgrMainData.SelectedIndex == dgrMainData.Items.Count);
                bNextRecord.IsEnabled = !(dgrMainData.SelectedIndex == dgrMainData.Items.Count);
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
            PersonViewWindow fPVW = new TeamManager.PersonViewWindow();
            fPVW.ShowDialog();
        }

        private void bEditSelectedUserData_Click(object sender, RoutedEventArgs e)
        {
            //TeamMember currentTeamMemberData;
            //currentTeamMemberData.Id =
            MessageBox.Show(dgrMainData.SelectedItem.ToString());
            PersonViewWindow fPVW = new TeamManager.PersonViewWindow();
            fPVW.ShowDialog();
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
    }
}
