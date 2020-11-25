//using Gma.System.MouseKeyHook;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Media.Effects;
using System.Windows.Controls.Primitives;
using System.Configuration;
using Gma.System.MouseKeyHook;


namespace _1760187_1760223_1760254_MiniProject_MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;

        public MainWindow()
        {

            InitializeComponent();

        }
        private IKeyboardMouseEvents _hook;
        public void Subscribe()
        {
            _hook = Hook.GlobalEvents();
            _hook.KeyUp += _hook_KeyUp;



        }

        private void _hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            
            if(e.Control&&(e.KeyCode== System.Windows.Forms.Keys.P))
            {
                videoPlayer.Play();
            }
            if (e.Control  && (e.KeyCode == System.Windows.Forms.Keys.D))
            {
                videoPlayer.Pause();
            }
            if (e.Control&&e.Shift && (e.KeyCode == System.Windows.Forms.Keys.T))
            {
                for (int i = 0; i < ListVideo.Count; i++)
                {
                    if (ListVideo[i] == fileNameOpen)
                    {


                        if (i == ListVideo.Count)
                            return;
                        else
                        {
                            var next = ListVideo[i + 1];
                            MessageBox.Show(next);

                            fileListView.SelectedIndex = i + 1;
                            fileNameOpen = next;
                            videoPlayer.Source = new Uri(fileNameOpen);

                            _timer.Interval = TimeSpan.FromSeconds(1);
                            _timer.Tick += timer_Tick;
                            videoPlayer.Play();
                            _timer.Start();
                            break;
                        }




                    }
                }
            }
            if (e.Control && e.Shift&&(e.KeyCode == System.Windows.Forms.Keys.L))
            {
                for (int i = 0; i < ListVideo.Count; i++)
                {
                    if (ListVideo[i] == fileNameOpen)
                    {
                        if (i == 0)
                            return;
                        else
                        {
                            var next = ListVideo[i - 1];
                            MessageBox.Show(next);
                            fileListView.SelectedIndex = i - 1;
                            fileNameOpen = next;
                            videoPlayer.Source = new Uri(fileNameOpen);
                            MessageBox.Show(fileNameOpen);

                            _timer.Interval = TimeSpan.FromSeconds(1);
                            _timer.Tick += timer_Tick;
                            videoPlayer.Play();

                            _timer.Start();
                            break;
                        }
                    }
                }
            }
           
        }
        public void Unsubscribe()
        {
            _hook.KeyUp -= _hook_KeyUp;
           
            _hook.Dispose();
        }

      

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Subscribe();
            nutPlay.Source = new BitmapImage(new Uri(@"/Icons/play.png", UriKind.Relative));
           
            {

               // var result = MessageBox.Show("are you want to loading", "Loaded", MessageBoxButton.OKCancel);
                //if (result == MessageBoxResult.OK)
                //{
                //    var reader = new StreamReader("save.dat");
                //    var name = reader.ReadLine();
                //    MessageBox.Show(name);
                //    if (File.Exists(name))
                //    {
                //        if (name != "")
                //        {

                //            while(reader!=null)
                //            {
                //                var a = reader.ReadLine();
                //                MessageBox.Show(a);
                //            }

                //        }
                //        else
                //        {
                //            MessageBox.Show("Old image not exist. Please choose new image");

                //        }
                //    }
                //    else
                //    {
                //        MessageBox.Show("Không tồn tại Tập tin");

                //    }
                //}

            }

        }
       
        string fileNameOpen;
        MediaPlayer _player = new MediaPlayer();
        DispatcherTimer _timer = new DispatcherTimer();
        List<string> ListVideo = null;
        XmlDocument doc = null;
        string filename;
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            ischeck = true;
            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                 filename = screen.FileName;
                 doc = new XmlDocument();
                doc.Load(filename);
              
                var childs = doc.DocumentElement.ChildNodes;
                ListVideo = new List<string>();
                foreach (XmlNode item in childs)
                {
                    var name = item.Attributes["source"].Value;
                    ListVideo.Add(name);

                    fileListView.Items.Add(name);
                }
            }

        }

        // phim tat
        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                videoPlayer.Source = new Uri(openFileDialog.FileName);
            Title = (openFileDialog.FileName);

        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            videoPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);

        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
            lblProgressStatus_1.Text = videoPlayer.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");

        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            videoPlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((videoPlayer.Source != null) && (videoPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = videoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = videoPlayer.Position.TotalSeconds;


            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

            {
                int index = 0;
 
               fileNameOpen = fileListView.SelectedItem.ToString();
                videoPlayer.Source =new Uri(fileNameOpen);
                _timer.Start();
                foreach (string item in fileListView.Items)
                {
                    if(item == fileNameOpen)
                    {
                        fileListView.SelectedIndex = index;
                    }
                    index++;  
                }
                videoPlayer.Play();
                videoPlayer.MediaEnded += video_MediaEnded;
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += timer_Tick;
            }
            MessageBox.Show(fileNameOpen);
            
            



        }

        private void video_MediaEnded(object sender, RoutedEventArgs e)
        {

            next();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _player.Pause();
        }
      
        bool clickvolum = false;
        private void volumoff_Click(object sender, RoutedEventArgs e)
        {
            if (clickvolum == false)
            {
                nutVolum.Source = new BitmapImage(new Uri(@"/Icons/Volumoff.png", UriKind.Relative));
                clickvolum = true;
            }
            else
            {
                nutVolum.Source = new BitmapImage(new Uri(@"/Icons/volum.png", UriKind.Relative));
                clickvolum = false;
            }

        }

        private void FileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        int fullscreen = 0;
        private void FullButton_Click(object sender, RoutedEventArgs e)
        {
            if (fullscreen == 0)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                Trai.Width = 337.5;
                fullscreen = 1;
            }
            else if (fullscreen == 1)
            {

                treeview.Width = 0;
                fileListView.Width = 0;
                Trai.Width = 337.5;
                Tren.Height = 0;
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                fullscreen = 2;
            }
            else if (fullscreen == 2)
            {

                treeview.Width = 0;
                fileListView.Width = 0;
                Tren.Height = 0;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                fullscreen = 0;
                cacButton.Height = 0;
                fullscreen = 3;

            }
        }

        private void Cuaso_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (fullscreen == 3)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                Tren.Height = 10;
                Trai.Width = 50;
                cacButton.Height = 74;
                treeview.Width = 241;
                fileListView.Width = 241;
                fullscreen = 0;
                WindowState = WindowState.Normal;
            }
        }

        private void ZoomoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (fullscreen > 0)
            {
                fullscreen--;
                if (fullscreen == 0)
                {

                    WindowStyle = WindowStyle.SingleBorderWindow;
                    Tren.Height = 10;
                    Trai.Width = 50;
                    cacButton.Height = 74;
                    treeview.Width = 241;
                    fileListView.Width = 241;
                    WindowState = WindowState.Normal;
                }
                else if (fullscreen == 1)
                {
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    Tren.Height = 10;
                    Trai.Width = 337.5;
                    cacButton.Height = 74;
                    treeview.Width = 241;
                    fileListView.Width = 241;
                    WindowState = WindowState.Maximized;
                }
                else if (fullscreen == 2)
                {
                    cacButton.Height = 74;
                    treeview.Width = 0;
                    Trai.Width = 337.5;
                    fileListView.Width = 0;
                    Tren.Height = 0;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                    WindowState = WindowState.Maximized;

                }

            }
        }

        private void MinimizeoutButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        } 
        public static void Save(string key, string value)
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
           
            MessageBox.Show("Save");
        }
        private void FileListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(fileListView.SelectedIndex.ToString());
            fileNameOpen = ListVideo[fileListView.SelectedIndex];
            videoPlayer.Source = new Uri(fileNameOpen);

            Title = (fileNameOpen);
        }
        bool ischeck = false;
        private void Delete_Click(object sender, RoutedEventArgs e)
     
        {

            while (true)
            {
                if (fileListView.SelectedIndex < 0)
                    break;
                else
                {
                    var child = fileListView.SelectedIndex;
                    MessageBox.Show(child.ToString());
                    fileListView.Items.RemoveAt(child);
                    ListVideo.RemoveAt(child);
                    if(ischeck==true)
                    {
                        doc.DocumentElement.RemoveChild(doc.DocumentElement.ChildNodes[child]);
                        doc.Save(filename);
                    }
                }

            }
           
           
           
        }
        bool isPlay_1 = false;
        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("choi 1 lan ");
            isPlay_1 = false;
            int index = fileListView.SelectedIndex;
              
        }
        private void RepeatButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("choi vo tan  ");
            isPlay_1 = true;
            foreach (var item in ListVideo)
            {

            }
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> last = new List<string>();
            last = ListVideo;
            Random random = new Random();
            int index = random.Next(ListVideo.Count);
            fileNameOpen = last[index];


        }
        public void next()
        {
            for (int i = 0; i < ListVideo.Count; i++)
            {
                if (ListVideo[i] == fileNameOpen)
                {


                    if (i == ListVideo.Count)
                    {
                        return;
                       
                    }
                    else
                    {
                        var next = ListVideo[i + 1];
                        MessageBox.Show(next);

                        fileListView.SelectedIndex = i + 1;
                        fileNameOpen = next;
                        videoPlayer.Source = new Uri(fileNameOpen);

                        _timer.Interval = TimeSpan.FromSeconds(1);
                        _timer.Tick += timer_Tick;
                        videoPlayer.Play();
                        _timer.Start();
                        break;
                    }




                }
            }
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            next();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < ListVideo.Count; i++)
            {
                if (ListVideo[i] == fileNameOpen)
                {
                    if (i == 0)
                        return;
                    else
                    {
                        var next = ListVideo[i - 1];
                        MessageBox.Show(next);
                        fileListView.SelectedIndex = i - 1;
                        fileNameOpen = next;
                        videoPlayer.Source = new Uri(fileNameOpen);
                        MessageBox.Show(fileNameOpen);

                        _timer.Interval = TimeSpan.FromSeconds(1);
                        _timer.Tick += timer_Tick;
                        videoPlayer.Play();

                        _timer.Start();
                        break;
                    }
                }
            }
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Pause();
        }

        private void BrowseButton_Click_1(object sender, RoutedEventArgs e)
        {
            ischeck = false;
            var screen = new OpenFileDialog();
            screen.Multiselect = true;
            ListVideo = new List<string>();
            if (screen.ShowDialog() == true)
            {
                foreach (var file in screen.FileNames)
                {
                  
                    fileListView.Items.Add(file);
                    ListVideo.Add(file);

                   

                }
            }
          
        }

        private void foldersItem_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
    }
}

