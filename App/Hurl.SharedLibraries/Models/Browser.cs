using Hurl.SharedLibraries.Services;
using System.Text.Json;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Text.Json.Serialization;

namespace Hurl.SharedLibraries.Models
{
    public class Browser
    {
        public Browser(string Name, string ExePath)
        {
            this.Name = Name;
            this.ExePath = ExePath;
            if (ExePath != null)
            {
                this.RawIcon = ExePath.StartsWith('"'.ToString())
                        ? IconExtractor.FromFile(ExePath.Substring(1, ExePath.Length - 2))
                        : IconExtractor.FromFile(ExePath);
            }
        }

        //[JsonInclude]
        //public BrowserSourceType SourceType { get; set; } // IMP for refreshing system browsers

        [JsonInclude]
        public string Name { get; set; }

        [JsonInclude]
        public string ExePath { get; set; }

        [JsonInclude]
        public string LaunchArgs { get; set; }

        [JsonInclude]
        public bool Hidden { get; set; } = false;

        [JsonInclude]
        public AlternateLaunch[] AlternateLaunches { get; set; }

        private Icon RawIcon { get; set; }

        [JsonIgnore]
        public ImageSource GetIcon
        {
            get
            {
                if (ExePath != null && RawIcon != null)
                    return IconUtilites.ToImageSource(RawIcon);
                else return null;
            }
        }
    }

    //[JsonObject(MemberSerialization.OptOut)]
    public class AlternateLaunch
    {
        public AlternateLaunch() { }

        public AlternateLaunch(string ItemName, string LaunchArgs)
        {
            this.ItemName = ItemName;
            this.LaunchArgs = LaunchArgs;
            //this.IsPath = false;
        }

        //public AlternateLaunch(string ItemName, string ExePath, string LaunchArgs)
        //{
        //    this.ItemName = ItemName;
        //    this.LaunchExe = ExePath;
        //    this.LaunchArgs = LaunchArgs;
        //    this.IsPath = true;
        //}

        [JsonInclude]
        public string ItemName { get; set; }
        
        //[JsonInclude]
        //public string LaunchExe { get; set; }
        
        [JsonInclude]
        public string LaunchArgs { get; set; }
        
        //[JsonInclude]
        //public bool IsPath { get; set; }
    }

    public enum BrowserSourceType
    {
        Registry,
        User
    }
}