using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Plugghest.Helpers
{
    public class Youtube
    {
        private string _youTubeCode; //11 character code

        public Youtube()
        {
            IsValid = false;
        }

        public Youtube(string code)
        {
            code = code.Trim();
            if (code.Length == 11)
            {
                YouTubeCode = code;
                IsValid = true;
            }
            else
            {
                if (code.IndexOf("www.youtube.com") > -1)
                    //Assume that code is the final 11 characters
                {
                    YouTubeCode = code.Substring(code.Length - 11, 11);
                    IsValid = true;
                }
                else
                {
                    IsValid = false;
                }
            }
        }

        public string YouTubeCode
        {
            get { return _youTubeCode; }
            set 
            {
                if (value.Length == 11)
                {
                    _youTubeCode = value;
                    IsValid = true;
                }
                else
                    throw new Exception("Youtube code must have 11 characters");
            }
        }

        public bool IsValid { get; set; }

        public string GetIframeString(string CultureCode)
        {
            if(CultureCode.Length != 2)
                throw new Exception("Culture code must have 2 characters");
            return "<iframe width=\"640\" height=\"390\" src=http://www.youtube.com/embed/" + YouTubeCode + "?cc_load_policy=1&amp;cc_lang_pref=" + CultureCode + "en\" frameborder=\"0\"></iframe>";
        }

        public string GetYouTubeData(string FilterBy, string videoID)
        {
            int lb = 0;
            int ub = 0;
            string videoHTML = "";
            string videoData = "";
            string vidMarker = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://gdata.youtube.com/feeds/api/videos?q=" + videoID);
            StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream());
            switch (FilterBy)
            {
                case "Title":
                    vidMarker = "<media:title type='plain'>";
                    if (string.IsNullOrEmpty(vidMarker)) return string.Empty;
                    videoHTML = sr.ReadToEnd();
                    lb = videoHTML.IndexOf(vidMarker) + vidMarker.Length;
                    ub = videoHTML.IndexOf("</media:title>", lb);
                    videoData = videoHTML.Substring(lb, ub - lb);
                    break;
                case "Views":
                    vidMarker = "viewCount='";
                    if (string.IsNullOrEmpty(vidMarker)) return string.Empty;
                    videoHTML = sr.ReadToEnd();
                    lb = videoHTML.IndexOf(vidMarker) + vidMarker.Length;
                    ub = videoHTML.IndexOf("'", lb);
                    videoData = videoHTML.Substring(lb, ub - lb);
                    break;
                case "Length":
                    vidMarker = "<yt:duration seconds='";
                    if (string.IsNullOrEmpty(vidMarker)) return string.Empty;
                    videoHTML = sr.ReadToEnd();
                    lb = videoHTML.IndexOf(vidMarker) + vidMarker.Length;
                    ub = videoHTML.IndexOf("'", lb);
                    string Seconds = videoHTML.Substring(lb, ub - lb);
                    TimeSpan t = TimeSpan.FromSeconds(int.Parse(Seconds));
                    videoData = t.Minutes + ":" + t.Seconds;
                    break;
            }
            return videoData;
        }


    }

}
