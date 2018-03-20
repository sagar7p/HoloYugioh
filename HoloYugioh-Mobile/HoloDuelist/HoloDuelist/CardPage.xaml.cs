using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;

namespace HoloDuelist
{
    public partial class CardPage : ContentPage
    {
        string player;
        string cardpos;
        string cardtype;
        FirebaseClient firebase;
        ChildQuery child;

        public CardPage() => InitializeComponent();

       
        //constructor for card
        public CardPage(string player, string cardpos, string cardtype, string source)
        {
            this.player = player;
            this.cardpos = cardpos;
            this.cardtype = cardtype;
            InitializeComponent();
            Console.WriteLine("P: " + player + " C: " + cardpos);
            firebase = new FirebaseClient("https://holoyugioh.firebaseio.com");
            if (!string.IsNullOrEmpty(player))
            {
                //change to stirng format to clean it up
                child = firebase.Child("Game/Players/" + player + "/Field/" + cardtype + "/" + cardpos);

            }
            if (!string.IsNullOrWhiteSpace(source))
            {
                card.Source = source;
            }

            //remove specific buttons
            if(this.cardtype == "SpellTrap")
            {
                defense.IsVisible = false;
                down.IsVisible = false;

            }
            else if(this.cardtype == "Monster")
            {
                set.IsVisible = false;
            }
           
        }

        //tap an action
        async void Handle_Tapped(object sender, System.EventArgs e)
        {
            var button = (Frame)sender;
            int position;
            if(int.TryParse(button.ClassId, out position)) 
            {
                Console.WriteLine(button.ClassId);
                await child.Child("Position").PutAsync(position);
                Console.WriteLine($"Key for the new item: {button.ClassId}");  
                if (position == 0) 
                {
                    //card.Source = "https://static-3.studiobebop.net/ygo_data/card_images/Token.jpg";
                    SendToFirebase("");
                }
            }
           await Application.Current.MainPage.Navigation.PopAsync();
        }

        //finish typing card
        void Handle_Completed(object sender, System.EventArgs e)
        {
            var entry = (Entry)sender;
            var text = entry.Text;
            var cardText = entry.Text.Replace(' ', '_');
            var source = "https://static-3.studiobebop.net/ygo_data/card_images/" + cardText + ".jpg";
            Console.WriteLine(source);
            card.Source = source;
            SendToFirebase(text);
        }

        //send to firebase
        private async void SendToFirebase(string name)
        {
            await child.Child("Name").PutAsync(name);
            Console.WriteLine($"Key for the new item: {name}");  
        }
    }
}
