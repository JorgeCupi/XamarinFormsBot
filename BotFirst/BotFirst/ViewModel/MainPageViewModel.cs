using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using System;
using System.Net.Http;
using System.IO;
using System.Text;
using BotFirst.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BotFirst.ViewModel
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        #region Class properties
        static HttpClient streamClient;
        static string botUriStartConversation;
        static string botUriChat;
        static string botSecret;
        static MessageRequest messageRequest;
        #endregion
        #region Properties
        private ObservableCollection<string> _myChat;
        public ObservableCollection<string> myChat
        {
            get { return _myChat; }
            set
            {
                _myChat = value;
                OnPropertyChanged("myChat");
            }
        }
        private void MyChat_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("myChat");
        }

        private string _myMessage;
        public string myMessage
        {
            get { return _myMessage; }
            set
            {
                _myMessage = value;
                OnPropertyChanged("myMessage");
            }
        }
        #endregion

        public MainPageViewModel()
        {
            myChat = new ObservableCollection<string>();
            myChat.CollectionChanged += MyChat_CollectionChanged;
            SendMessageCommand = new Command(SendMessage);
            
            streamClient = new HttpClient();
            botUriStartConversation = "https://directline.botframework.com/v3/directline/conversations/";
            botUriChat = "https://directline.botframework.com/v3/directline/conversations/{0}/activities";
            botSecret = "Bot-Secret-Goes-Here";

            StartConversation();
        }

        #region Commands
        public ICommand SendMessageCommand { get; set; }
        public async void SendMessage()
        {
            // Interaction with Bots go here
            ActivityToPost activityToPost = new ActivityToPost {
                    type = "message",
                    from = new User { id = "user1" },
                    text = myMessage };
            string activity = JsonConvert.SerializeObject(activityToPost);
            StringContent content = new StringContent(activity);
            botUriChat = String.Format(botUriChat, messageRequest.conversationId);
            string result = await PostAsync(botUriChat, content);
            myChat.Add(result);
        }
        #endregion
        #region Own methods
        public async void StartConversation()
        {
            StringContent content = new StringContent("");
            try
            {
                string result = await PostAsync(botUriStartConversation, content);
                messageRequest = JsonConvert.DeserializeObject<MessageRequest>(result);
                myChat.Add("Your bot is connected");

                ////Open Stream to chat with Bot
                //messageRequest.streamUrl = messageRequest.streamUrl.Replace("wss","https");
                //HttpClient streamClient = new HttpClient();
                //HttpResponseMessage resultStream = await streamClient.PostAsync(messageRequest.streamUrl,content);
                //myChat.Add(resultStream.ToString());
            }
            catch (Exception ex)
            {
                botSecret = ex.ToString();
            }
        }

        private static async Task<string> PostAsync(string uri, HttpContent content)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + botSecret);

            try
            {
                HttpResponseMessage response = await client.PostAsync(uri, content);

                Stream stream = await response.Content.ReadAsStreamAsync();
                StreamReader readStream = new StreamReader(stream, Encoding.UTF8);
                string result = readStream.ReadToEnd();
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
