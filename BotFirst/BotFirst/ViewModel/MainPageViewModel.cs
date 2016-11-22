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
        static HttpClient chatClient;
        static HttpClient startConversationClient;
        static PostResult postResult;
        static ActivityToPost activityToPost;
        static string botUriStartConversation;
        static string botUriChat;
        static string botSecret;
        static string activity;
        static MessageRequest messageRequest;
        static StringContent content;
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
                activityToPost = new ActivityToPost
                {
                    type = "message",
                    from = new User { id = "user1" },
                    text = myMessage
                };
                activity = JsonConvert.SerializeObject(activityToPost);
                content = new StringContent(activity, Encoding.UTF8, "application/json");

                OnPropertyChanged("myMessage");
            }
        }
        #endregion

        public MainPageViewModel()
        {
            myChat = new ObservableCollection<string>();
            myChat.CollectionChanged += MyChat_CollectionChanged;
            SendMessageCommand = new Command(SendMessage);

            botUriStartConversation = "https://directline.botframework.com/v3/directline/conversations/";
            botUriChat = "https://directline.botframework.com/v3/directline/conversations/{0}/activities";
            botSecret = "00lpHMa5tIU.cwA.3jM.FU2X7eHnLTLhwti165wVHmjYfYqrxghzUKD991lG2HI";
            //botSecret = "Your-Bot-Secret-Goes-Here";

            chatClient = new HttpClient();
            startConversationClient = new HttpClient();
            chatClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + botSecret);
            startConversationClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + botSecret);

            StartConversation();
        }

        #region Commands
        public ICommand SendMessageCommand { get; set; }
        public async void SendMessage()
        {
            // Posting an activity to Bot
            myChat.Add(myMessage);
            postResult = JsonConvert.DeserializeObject<PostResult>(await PostAsync(botUriChat, content));
            // if Bot succesfully received the message then it replies with an ID:
            if (postResult !=null)
            {
                // Polling the Bot's reply with a GET request
                GetResult getResult = JsonConvert.DeserializeObject<GetResult>(await chatClient.GetStringAsync(botUriChat));

                myChat.Add(getResult.activities[int.Parse(getResult.watermark) - 1].text);
            }
            
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
                botUriChat = String.Format(botUriChat, messageRequest.conversationId);
                myChat.Add("Your bot is connected");
            }
            catch (Exception ex)
            {
                botSecret = ex.ToString();
            }
        }

        private static async Task<string> PostAsync(string uri, HttpContent content)
        {
            try
            {
                HttpResponseMessage response = await startConversationClient.PostAsync(uri, content);

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
