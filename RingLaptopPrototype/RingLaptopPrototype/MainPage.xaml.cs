using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Devices;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RingLaptopPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MediaPlayer player = new MediaPlayer();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RegisterBackgroundTask(sender, e);
        }

        private void RegisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            BackgroundTaskSample.UnregisterBackgroundTasks(BackgroundTaskSample.SampleBackgroundTaskName);
            BackgroundTaskSample.UnregisterBackgroundTasks(BackgroundTaskSample.SampleBackgroundTaskName);

            BackgroundTaskSample.UnregisterBackgroundTasks(BackgroundTaskSample.SampleBackgroundTaskName);
                        BackgroundTaskSample.UnregisterBackgroundTasks(BackgroundTaskSample.SampleBackgroundTaskName);

           BackgroundTaskSample.RegisterBackgroundTask(BackgroundTaskSample.SampleBackgroundTaskEntryPoint,
                BackgroundTaskSample.SampleBackgroundTaskName,
                new SystemTrigger(SystemTriggerType.UserPresent, false) ,null);

            BackgroundTaskSample.RegisterBackgroundTask(BackgroundTaskSample.SampleBackgroundTaskEntryPoint,
                BackgroundTaskSample.SampleBackgroundTaskName,
                new SystemTrigger(SystemTriggerType.SessionConnected, false), null);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (player == null)
            {
                player = new MediaPlayer();
            }
            player.AutoPlay = false;
            player.AudioCategory = MediaPlayerAudioCategory.Media;
            player.MediaEnded += Player_MediaEnded;
            player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/TILE_118_Active.mp3"));

            //lc.LogMessage("Created MediaPlayer instance");

            //lc.LogMessage("playing...");
            player.PlaybackSession.Position = System.TimeSpan.Zero;
            player.IsMuted = false;
            player.Volume = 1;
            player.Play();
        }

        private void Player_MediaEnded(MediaPlayer sender, object args)
        {
            player.PlaybackSession.Position = System.TimeSpan.Zero;
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string audioSelector = MediaDevice.GetAudioRenderSelector();
            var outputDevices = await DeviceInformation.FindAllAsync(audioSelector);
            foreach (var device in outputDevices)
            {
                var deviceItem = new ComboBoxItem();
                deviceItem.Content = device.Name;
                deviceItem.Tag = device;
                AudioDeviceComboBox?.Items?.Add(deviceItem);
            }
        }
    }
}
