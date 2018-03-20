using Xamarin.Forms;
using Firebase.Xamarin.Database;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HoloDuelist
{
    public static class GameInfo
    {
        public static string GameName;
        public static string CurrentPlayer;
        public static string Opponent;
        public static string TokenCard;

        public static void Initialize()
        {
            GameName = "Game";
            CurrentPlayer = "Player1";
            Opponent = "Player2";
            TokenCard = "https://static-3.studiobebop.net/ygo_data/card_images/Token.jpg";
        }
    }

    public partial class HoloDuelistPage : ContentPage
    {
        public HoloDuelistPage()
        {
            InitializeComponent();

            //create duel button
            Button button = new Button
            {
                Text = "Duel!",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            //upon button click set game and current player
            button.Clicked += async (sender, args) =>
            {
                await Navigation.PushAsync(new FieldPage());
            };

            Content = button;
        }
    }

   
}
