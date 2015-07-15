using System;
using System.IO;    //FileStream
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;   //Debug
using Microsoft.Win32;      //OpenFileDialog


namespace ImageViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // ファイルリスト
        List<string> fileList = new List<string>();
        // 画像読み込み用クラス
        ImageController controller;
        // 画像データ保存用BitmapImage
        BitmapImage imageHandle = new BitmapImage();
        // TransFromグループ
        TransformGroup trans = new TransformGroup();
        // フルスクリーン状態
        bool isFullScreen;


        /// <summary>
        /// オリジナルコマンド用RoutedCommand
        /// </summary>
        public static readonly RoutedCommand ViewFullScreen
            = new RoutedCommand("ViewFullScreenCmd", typeof(MainWindow));   // 全画面表示コマンド
        public static readonly RoutedCommand Zoom 
            = new RoutedCommand("ZoomCmd", typeof(MainWindow));             // 画像拡大コマンド
        public static readonly RoutedCommand Reduction
            = new RoutedCommand("ReductionCmd", typeof(MainWindow));        // 画像縮小コマンド
        public static readonly RoutedCommand RotateRight 
            = new RoutedCommand("RotateRightCmd", typeof(MainWindow));      // 右回転
        public static readonly RoutedCommand RotateLeft 
            = new RoutedCommand("RotateLeftCmd", typeof(MainWindow));       // 左回転
        public static readonly RoutedCommand FitWindow 
            = new RoutedCommand("FitWindowCmd", typeof(MainWindow));        // 画面に合わせ拡大
        public static readonly RoutedCommand FitWidth 
            = new RoutedCommand("FitWidthCmd", typeof(MainWindow));         // 横幅に合わせ拡大
        public static readonly RoutedCommand ShowVersion 
            = new RoutedCommand("ShowVersionCmd", typeof(MainWindow));      // バージョン情報の表示

        public static readonly RoutedCommand getNextImage
            = new RoutedCommand("GetNextImageCmd", typeof(MainWindow));     // 次の画像を取得
        public static readonly RoutedCommand getPrevImage
            = new RoutedCommand("GetPrevImageCmd", typeof(MainWindow));     // 前の画像を取得
        

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ファイルを開くコマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // ファイルダイアログの設定
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Filter = Properties.Settings.Default.FileFilter;

            // 画像変形のためのTransFormGroupに拡大・回転のTransformクラスを追加
            trans.Children.Add(new ScaleTransform());
            trans.Children.Add(new RotateTransform());

            // 画像表示用のpictureview1にTransFormGroupを設定
            pictureview1.LayoutTransform = trans;

            // フルスクリーンフラグの設定
            isFullScreen = false;

            // ファイルダイアログの表示
            try
            {
                bool? result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    controller = new ImageController(openFileDialog.FileNames);
                    imageHandle = controller.Init();
                    pictureview1.Source = imageHandle;      
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 終了コマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppCloseCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 切り取りコマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CutCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// コピーコマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        /// <summary>
        /// 貼り付けコマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        /// <summary>
        /// 全画面表示コマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewFullScreenCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            isFullScreen = !isFullScreen;
            if (isFullScreen == true)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.Topmost = true;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                this.Topmost = false;
            }
        }

        /// <summary>
        /// 画像拡大コマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScaleTransform scale = (ScaleTransform)trans.Children[0];
            scale.ScaleX += 0.1;
            scale.ScaleY += 0.1;
            scale.CenterX = pictureview1.ActualWidth / 2;
            scale.CenterY = pictureview1.ActualHeight / 2;                        
        }

        /// <summary>
        /// 画像縮小コマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReductionCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScaleTransform scale = (ScaleTransform)trans.Children[0];
            if (scale.ScaleX == 0.1) 
            {  
                scale.ScaleX = 0.1;
                scale.ScaleY = 0.1;
            }
            else if(scale.ScaleX > 0.2)
            {
                scale.ScaleX -= 0.1;
                scale.ScaleY -= 0.1;
                
            }

            scale.CenterX = pictureview1.ActualWidth / 2;
            scale.CenterY = pictureview1.ActualHeight / 2;
            Debug.WriteLine("Scale X: " + scale.ScaleX);
            Debug.WriteLine("Scale Y: " + scale.ScaleY);
        }

        /// <summary>
        /// 右回転コマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateRightCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RotateTransform rotate = (RotateTransform)trans.Children[1];
            rotate.Angle += 90;
            if (rotate.Angle == 360)
            {
                rotate.Angle = 0;
            }
        }

        /// <summary>
        /// 左回転コマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RotateLeftCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RotateTransform rotate = (RotateTransform)trans.Children[1];
            rotate.Angle -= 90;
            if (rotate.Angle == -360)
            {
                rotate.Angle = 0;
            }
        }

        /// <summary>
        /// 画面に合わせ拡大するコマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FitWindowCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScaleTransform scale = (ScaleTransform)trans.Children[0];

            double x = this.ActualHeight / pictureview1.Source.Height;
            double y = this.ActualHeight / pictureview1.Source.Height;

            scale.ScaleX = x;
            scale.ScaleY = y;
        }

        /// <summary>
        /// 横幅に合わせ拡大するコマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FitWidthCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScaleTransform scale = (ScaleTransform)trans.Children[0];
            
            double x = this.ActualWidth / pictureview1.Source.Width;
            double y = this.ActualWidth / pictureview1.Source.Width;

            scale.ScaleX = x;
            scale.ScaleY = y;
            
            Debug.WriteLine("FitWidth");
        }

        /// <summary>
        /// バージョン情報表示コマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowVersionCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 次の画像を表示イベントが発生した際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getNextImageCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            pictureview1.Source = controller.getNextImage();            
            Debug.WriteLine("CurrentNum:" + controller.currentImageNum);
        }

        /// <summary>
        /// 前の画像を表示イベントが発生した際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getPrevImageCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            pictureview1.Source = controller.getPrevImage();
            Debug.WriteLine("CurrentNum:" + controller.currentImageNum);
        }
    }
}
