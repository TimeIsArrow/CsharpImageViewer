using System;
using System.IO;    //FileStream
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;      //OpenFileDialog
using System.Diagnostics;   //Debug



namespace ImageViewer
{
    class ImageController
    {
        private List<String> fileList;          // ファイル名のリスト
        private static int currentNum;          // 現在選択されているファイルの番号
        private static int filecount;           // ファイルの数
        private static double scalex;
        private static double scaley;

        private TransformGroup transgroup;      // 画面の拡大・縮小・回転のためのTransformクラスのグループ
        private BitmapImage image;              // 画像表示のためのBitmapImage
        private TransformedBitmap transimg;     // Transformした画像を格納するためのTransformedBitmap
        private RotateTransform rotate;
        private ScaleTransform scale;


        /// <summary>
        /// プロパティ
        /// </summary>
       
        // 現在選択されているファイルの番号
        public int currentImageNum
        {
            get { return currentNum; }
          
        }
        // ファイルの数
        public int fileCount
        {
            get { return filecount; }
        }

        // 現在の画像の倍率
        public double imageScaleX
        {
            get { return scale.ScaleX; }
        }
        public double imageScaleY
        {
            get { return scale.ScaleY; }
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="filelist">ファイルリストのstring配列</param>
        public ImageController(string[] filelist)
        {
            fileList = new List<string>();
            transgroup = new TransformGroup();
            rotate = new RotateTransform();
            scale = new ScaleTransform();
            transgroup.Children.Add(rotate);
            transgroup.Children.Add(scale);

            // メンバの初期化
            currentNum = 0;
            filecount = 0;
            scalex = 0;
            scaley = 0;

            foreach (string filename in filelist)
            {
                fileList.Add(filename);
            }
            //ファイル数を取得
            filecount = fileList.Count;
        }

        public BitmapImage Init() {
            String filename = fileList[currentImageNum];
            FileStream fs;

            fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            
            image = getImage();
            return image;
        }

        /// <summary>
        /// 取得したファイルリストの次の画像を参照
        /// </summary>
        /// <returns></returns>
        public BitmapImage getNextImage()
        {
            if (currentNum < filecount - 1)
            {
                currentNum++;
            }
            else if (currentNum == filecount -1)
            {
                currentNum = 0;
            }
            image = getImage();
            return image;
        }

        /// <summary>
        /// 取得したファイルリストの前の画像を取得
        /// </summary>
        /// <returns></returns>
        public BitmapImage getPrevImage()
        {
            if (currentNum > 0)
            {
                currentNum--;
            }
            else if (currentNum == 0)
            {
                currentNum = filecount - 1;
            }
            image = getImage();
            return image;
        }

        /// <summary>
        /// ファイル名から画像ファイルを取得
        /// </summary>
        /// <returns></returns>
        private BitmapImage getImage()
        {
            image = new BitmapImage();
            try
            {
                String filename = fileList[currentImageNum];

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.None;
                image.UriSource = new Uri(fileList[currentNum], UriKind.RelativeOrAbsolute);
                image.EndInit();
                image.Freeze();

                Debug.WriteLine(image.UriSource);
                Debug.WriteLine(fileList[currentImageNum]);
                Debug.WriteLine("pixel width before:" + image.PixelWidth);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
            finally
            {

            }
            return image;
        }

        /// <summary>
        /// 画像を回転させる
        /// </summary>
        /// <returns></returns>
        public TransformedBitmap rotateImage(bool isLeft)
        {
            transimg = new TransformedBitmap();
            transimg.BeginInit();
            
            if (isLeft)
            {
                rotate.Angle -= 90;
            }
            else
            {
            rotate.Angle += 90;
            }

            if (rotate.Angle == 360 || rotate.Angle == -360)
            {
                rotate.Angle = 0;
            }

            transimg.Source = image;
            transimg.Transform = transgroup;
            transimg.EndInit();
            Debug.WriteLine("rotate"+ rotate.Angle);
            return transimg;
        }

        /// <summary>
        /// 画像を拡大する
        /// </summary>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <returns></returns>
        public TransformedBitmap scaleImage(double scaleX, double scaleY)
        {
            transimg = null;
            if (transimg == null) { 
            transimg = new TransformedBitmap();
            transimg.BeginInit();
                transimg.Source = image;
                transimg.EndInit();
            }
            //transimg.BeginInit();
            if (scalex >= 0)
            {
                scalex += scaleX;
                scaley += scaleY;
            }
            else
            {
                scalex = 1.0;
                scaley = 1.0;
            }
            scale.ScaleX = scalex;
            scale.ScaleY = scaley;
            //transimg.Source = image;

            transimg.Transform = transgroup;
            //transimg.EndInit();
            
            Debug.WriteLine("x:" + scale.ScaleX);
            Debug.WriteLine("y:" + scale.ScaleY);
            Debug.WriteLine("pixel width after:" + transimg.PixelWidth);
            return transimg;
        }
    }
}
