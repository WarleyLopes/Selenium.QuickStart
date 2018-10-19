using System;
using System.Configuration;
using System.IO;
using System.Threading;
using ScreenRecorderLib;

namespace Selenium.QuickStart.Utilities
{
    internal static class VideoRecorder
    {
        static Recorder _rec;
        static string _fileName;
        private static readonly string path = ConfigurationManager.AppSettings["REPORT_FILE_PATH"];
        private static readonly string fullPath = path + _fileName + ".mp4";

        /// <summary>
        /// Method automatically used on the start of each test for starting the video recording
        /// </summary>
        /// <param name="filename">File name for the record to be temporarily created before converting to Base64</param>
        public static void CreateRecording(string filename)
        {
            _fileName = filename;
            System.IO.Directory.CreateDirectory(path);
            string videoPath = Path.Combine(fullPath);
            RecorderOptions options = new RecorderOptions
            {
                RecorderMode = RecorderMode.Video,

                //If throttling is disabled, out of memory exceptions may eventually crash the program,
                //depending on encoder settings and system specifications.
                IsThrottlingDisabled = false,

                //Hardware encoding is enabled by default.
                IsHardwareEncodingEnabled = true,

                //Low latency mode provides faster encoding, but can reduce quality.
                IsLowLatencyEnabled = false,

                //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
                IsMp4FastStartEnabled = false,
                AudioOptions = new AudioOptions
                {
                    IsAudioEnabled = false
                },
                VideoOptions = new VideoOptions
                {
                    Bitrate = 1000 * 1000,
                    Framerate = 30,
                    IsMousePointerEnabled = true,
                    IsFixedFramerate = true,
                    EncoderProfile = H264Profile.Main
                }
            };
            _rec = Recorder.CreateRecorder(options);
            _rec.OnRecordingComplete += Rec_OnRecordingComplete;
            _rec.OnRecordingFailed += Rec_OnRecordingFailed;
            _rec.OnStatusChanged += Rec_OnStatusChanged;
            try
            {
                _rec.Record(videoPath);
                Thread.Sleep(500);
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static void EndRecording()
        {
            try
            {
                Thread.Sleep(500);
                _rec.Stop();
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal static string GetVideoRecordedAsBase64StringAndDeleteLocalFile()
        {
            string base64String = Convert.ToBase64String(File.ReadAllBytes(fullPath));
            File.Delete(fullPath);
            return base64String;
        }
        private static void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            //Get the file path if recorded to a file
            string path = e.FilePath;
        }
        private static void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            string error = e.Error;
        }
        private static void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
        }
    }
}
