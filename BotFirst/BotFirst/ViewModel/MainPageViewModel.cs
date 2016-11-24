// Most of the heavy lifting occurs here. Probably in a new release I'll wrap
// and refactorize some code to have a clean library ready to share.
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
		static GetResult getResult;
		static string botUriStartConversation;
        static string botUriChat;
        static string botSecret;
        static string activity;
		static bool firstMessage;
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
                // Creating the JSON string that will be upload as an activity to the BOT
                // For now it seems unnecesary to create a JSON object and then serialize it into a string
                // but in the coming future this Activity Object can become more complex and have useful info to use
                // in other parts of the project.
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

        // Just instantiating the Uris, HTTP Clients and the Observable collection that 
        // will hold the messages between the user and the Bot.
        public MainPageViewModel()
        {
            myChat = new ObservableCollection<string>();
            myChat.CollectionChanged += MyChat_CollectionChanged;
            SendMessageCommand = new Command(SendMessage);
			getResult = new GetResult();
			firstMessage = true;

            botUriStartConversation = "https://directline.botframework.com/v3/directline/conversations/";
            botUriChat = "https://directline.botframework.com/v3/directline/conversations/{0}/activities";
            botSecret = "Your-Bot-Secret-Goes-Here";

            chatClient = new HttpClient();
            startConversationClient = new HttpClient();
            chatClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + botSecret);
            startConversationClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + botSecret);

            StartConversation();
        }

        #region Commands
        public ICommand SendMessageCommand { get; set; }
        // Once a conversation has been started we are able to send and receive messages from the Bot
        // For this, we are just making a POST request to the BOT and if we receive a MessageID we're confirming
        // the BOT received our message, so we can make a GET request and we'll get a list of all the activities 
        // (chats) the Bot had with the current MessageID.
        // Right now, everytime we make a GET request we'll receive all the conversation history. 
        // For future commits we'll work with watermarks to receive only the latest message.
        public async void SendMessage()
        {
            myChat.Add("Me: "+ myMessage);
			// Posting an activity to Bot
			postResult = JsonConvert.DeserializeObject<PostResult>(await PostAsync(botUriChat, content));
            // if Bot succesfully received the message then it replies with an ID:
            if (postResult !=null)
            {
				// Polling the Bot's reply with a GET request
				// The first time we make a GET request we don't have a watermark for the BOT to know which messages 
				// are the latest in comparison to our last request so we don't send a watermark
				if (firstMessage)
				{
					getResult = JsonConvert.DeserializeObject<GetResult>(await chatClient.GetStringAsync(botUriChat));
					firstMessage = false;
				}
				// Now that we have a watermark we can send it so the Bot framework won't send the whole conversation history everytime 
				// we make a GET request.
				else
				{
					string jsonResultFromBot = await chatClient.GetStringAsync(botUriChat + "?watermark=" + getResult.watermark);
					getResult = JsonConvert.DeserializeObject<GetResult>(jsonResultFromBot);
				}
				// The BOT will return 2 activities instead of the whole conversation now that we are using watermarks.
				// But why 2 and not just 1? That is because the message we sent wasn't part of the conversation yet since the last GET
				// request we did, so both OUR message and the BOT's reply come as new activities in the latest GET.
				myChat.Add("Bot: " + getResult.activities[1].text);
            }
            
        }
        #endregion
        #region Methods
        // For the Bot to be able to chat, first it needs a MessageID which is granted by starting a conversation
        // or recovering a MessageID from the past. In the meantime I'm creating a new conversation everytime the 
        // App starts. This will improve with future commits by properly handling Tokens, saving MessageIDs, checking
        // watermarks and so on.
        public async void StartConversation()
        {
            StringContent content = new StringContent("",Encoding.UTF8,"application/json");
            try
            {
                string result = await PostAsync(botUriStartConversation, content);
                messageRequest = JsonConvert.DeserializeObject<MessageRequest>(result);
                botUriChat = String.Format(botUriChat, messageRequest.conversationId);
                myChat.Add("Bot: I'm all set up!");
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
