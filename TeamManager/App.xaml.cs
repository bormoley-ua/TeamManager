using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace TeamManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		private static List<CultureInfo> m_Languages = new List<CultureInfo>();

		public static List<CultureInfo> Languages
		{
			get
			{
				return m_Languages;
			}
		}

		public App()
		{
			m_Languages.Clear();
			m_Languages.Add(new CultureInfo("en-US")); //should be set ast project neutral culture, I have app exception after changing ths property therefore it was left (none)
			m_Languages.Add(new CultureInfo("he-HE"));
		}

		//Event to notify all app windows of language change
		public static event EventHandler LanguageChanged;

		public static CultureInfo Language
		{
			get
			{
				return System.Threading.Thread.CurrentThread.CurrentUICulture;
			}
			set
			{
				if (value == null) throw new ArgumentNullException("value");
				if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

				//1. Changing app language
				System.Threading.Thread.CurrentThread.CurrentUICulture = value;

				//2. Creating ResourceDictionary ffor new culture
				ResourceDictionary dict = new ResourceDictionary();
				switch (value.Name)
				{
					case "he-HE":
						dict.Source = new Uri(String.Format(System.AppDomain.CurrentDomain.BaseDirectory+"Resources/lang.{0}.xaml", value.Name), UriKind.Relative);
						break;
					default:
						dict.Source = new Uri("Resources/lang.xaml", UriKind.Relative);
						break;
				}

				//3. Locating old ResourceDictionary nad removing it, Ndding new ResourceDictionary
				ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
											  where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
											  select d).First();
				if (oldDict != null)
				{
					int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
					Application.Current.Resources.MergedDictionaries.Remove(oldDict);
					Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
				}
				else
				{
					Application.Current.Resources.MergedDictionaries.Add(dict);
				}

                //4. Calling and event to notify all app windows of language change
                LanguageChanged(Application.Current, new EventArgs());
			}
		}
	}
}
