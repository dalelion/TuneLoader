using NAudio.Lame;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace TuneLoader {
    class Program {

        String UserInput, Title, Path;
        Regex RGX = new Regex("[^a-zA-Z0-9 -]");
        IEnumerable<String> SearchList;

        public static void Main (string[] args) => new Program().Start().GetAwaiter().GetResult();

        private async Task Start () {
            
            YoutubeClient YTC = new YoutubeClient();

            Console.WriteLine("Enter a link or search: ");

            UserInput = Console.ReadLine();

            if (UserInput.ToLower().Contains("youtube.com")) {
                UserInput = YoutubeClient.ParseVideoId(UserInput);
            } else {
                SearchList = await YTC.SearchAsync(UserInput);
                UserInput = SearchList.First();
            }

            VideoInfo VideoInfo = await YTC.GetVideoInfoAsync(UserInput);

            AudioStreamInfo ASI = VideoInfo.AudioStreams.OrderBy(x => x.Bitrate).Last();

            Title = VideoInfo.Title;
            
            Title = RGX.Replace(Title, "");

            Path = "E:/Tunes/" + $"{Title}.mp3";
            var Out = System.IO.File.Create(Path);
            using (var Input = await YTC.GetMediaStreamAsync(ASI))
            using (Out)
                await Input.CopyToAsync(Out);

            
            Console.WriteLine(Title);

            Console.ReadKey();
        }


        LameMP3FileWriter mp3writer = null;
        WaveFormat mp3format = null;

        public void StartMP3Encoding (string filename, WaveFormat format) {
            
            mp3format = format;
            mp3writer = new LameMP3FileWriter(filename, format, 128);
        }


    }
}
