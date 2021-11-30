using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CurrencyConverter
{
    // Icon is from https://www.flaticon.com/premium-icon/dollar_798467
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CenterWindowOnScreen();
        }

        public class API_Obj
        {
            public string result { get; set; }
            public string documentation { get; set; }
            public string terms_of_use { get; set; }
            public string time_zone { get; set; }
            public string time_last_update { get; set; }
            public string time_next_update { get; set; }
            public double conversion_rate { get; set; }
            public double conversion_result { get; set; }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // This is to prevent the user from entering anything other than a number

            string txt = Amount_Entered.Text;
            if (txt != "")
            {
                Amount_Entered.Text = Regex.Replace(Amount_Entered.Text, "[^0-9]", "");
                if (txt != Amount_Entered.Text)
                {
                    Amount_Entered.Select(Amount_Entered.Text.Length, 0);
                }
            }
        }

        public void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void Convert_Btn_Clicked(object sender, RoutedEventArgs e)
        {
            string currency_from = ExchangeFrom.Text;
            try
            {
                currency_from = currency_from.Substring(0, 3);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("You have not selected a currency to convert from.");
            }

            string currency_to = ExchangeTo.Text;
            try
            {
                currency_to = currency_to.Substring(0, 3);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("You have not selected a currency to convert to.");
            }


            if (Amount_Entered.Text != "")
            {
                try
                {
                    int amount = Int32.Parse(Amount_Entered.Text);

                    String URLString = $"https://v6.exchangerate-api.com/v6/bc4f21f83db138031d15337c/pair/ {currency_from}/{currency_to}/{amount}";
                    URLString = Regex.Replace(URLString, @"\s+", "");                    
                    using (var webClient = new System.Net.WebClient())
                    {
                        var json = webClient.DownloadString(URLString);
                        API_Obj CurrencyFrom = JsonConvert.DeserializeObject<API_Obj>(json);

                        if ((bool)RoundToInt.IsChecked)
                        {
                            double rate = CurrencyFrom.conversion_rate;
                            int result = (int)Math.Round(CurrencyFrom.conversion_result);

                            // Display the results

                            ConvRate.Content = $"The rate of conversion from {currency_from} to {currency_to} is: {rate}";
                            ConvResult.Content = $"The result of the conversion is: {result} {currency_to}";
                        }
                        else
                        {
                            double rate = CurrencyFrom.conversion_rate;
                            double result = CurrencyFrom.conversion_result;

                            // Display the results

                            ConvRate.Content = $"The rate of conversion from {currency_from} to {currency_to} is: {rate}";
                            ConvResult.Content = $"The result of the conversion is: {result} {currency_to}";
                        }
                    }
                }
                catch (Exception)
                {
                    // MessageBox.Show($"{er}"); <-- keep this for debugging purposes
                    // This is most of the time a 404 error or the number is too high, no need to show it.
                    MessageBox.Show("An error occured while connecting to the server.\nMake sure that the number you entered isn't ridicolously huge ;).");
                }
            }
            else
                MessageBox.Show("No amount was to exchange was entered.");
        }

        private void Exit_Btn_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

