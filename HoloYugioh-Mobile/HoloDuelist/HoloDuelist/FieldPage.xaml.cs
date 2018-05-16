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
            InitFirebase();
        }

        //Initialize Firebase
        public void InitFirebase()
        {
            //Initialize Firebase
            firebase = new FirebaseClient("https://holoyugioh.firebaseio.com");
            firebase.Child(GameInfo.GameName).AsObservable<JObject>().Subscribe(_ => InitCards());
                 
        }

        //Get cards
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
                InitPlayer(players[GameInfo.CurrentPlayer], GameInfo.CurrentPlayer, GameInfo.Player1);
                InitPlayer(players[GameInfo.Opponent], GameInfo.Opponent, GameInfo.Player2);
            }
        }

        //get cards from firebase
        public void InitPlayer(JToken player,string replacePlayer, string fieldPlayer)
        {
            var field = player[GameInfo.Field];
            var lp = player[GameInfo.LifePoints];

            var monsters = field[GameInfo.Monster];
            var spellTrap = field[GameInfo.SpellTrap];
            for (var i = 1; i <= 6; i++)
            {
                //go through every card
                string cardIndex = "Card" + i;

                //monsters
                if (i  < 6)
                {
                    //get monster path
                    var monster = monsters[cardIndex];
                    var monsterPath = monster.Path.Replace('.', '_').Replace(replacePlayer, fieldPlayer);
                    var monImage = this.FindByName<Image>(monsterPath);
                    var monText = monster[GameInfo.Name].ToString().Replace(' ', '_');
                    var monPos = monster[GameInfo.Position].ToString();

                    //set source and position
                    if (!monPos.Equals(GameInfo.DestroyPos) && !string.IsNullOrEmpty(monText))
                    {
                        var source = string.Format("{0}{1}.jpg", GameInfo.GenericCard, monText);
                        monImage.Parent.StyleId = source;
                        monImage.StyleId = monPos;
                        monImage.Scale = 1;
                        monImage.Rotation = 0;

                        if (monPos.Equals(GameInfo.DefensePos) || monPos.Equals(GameInfo.SetPos))
                        {
                            monImage.Rotation = 90;
                            monImage.Scale = 0.7;
                            source = monPos.Equals(GameInfo.SetPos) ? GameInfo.SetCard : source;
                        }



                        monImage.Source = source;

                    }

                    //reset ids
                    else
                    {
                        monImage.Source = string.Empty;
                        monImage.Parent.StyleId = string.Empty;
                        monImage.StyleId = string.Empty;
                    } 
                }


                //spell get path
                var magic = spellTrap[cardIndex];
                var magicPath = magic.Path.Replace('.', '_').Replace(replacePlayer, fieldPlayer);
                 
                var magImage = this.FindByName<Image>(magicPath);

                var magText = magic[GameInfo.Name].ToString().Replace(' ', '_');
                var magPos = magic[GameInfo.Position].ToString();


                //set ids and source
                if (!magPos.Equals(GameInfo.DestroyPos) && !string.IsNullOrEmpty(magText))
                {
                    var source = string.Format("{0}{1}.jpg", GameInfo.GenericCard, magText);
                    magImage.Parent.StyleId = source;
                    magImage.StyleId = magPos;
                    magImage.Source = magPos.Equals(GameInfo.SetPos) ? GameInfo.SetCard : source;

                }

                else
                {
                    magImage.Source = string.Empty;
                    magImage.Parent.StyleId = string.Empty;
                    magImage.StyleId = string.Empty;
                } 
               
            }
        }

        //go to specific card type
        async void Card_Clicked(object sender, System.EventArgs e)
        {
            Frame card = sender as Frame;
            var image = card.FindByName<Image>(card.ClassId);

            var fullName = card.ClassId;
            var path = fullName.Split('_');
            var player = path[0];

            //center button
            if (player.Equals("DUEL")) return;

            var loc = path[0];

            //life points
            if (loc.Equals(GameInfo.LifePoints))
            {
                //TO DO
            }

            //navigate to page
            else
            {
                var cardType = path[2];
                var cardLoc = path[3];
                var cardPos = image.StyleId;
                var cardSource = card.StyleId;
                await Navigation.PushAsync(new CardPage(player.Equals(GameInfo.CurrentPlayer) ? GameInfo.Player1 : GameInfo.Player2, cardType, cardLoc, cardPos, cardSource));

            }

        }


    }

}
