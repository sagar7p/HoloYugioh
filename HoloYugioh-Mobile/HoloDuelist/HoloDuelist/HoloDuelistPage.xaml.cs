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
        public static string Player1;
        public static string Player2;
        public static string TokenCard;
        public static string GenericCard;
        public static string SetCard;
        public static string DestroyPos;
        public static string ActivePos;
        public static string SetPos;
        public static string DefensePos;
        public static string Field;
        public static string SpellTrap;
        public static string Monster;
        public static string LifePoints;
        public static string Name;
        public static string Position;

        public static void Initialize()
        {
            GameName = "Game";
            CurrentPlayer = "Player1";
            Opponent = "Player2";
            Player1 = "Player1";
            Player2 = "Player2";
            TokenCard = "https://static-3.studiobebop.net/ygo_data/card_images/Token.jpg";
            GenericCard = "https://static-3.studiobebop.net/ygo_data/card_images/";
            SetCard = "http://img3.wikia.nocookie.net/__cb20100726082049/yugioh/images/thumb/d/da/Back-JP.png/200px-Back-JP.png";
            DestroyPos = "0";
            ActivePos = "1";
            SetPos = "2";
            DefensePos = "3";
            Field = "Field";
            SpellTrap = "SpellTrap";
            Monster = "Monster";
            LifePoints = "LifePoints";
            Name = "Name";
            Position = "Position";

        }
    }

    public partial class HoloDuelistPage : ContentPage
    {
        public HoloDuelistPage()
        {
            InitializeComponent();

           
        }

        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            Button btn = sender as Button;
            GameInfo.Initialize();
            GameInfo.CurrentPlayer = btn.Text;
            GameInfo.Opponent = GameInfo.CurrentPlayer.Equals(GameInfo.Player1) ? GameInfo.Player2 : GameInfo.Player1; 
            await Navigation.PushAsync(new FieldPage());

        }
    }

   
}
