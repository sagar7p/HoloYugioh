using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Firebase.Xamarin.Database;
using Firebase.Xamarin.Database.Query;
using Poz1.NFCForms.Abstract;
using NdefLibrary.Ndef;
using System.Threading.Tasks;

namespace HoloDuelist
{
    public partial class CardPage : ContentPage
    {
        string player;
        string cardType;
        string cardLoc;
        string cardPos;
        string cardSource;
        string cardName;
        FirebaseClient firebase;
        ChildQuery child;
        private readonly INfcForms device;

        public CardPage() => InitializeComponent();


        //constructor for card
        public CardPage(string player, string cardType, string cardLoc, string cardPos, string cardSource)
        {
            InitializeComponent();

            this.player = player;
            this.cardType = cardType;
            this.cardLoc = cardLoc;
            this.cardPos = cardPos;
            this.cardSource = cardSource;


            //init firebase
            firebase = new FirebaseClient("https://holoyugioh.firebaseio.com");
            if (!string.IsNullOrEmpty(player))
            {
                child = firebase.Child(string.Format("Game/Players/{0}/Field/{1}/{2}", player, cardType, cardLoc));

            }

            //init NFC 
            device = DependencyService.Get<INfcForms>();
            device.NewTag += HandleNewTag;



            //init source
            if (!string.IsNullOrWhiteSpace(cardSource))
            {
                card.Source = cardSource;
            }

            //remove specific buttons
            if (cardType.Equals(GameInfo.SpellTrap))
            {
                defense.IsVisible = false;
                down.IsVisible = false;

            }
            else if (cardType.Equals(GameInfo.Monster))
            {
                set.IsVisible = false;
            }

        }

        //nfc found
        private void HandleNewTag(object sender, NfcFormsTag e)
        {
            var record = e.NdefMessage[0];

            // Go through each record, get payload
            var output = System.Text.Encoding.ASCII.GetString(record.Payload);
            var cardname = output.Substring(3);
            Console.WriteLine("Name: " + cardname);
            UpdateCardImage(cardname);

        }

        //tap an action
        async void Action_Tapped(object sender, System.EventArgs e)
        {
            var button = (Frame)sender;
            int position;
            if(int.TryParse(button.ClassId, out position)) 
            {
                //send position to firebase
                if (button.ClassId.Equals(GameInfo.DestroyPos)) 
                {
                    SendToFirebase("");
                }
                else if(!string.IsNullOrEmpty(cardName))
                {
                    SendToFirebase(cardName);
                }

                await Task.Delay(500);
                await child.Child(GameInfo.Position).PutAsync(position);

               
            }

            device.NewTag -= HandleNewTag;

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        //finish typing card
        void Card_Completed(object sender, System.EventArgs e)
        {
            var entry = (Entry)sender;

            UpdateCardImage(entry.Text);
        }

        void UpdateCardImage(string rawText)
        {
            var modified = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(rawText.ToLower());
            var cardText = modified.Replace(' ', '_');
            var source = string.Format("{0}{1}.jpg", GameInfo.GenericCard, cardText);
            card.Source = source;
            cardName = modified;
            //SendToFirebase(modified);
        }

        //send to firebase
        async void SendToFirebase(string name)
        {
            Console.WriteLine("Uploading: " + name);
            await child.Child(GameInfo.Name).PutAsync(name);
        }


    }
}
