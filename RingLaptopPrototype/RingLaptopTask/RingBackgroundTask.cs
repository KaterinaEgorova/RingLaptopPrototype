//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Diagnostics;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System.Threading;

//
// The namespace for the background tasks.
//
namespace Tasks
{
    //
    // A background task always implements the IBackgroundTask interface.
    //
    public sealed class RingBackgroundTask : IBackgroundTask
    {
        BackgroundTaskCancellationReason _cancelReason = BackgroundTaskCancellationReason.Abort;
        volatile bool _cancelRequested = false;
        BackgroundTaskDeferral _deferral = null;
        ThreadPoolTimer _periodicTimer = null;
        uint _progress = 0;
        IBackgroundTaskInstance _taskInstance = null;
        static MediaPlayer player = null;
        static LoggingChannel lc = null;

        //
        // The Run method is the entry point of a background task.
        //
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();
            if (lc == null)
            {
                lc = new LoggingChannel("Tile", null, new Guid("4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a"));
            }

            lc.LogMessage("Run entry");
            Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...");

            PlaySound();
            
            //
            // Associate a cancellation handler with the background task.
            //
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            //
            // Get the deferral object from the task instance, and take a reference to the taskInstance;
            //
            //_deferral = taskInstance.GetDeferral();
            _taskInstance = taskInstance;

            //_periodicTimer = ThreadPoolTimer.CreatePeriodicTimer(new TimerElapsedHandler(PeriodicTimerCallback), TimeSpan.FromSeconds(1));
        }

        private void PlaySound()
        {
            //ToastHelper.PopToast("Ring Laptop", $"the task is activated");

            if (player == null)
            {
                player = new MediaPlayer();

                player.MediaEnded += OnMediaPlayerEnded;
                player.MediaFailed += OnMediaFailed;
                player.AutoPlay = false;
                player.AudioCategory = MediaPlayerAudioCategory.Media;


                player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/TILE_118_Active.mp3"));

                lc.LogMessage("Created MediaPlayer instance");
            }

            lc.LogMessage("playing...");
            player.PlaybackSession.Position = System.TimeSpan.Zero;
            player.IsMuted = false;
            player.Volume = 1;
            player.Play();
            //_deferral?.Complete();
        }

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //
            // Indicate that the background task is canceled.
            //
            _cancelRequested = true;
            _cancelReason = reason;

            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
            _deferral.Complete();
        }

        private void OnMediaPlayerEnded(MediaPlayer sender, object args)
        {
            lc.LogMessage("OnMediaPlayerEnded");

            sender.PlaybackSession.Position = TimeSpan.Zero;
            sender.Dispose();
            _deferral?.Complete();
        }

        private void OnMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            lc.LogMessage("OnMediaFailed");

            _deferral.Complete();
        }
    }
}
