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
using MahApps.Metro.Controls; // MetroWindow



namespace ImageViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        // ファイルリスト
        List<string> fileList = new List<string>();
        // 画像読み込み用クラス
        ImageController controller;
        // 画像データ保存用BitmapImage
        BitmapImage imageHandle;
        // TransFromグループ
        TransformGroup trans = new TransformGroup();
        
        // フルスクリーン状態
        bool isFullScreen;

        // 現在の画像の幅
        double imageWidth;

        // 現在の画像の高さ
        double imageHeight;


        /// <summary>
        /// オリジナルコマンド用RoutedCommand
        /// </summary>
        public static readonly RoutedCommand showFullScreen
            = new RoutedCommand("showFullScreenCmd", typeof(MainWindow));   // 全画面表示コマンド
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

        public static readonly RoutedCommand showFileList
            = new RoutedCommand("ShowFileListCmd", typeof(MainWindow));     // ファイルリストの表示・非表示
        public static readonly RoutedCommand selectShowFileList
            = new RoutedCommand("selectShowFileListCmd", typeof(MainWindow));   // ファイルリストの項目を選択

        public static readonly RoutedCommand showImageTags
            = new RoutedCommand("ShowImageTagsCmd", typeof(MainWindow));    // イメージタグの表示

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
                    imageHandle = new BitmapImage();
                    imageHandle = controller.Init();
                    pictureview1.Source = imageHandle;
                    imageHeight = imageHandle.PixelHeight;
                    imageWidth = imageHandle.PixelWidth;
                    ScaleTransform scale = (ScaleTransform)trans.Children[0];

                    if (fileListBox.Items != null)
                    {
                        fileListBox.Items.Clear();
                    }
                    BitmapImage src;
                    System.Windows.Controls.Image thumb;
                    foreach (string item in controller.FileList)
                    {
                        thumb = new System.Windows.Controls.Image();
                        thumb.Margin = new Thickness(10);
                        src = new BitmapImage();
                        src.BeginInit();
                        src.UriSource = new Uri(item);
                        src.DecodePixelWidth=150;
                        src.EndInit();
                        thumb.Source = src;
                        fileListBox.Items.Add(thumb);
                    }
                    
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
        private void showFullScreenCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            isFullScreen = !isFullScreen;
            if (isFullScreen == true)
            {
                //this.WindowStyle = WindowStyle.None;
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
        /// ファイルリストの表示非表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showFileListCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (showFileListMenu.IsChecked)
            {
                mainGrid.ColumnDefinitions[0].Width = GridLength.Auto;
            }
            else
            {
                mainGrid.ColumnDefinitions[0].Width = new GridLength(0);
            }
            
        }

        /// <summary>
        /// ファイルリストの項目を選択した際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectShowFileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fileListBox.Items.Count > 0)
            {
                RotateTransform rotate = (RotateTransform)trans.Children[1];
                imageHandle = controller.getSelectedImage(fileListBox.SelectedIndex);
                pictureview1.Source = imageHandle;
                rotate.Angle = 0;
                imageHeight = imageHandle.PixelHeight;
                imageWidth = imageHandle.PixelWidth;
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

            scale.CenterX = scrollviewer1.ActualWidth / 2;
            scale.CenterY = scrollviewer1.ActualHeight / 2;
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
            double tmp = imageHeight;
            imageHeight = imageWidth;
            imageWidth = tmp;
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
            double tmp = imageHeight;
            imageHeight = imageWidth;
            imageWidth = tmp;
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

            double scalex = scrollviewer1.ActualWidth / imageWidth;
            double scaley = scrollviewer1.ActualHeight / imageHeight;

            if (scalex > scaley)
            {
                scale.ScaleX = scaley;
                scale.ScaleY = scaley;
            }
            else
            {
                scale.ScaleX = scalex;
                scale.ScaleY = scalex;
            }

        }

        /// <summary>
        /// 横幅に合わせ拡大するコマンドが発行された際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FitWidthCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ScaleTransform scale = (ScaleTransform)trans.Children[0];

            double x = scrollviewer1.ActualWidth / imageWidth;
            double y = scrollviewer1.ActualWidth / imageWidth;

            scale.ScaleX = x;
            scale.ScaleY = y;
            
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
            RotateTransform rotate = (RotateTransform)trans.Children[1];
            imageHandle = controller.getNextImage();
            pictureview1.Source = imageHandle;
            rotate.Angle = 0;
            imageHeight = imageHandle.PixelHeight;
            imageWidth = imageHandle.PixelWidth;
            
        }

        /// <summary>
        /// 前の画像を表示イベントが発生した際の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void getPrevImageCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RotateTransform rotate = (RotateTransform)trans.Children[1];
            imageHandle = controller.getPrevImage();
            pictureview1.Source = imageHandle;
            rotate.Angle = 0;
            imageHeight = imageHandle.PixelHeight;
            imageWidth = imageHandle.PixelWidth;
        }

        private void ShowImageTagsCmd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (imageTags.Visibility == Visibility.Hidden)
            {
                imageTags.Visibility = Visibility.Visible;
            }
            else
            {
                imageTags.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 画像編集メニュー有効化前確認
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageControll_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            // 画像が読み込まれているか?
            if (imageHandle != null)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }


    }
}
