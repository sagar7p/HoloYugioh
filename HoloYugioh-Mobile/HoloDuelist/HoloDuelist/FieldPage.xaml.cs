using System;
using System.Collections.Generic;
using Firebase.Xamarin.Database;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace HoloDuelist
{
    public partial class FieldPage : ContentPage
    {
        FirebaseClient firebase;

        //initializings
        public FieldPage()
        {
            InitializeComponent();
            firebase = new FirebaseClient("https://holoyugioh.firebaseio.com");
            firebase.Child(GameInfo.GameName).AsObservable<JObject>().Subscribe(cards => InitCards());

            InitCards();

        }

        protected override void OnAppearing()
        {
            InitCards();
        }

        //get player cards
        public async void InitCards()
        {
            var items = await firebase.Child(GameInfo.GameName).OnceAsync<JObject>();

           

            foreach (var item in items)
            {
                //Players
                var players = item.Object;
                var player1 = players["Player1"];
                var player2 = players["Player2"];
                InitPlayer(player1);
                InitPlayer(player2);

                //Console.WriteLine($"{item.Key} name is {item.Object.ToString()}");
            }
        }

        //get cards from firebase
        public void InitPlayer(JToken player)
        {
            var field = player["Field"];
            var lp = player["LifePoints"];

            var monsters = field["Monster"];
            var SpellTrap = field["SpellTrap"];
            for (var i = 1; i < 6; i++)
            {
                string cardIndex = "Card" + i;
                var monster = monsters[cardIndex];
                var magic = SpellTrap[cardIndex];
                var monImage = this.FindByName<Image>(monster.Path.Replace('.','_'));
                var magImage = this.FindByName<Image>(magic.Path.Replace('.', '_'));

                var monText = monster["Name"].ToString().Replace(' ','_');
                var monPos = monster["Position"].ToString();
                var magText = magic["Name"].ToString().Replace(' ', '_');
                var magPos = magic["Position"].ToString();


                if(monPos != "0" && !string.IsNullOrEmpty(monText)) 
                {
                    var source = "https://static-3.studiobebop.net/ygo_data/card_images/" + monText + ".jpg";
                    monImage.Parent.StyleId = source;
                    monImage.Scale = 1;
                    monImage.Rotation = 0;

                    if (monPos == "2" || monPos == "3")
                    {
                        monImage.Rotation = 90;
                        monImage.Scale = 0.7;
                        source = monPos=="2" ? "https://orig00.deviantart.net/9c03/f/2013/196/d/3/yugioh_card_back_v2_by_endergon_oscuro-d6dlsyg.jpg" : source;
                    }
                   

                    monImage.Source = source;

                }

                else
                {
                    monImage.Source = string.Empty;
                    monImage.Parent.StyleId = string.Empty;
                }

                if (magPos != "0" && !string.IsNullOrEmpty(magText))
                {
                    var source = "https://static-3.studiobebop.net/ygo_data/card_images/" + magText + ".jpg";
                    magImage.Parent.StyleId = source;

                    source = magPos == "2" ? "https://orig00.deviantart.net/9c03/f/2013/196/d/3/yugioh_card_back_v2_by_endergon_oscuro-d6dlsyg.jpg" : source;
                    magImage.Source = source;
                }

                else
                {
                    magImage.Source = string.Empty;
                    magImage.Parent.StyleId = string.Empty;
                }
               

            }
        }

        //go to specific card type
        async void Handle_Clicked(object sender, System.EventArgs e)
        {
            Frame btn = sender as Frame; 
            int x=(int)btn.GetValue(Grid.RowProperty); 
            int y=(int)btn.GetValue(Grid.ColumnProperty);

            string player = "";
            string cardpos = "";
            string cardtype = "";

            //player 1
            if (x > 2) {
                player = "Player1";
                cardpos = "Card" + (y+1);
                cardtype = x == 3 ? "Monster" : "SpellTrap";
            }

            //player 2
            else if(x < 2) {
                player = "Player2";
                cardpos = "Card" + (5 - y);
                cardtype = x == 1 ? "Monster" : "SpellTrap";
            }
            //fieldspell
            else {
                if (y == 0 || y == 4) {
                    player = y == 0 ? "Player1"  : "Player2";
                    cardpos = "Card6";
                    cardtype = "SpellTrap";
                }
            }

            //if(GameInfo.CurrentPlayer.Equals(player))
            await Navigation.PushAsync(new CardPage(player,cardpos,cardtype,btn.StyleId));
        }
    }

}
