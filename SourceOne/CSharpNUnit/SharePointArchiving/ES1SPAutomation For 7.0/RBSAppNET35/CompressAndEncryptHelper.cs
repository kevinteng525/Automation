using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EMC.SourceOne.RBSProvider.ProviderConfig;
using System.IO;

namespace RBSAppNET35
{
    public static class CompressAndEncryptHelper
    {
        public const int DefaultChunkSize = 0x8000; // do not change it!
        public const int DefaultBufferSize = 0x8000;

        public static void CompressAndEncryptFile(string inFile, bool isCompressed, EncryptionType encryptType, ref string outFile)
        {
            if (isCompressed == false && encryptType == EncryptionType.None)
            {
                File.Copy(inFile, outFile, true);
            }
            else
            {
                using (EMC.SourceOne.RBSProvider.ProviderLibrary.RBSBlob.RBSBlobWriterStream writerStream =
                    new EMC.SourceOne.RBSProvider.ProviderLibrary.RBSBlob.RBSBlobWriterStream(outFile, new Microsoft.Data.BlobStores.BlobInformation(),
                            isCompressed ? EMC.SourceOne.RBSProvider.ProviderConfig.CompressionType.DeflateStream : EMC.SourceOne.RBSProvider.ProviderConfig.CompressionType.None,
                            (EMC.SourceOne.RBSProvider.ProviderConfig.EncryptionType)((short)encryptType), DefaultChunkSize, DefaultBufferSize, "P@ssw0rd"))
                {
                    using (FileStream inputStream = new FileStream(inFile, FileMode.Open, FileAccess.Read))
                    {
                        inputStream.Copy(writerStream, DefaultBufferSize);
                        writerStream.Commit();

                        inputStream.Close();
                        writerStream.Close();
                    }
                }
            }
        }

        public static void DecryptAndDecompressFile(string inFile, bool isCompressed, EncryptionType encryptType, ref string outFile)
        {
            if (isCompressed == false && encryptType == EncryptionType.None)
            {
                File.Copy(inFile, outFile, true);
            }
            else
            {
                using (EMC.SourceOne.RBSProvider.ProviderLibrary.RBSBlob.RBSBlobReaderStream readerStream = new EMC.SourceOne.RBSProvider.ProviderLibrary.RBSBlob.RBSBlobReaderStream(inFile, 0x8000, "P@ssw0rd"))
                {
                    using (FileStream outputStream = File.Open(outFile, FileMode.Create, FileAccess.ReadWrite))
                    {
                        readerStream.Copy(outputStream, DefaultBufferSize);

                        outputStream.Close();
                        readerStream.Close();
                    }
                }
            }
        }

        public static void Copy(this Stream source, Stream destination, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int read = 0;
            while ((read = source.Read(buffer, 0, bufferSize)) > 0)
            {
                destination.Write(buffer, 0, read);
            }
        }
    }
}
