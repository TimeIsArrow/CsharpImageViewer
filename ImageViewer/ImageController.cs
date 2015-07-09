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



namespace ImageViewer
{
    class ImageController
    {
        private List<String> fileList;     // ファイル名のリスト
        private static int currentNum;     //現在選択されているファイルの番号
        private static int filecount;      //ファイルの数


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
            BitmapImage bi = new BitmapImage();
            bi = getImage();
            return bi;
        }

        /// <summary>
        /// 取得したファイルリストの次の画像を参照
        /// </summary>
        /// <returns></returns>
        public BitmapImage getNextImage()
        {
            BitmapImage bi = new BitmapImage();
            if (currentNum < filecount - 1)
            {
                currentNum++;
            }
            else if (currentNum == filecount -1)
            {
                currentNum = 0;
            }
            bi = getImage();
            return bi;
        }

        /// <summary>
        /// 取得したファイルリストの前の画像を取得
        /// </summary>
        /// <returns></returns>
        public BitmapImage getPrevImage()
        {
            BitmapImage bi = new BitmapImage();
            if (currentNum > 0)
            {
                currentNum--;
            }
            else if (currentNum == 0)
            {
                currentNum = filecount - 1;
            }
            bi = getImage();
            return bi;
        }

        /// <summary>
        /// ファイル名から画像ファイルを取得
        /// </summary>
        /// <returns></returns>
        private BitmapImage getImage()
        {
            BitmapImage bi = new BitmapImage();
            FileStream fs;

            try
            {
                String filename = fileList[currentImageNum];
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = fs;
                bi.EndInit();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
            finally
            {
                
            }
            return bi;
        }

        public TransformedBitmap rotateImage()
        {
            TransformedBitmap tb = new TransformedBitmap();
            return tb;
        }
    }
}
