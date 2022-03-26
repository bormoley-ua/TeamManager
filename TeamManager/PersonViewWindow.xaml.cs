using System.Windows;


namespace TeamManager
{
    /// <summary>
    /// Interaction logic for PersonViewWindow.xaml
    /// </summary>
    public partial class PersonViewWindow : Window
    {
        public PersonViewWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
