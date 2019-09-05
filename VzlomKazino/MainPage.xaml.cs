using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VzlomKazino.Models;
using Xamarin.Forms;
using System.IO;
using Newtonsoft.Json;

namespace VzlomKazino
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private Player GetPlayer()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "config.json");
            if (File.Exists(folder))
            {
                var text = File.ReadAllText(folder);
                try
                {
                    return JsonConvert.DeserializeObject<PlayerContext>(text);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void Update()
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "config.json");
            if (File.Exists(folder))
            {
                File.WriteAllText(folder, JsonConvert.SerializeObject(_player));
            }
            else
            {
                File.Create(folder).Close();
                File.WriteAllText(folder, JsonConvert.SerializeObject(_player));
            }
        }

        private Player _player;

        private bool abort = false;
        private int _currentBuff = 0;
        private decimal _money = 0;

        public decimal Money
        {
            get => _money;
            private set
            {
                _money = Math.Round(Money, 2);
            }
        }

        public double Vzlomano { get => Math.Round(progressBar.Progress * 100, 2) * _currentBuff; }
        public string Progress { get => Math.Round(progressBar.Progress * 100, 2).ToString() + $"% ({_currentBuff * Vzlomano}₽)"; }

        public MainPage()
        {
            
            InitializeComponent();
            _player = GetPlayer() ?? new Player();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            _currentBuff = new Random().Next(98, 200);

            state1.IsVisible = false;
            state2.IsVisible = true;
            state3.IsVisible = false;
            ushort timeOut =
#if DEBUG
                500;
#endif
#if !DEBUG
            (ushort)new Random(Environment.TickCount + 1).Next(6500, 19000);
#endif

            for (; progressBar.Progress < 1 && !abort; progressBar.Progress += 0.01)
            {
                pgLabel.Text = Progress;
                _player.XP += (uint)new Random().Next(1, 5);
                await Task.Run(() => Thread.Sleep(timeOut));
            }
            abort = false;
            vzLabel.Text = (Vzlomano * _currentBuff).ToString();

            _player.HackCount++;
            if (progressBar.Progress == 1)
            {
                _player.FullHackCount++;
                _player.XP += 10;
            }

            _player.AddStat((decimal)Vzlomano);

            Update();

            state2.IsVisible = false;
            state3.IsVisible = true;

            Money += (decimal)Vzlomano;
            progressBar.Progress = 0;
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            abort = true;
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Level: {_player.Level}");
            builder.AppendLine($"XP: {_player.XP}");
            builder.AppendLine($"Need to next level: {(Player.NeedsXpForLevel * (Player.NeedsXpPerLevel * _player.Level)) - _player.XP}");
            builder.AppendLine($"Total XP: {_player.TotalXP}");
            builder.AppendLine();
            builder.AppendLine($"Casino hacked: {_player.FullHackCount} ({_player.HackCount})");
            builder.AppendLine($"AVG money: {_player.Average}");
            await DisplayAlert("Stats", builder.ToString(), "Ok");
        }

        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            if (await DisplayAlert("Reset stats", "Do you want reset stats?", "Yes.", "No."))
            {
                _player = new Player();
                Update();
            }
        }
    }
}
