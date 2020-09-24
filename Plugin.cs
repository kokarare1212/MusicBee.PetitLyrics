﻿using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();
        private const string Provider = "プチリリ";
        private static readonly string[] Providers = { Provider };
        private const string petitClientAppId = "p1110417";

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = $"{Provider}";
            about.Description = $"{Provider}から歌詞を取得します";
            about.Author = "kokarare1212";
            about.TargetApplication = "";
            about.Type = PluginType.LyricsRetrieval;
            about.VersionMajor = 1;
            about.VersionMinor = 0;
            about.Revision = 0;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;
            return about;
        }

        public bool Configure(IntPtr panelHandle)
        {
            return false;
        }
        public void SaveSettings()
        {
        }
        public void Close(PluginCloseReason reason)
        {
        }
        public void Uninstall()
        {
        }
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
        }
        public string[] GetProviders()
        {
            return Providers;
        }
        public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
        {
            switch (provider)
            {
                case Provider:
                    return GetLyricsByMetadata(artist, trackTitle, album);
                default:
                    return null;
            }
        }
        public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        {
            return null;
        }
        private string GetLyricsByMetadata(string artist, string trackTitle, string album)
        {
            var client = new WebClient();
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] postData = System.Text.Encoding.UTF8.GetBytes($"clientAppId={petitClientAppId}&lyricsType=1&terminalType=10&key_artist={artist}&key_title={trackTitle}&key_album={album}");
            var resp = client.UploadData("https://p1.petitlyrics.com/api/GetPetitLyricsData.php", postData);
            var musicMetadataStr = Encoding.UTF8.GetString(resp);
            var lyrics = Encoding.UTF8.GetString(
                Convert.FromBase64String(
                    Regex.Match(musicMetadataStr, "<lyricsData>(?<lyrics>.*)</lyricsData>")
                        .Groups["lyrics"].Value
                )
            );
            return lyrics;
        }
    }
}