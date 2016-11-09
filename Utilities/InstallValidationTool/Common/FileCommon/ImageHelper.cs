using System.Drawing;
using System.IO;

namespace Common.FileCommon
{
    public static class ImageHelper
    {
        /// <summary>
        /// Image 转换为 byte[]数组
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(Image imageIn)
        {
            if (imageIn == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// byte[]数组转换为 Image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null)
                return null;

            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
        }

        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap(new Bitmap(stream));
            }
            finally
            {
                stream.Close();
            }
        }

        public static byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, Bitmap.RawFormat);
                byte[] byteImage = ms.ToArray();
                return byteImage;
            }
            finally
            {
                ms.Close();
            }
        }
    }
}
