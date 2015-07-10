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
        private List<String> fileList;     // ファイル名のリスト
        private static int currentNum;     //現在選択されているファイルの番号
        private static int filecount;      //ファイルの数

        private TransformedBitmap transform;
        private static BitmapImage image;
        private static int rotatedeg;
        private static double imageScaleX;
        private static double imageScaleY;


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

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="filelist">ファイルリストのstring配列</param>
        public ImageController(string[] filelist)
        {
            fileList = new List<string>();
            transform = new TransformedBitmap();

            // メンバの初期化
            currentNum = 0;
            filecount = 0;

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
                image.CacheOption = BitmapCacheOption.None;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(fileList[currentNum], UriKind.RelativeOrAbsolute);
                image.EndInit();

                Debug.WriteLine(image.UriSource);
                Debug.WriteLine(fileList[currentImageNum]);
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

        public TransformedBitmap rotateImage()
        {
            TransformedBitmap tb = new TransformedBitmap();
            return tb;
        }
    }
}
