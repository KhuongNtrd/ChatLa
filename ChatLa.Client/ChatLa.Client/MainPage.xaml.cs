using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Xamarin.Forms;

namespace ChatLa.Client
{
    public class Message
    {
        public string Content { get; set; }
        public bool Mine { get; set; }
    }
    public partial class MainPage : ContentPage
    {
        private IHubProxy ChatHub { get; }
        private HubConnection Connection { get; }

        public ObservableCollection<Message> Collection { get; set; }
        public MainPage()
        {
            InitializeComponent();
            Collection = new ObservableCollection<Message>();
            Connection = new HubConnection("http://192.168.54.2:5000");
            ChatHub = Connection.CreateHubProxy("ChatHub");
            ChatHub.On("Ready", OnReady);
            ChatHub.On<string, string>("Message", OnMessage);

            MainListView.ItemsSource = Collection;
        }

        private void OnMessage(string message, string userId)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Message() { Content = message, Mine = userId == Connection.ConnectionId };
                Collection.Add(msg);
            });
        }

        public bool IsReady { get; set; } = false;
        private void OnReady()
        {
            IsReady = true;

            UpdateStatus("Người lạ đã vào phòng, bạn có thể chat");
        }

        private async void ActionClicked(object sender, EventArgs e)
        {
            MainButton.IsEnabled = false;

            await Connection.Start();

            if (IsReady == false)
                UpdateStatus("Đã vào, đang chờ người lạ");
        }


        private void UpdateStatus(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                StatusLabel.Text = message;
            });
        }

        private async void SendOnClicked(object sender, EventArgs e)
        {
            ChatButton.IsEnabled = false;
            await ChatHub.Invoke("Chat", ChatEntry.Text);
            ChatEntry.Text = string.Empty;
            ChatButton.IsEnabled = true;
        }

        private async void ChatEntry_OnCompleted(object sender, EventArgs e)
        {
            ChatButton.IsEnabled = false;
            await ChatHub.Invoke("Chat", ChatEntry.Text);
            ChatEntry.Text = string.Empty;
            ChatButton.IsEnabled = true;
        }
    }
}
