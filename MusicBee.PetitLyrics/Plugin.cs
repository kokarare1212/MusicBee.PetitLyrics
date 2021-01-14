using System;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();
        private const string Provider = "プチリリ";
        private static readonly string[] Providers = { Provider };
        private const string petitlyricsClientAppId = "p1110417";
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
            about.VersionMinor = 2;
            about.Revision = 0;
            about.MinInterfaceVersion = 1;
            about.MinApiRevision = 1;
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
            return;
        }
        public void Close(PluginCloseReason reason)
        {
            return;
        }
        public void Uninstall()
        {
            return;
        }
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            return;
        }
        public string[] GetProviders()
        {
            return Providers;
        }
        public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
        {
            if (provider == Provider)
            {
                return GetLyricsByMetadata(artist, trackTitle, album);
            } else {
                return null;   
            }
        }
        public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        {
            return null;
        }
        private string GetLyricsByMetadata(string artist, string trackTitle, string album)
        {
            artist = Uri.EscapeDataString(artist);
            trackTitle = Uri.EscapeDataString(trackTitle);
            album = Uri.EscapeDataString(album);
            var client = new WebClient();
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] postData = System.Text.Encoding.UTF8.GetBytes($"clientAppId={petitlyricsClientAppId}&lyricsType=1&terminalType=10&key_artist={artist}&key_title={trackTitle}&key_album={album}");
            var resp = client.UploadData("http://p0.petitlyrics.com/api/GetPetitLyricsData.php", postData);
            var musicMetadataStr = Encoding.UTF8.GetString(resp);
            var musicMetadataX = XDocument.Parse(musicMetadataStr);
            var lyricsElement = musicMetadataX.Element("response").Element("songs").Element("song").Element("lyricsData");
            if (lyricsElement == null)
            {
                return "";
            }
            var encodedLyrics = lyricsElement.Value;
            var decodedLyrics = Encoding.UTF8.GetString(Convert.FromBase64String(encodedLyrics));
            return decodedLyrics;
        }
    }
}
