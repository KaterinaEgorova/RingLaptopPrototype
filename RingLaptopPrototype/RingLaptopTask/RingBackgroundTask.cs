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
using Windows.Foundation.Diagnostics;
using Windows.Media.Core;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using Windows.Media.Playback;

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
        BackgroundTaskDeferral _deferral = null;
        IBackgroundTaskInstance _taskInstance = null;
        MediaPlayer player = null;
        LoggingChannel lc = null;
        private ExtendedExecutionForegroundSession session = null;

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

            lc.LogMessage(taskInstance.Task.Name + " Entry");

            //PlaySound();

            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();

                lc.LogMessage("app launched");

                deferral.Complete();
            }
            

            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            _taskInstance = taskInstance;
        }

        private void PlaySound()
        {
            lc.LogMessage("PlaySound");

            //ToastHelper.PopToast("Ring Laptop", $"the task is activated");
            if (player == null)
            {
                player = new MediaPlayer();

                player.MediaEnded += OnMediaPlayerEnded;
                player.MediaFailed += OnMediaFailed;
                player.AutoPlay = true;
                player.AudioCategory = MediaPlayerAudioCategory.Media;
                player.CurrentStateChanged += MediaPlayer_CurrentStateChanged;

                player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/TILE_118_Active.mp3"));

                lc.LogMessage("Created MediaPlayer instance");
            }

            lc.LogMessage("playing...");
            player.PlaybackSession.Position = System.TimeSpan.Zero;
            player.IsMuted = false;
            player.Volume = 1;
            player.Play();
        }

        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            lc.LogMessage("OnCanceled");

            //
            // Indicate that the background task is canceled.
            //
            _cancelReason = reason;

            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
            player.Dispose();
            _deferral.Complete();
        }

        private async void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            lc.LogMessage("MediaPlayer_CurrentStateChanged");
            if (sender.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                session = new ExtendedExecutionForegroundSession();
                session.Reason = ExtendedExecutionForegroundReason.BackgroundAudio;
                var result = await session.RequestExtensionAsync();

                lc.LogMessage($"requested extended session... result: {result}");
                if (result != ExtendedExecutionForegroundResult.Allowed)
                {
                    lc.LogMessage($"denied");

                    throw new Exception("EE denied");
                }
            }
        }

        private void OnMediaPlayerEnded(MediaPlayer sender, object args)
        {
            lc.LogMessage("OnMediaPlayerEnded");

            sender.PlaybackSession.Position = TimeSpan.Zero;
            player.Dispose();
            _deferral?.Complete();
        }

        private void OnMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            lc.LogMessage("OnMediaFailed");

            player.Dispose();

            _deferral.Complete();
        }
    }
}
