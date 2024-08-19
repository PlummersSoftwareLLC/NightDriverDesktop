using System.IO.Compression;
namespace NightDriver
{
    public sealed class ZLIBStream : Stream
    {
        #region "Variables globales"
        private CompressionMode mCompressionMode = CompressionMode.Compress;
        private CompressionLevel mCompressionLevel = CompressionLevel.NoCompression;
        private bool mLeaveOpen = false;
        private Adler32 adler32 = new Adler32();
        private DeflateStream mDeflateStream;
        private Stream mRawStream;
        private bool mClosed = false;
        private byte[] mCRC = null;
        #endregion
        #region "Constructores"
        /// <summary>
        /// Inicializa una nueva instancia de la clase ZLIBStream usando la secuencia y nivel de compresión especificados.
        /// </summary>
        /// <param name="stream">Secuencia que se va a comprimir</param>
        /// <param name="compressionLevel">Nivel de compresión</param>
        public ZLIBStream(Stream stream, CompressionLevel compressionLevel) : this(stream, compressionLevel, false)
        {
        }
        /// <summary>
        /// Inicializa una nueva instancia de la clase ZLIBStream usando la secuencia y modo de compresión especificados.
        /// </summary>
        /// <param name="stream">Secuencia que se va a comprimir o descomprimir</param>
        /// <param name="compressionMode">Modo de compresión</param>
        public ZLIBStream(Stream stream, CompressionMode compressionMode) : this(stream, compressionMode, false)
        {
        }
        /// <summary>
        /// Inicializa una nueva instancia de la clase ZLIBStream usando la secuencia y nivel de compresión especificados y, opcionalmente, deja la secuencia abierta.
        /// </summary>
        /// <param name="stream">Secuencia que se va a comprimir</param>
        /// <param name="compressionLevel">Nivel de compresión</param>
        /// <param name="leaveOpen">Indica si se debe de dejar la secuencia abierta después de comprimir la secuencia</param>
        public ZLIBStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen)
        {
            mCompressionMode = CompressionMode.Compress;
            mCompressionLevel = compressionLevel;
            mLeaveOpen = leaveOpen;
            mRawStream = stream;
            InicializarStream();
        }
        /// <summary>
        /// Inicializa una nueva instancia de la clase ZLIBStream usando la secuencia y modo de compresión especificados y, opcionalmente, deja la secuencia abierta.
        /// </summary>
        /// <param name="stream">Secuencia que se va a comprimir o descomprimir</param>
        /// <param name="compressionMode">Modo de compresión</param>
        /// <param name="leaveOpen">Indica si se debe de dejar la secuencia abierta después de comprimir o descomprimir la secuencia</param>
        public ZLIBStream(Stream stream, CompressionMode compressionMode, bool leaveOpen)
        {
            mCompressionMode = compressionMode;
            mCompressionLevel = CompressionLevel.Fastest;
            mLeaveOpen = leaveOpen;
            mRawStream = stream;
            InicializarStream();
        }
        #endregion
        #region "Propiedades sobreescritas"
        public override bool CanRead
        {
            get
            {
                return mCompressionMode == CompressionMode.Decompress && mClosed != true;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return mCompressionMode == CompressionMode.Compress && mClosed != true;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion
        #region "Metodos sobreescritos"
        public override int ReadByte()
        {
            int result = 0;

            if (CanRead == true)
            {
                result = mDeflateStream.ReadByte();

                //Comprobamos si se ha llegado al final del stream
                if (result == -1)
                {
                    ReadCRC();
                }
                else
                {
                    adler32.Update(Convert.ToByte(result));
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = 0;

            if (CanRead == true)
            {
                result = mDeflateStream.Read(buffer, offset, count);

                //Comprobamos si hemos llegado al final del stream
                if (result < 1 && count > 0)
                {
                    ReadCRC();
                }
                else
                {
                    adler32.Update(buffer, offset, result);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        public override void WriteByte(byte value)
        {
            if (CanWrite == true)
            {
                mDeflateStream.WriteByte(value);
                adler32.Update(value);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (CanWrite == true)
            {
                mDeflateStream.Write(buffer, offset, count);
                adler32.Update(buffer, offset, count);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override void Close()
        {
            if (mClosed == false)
            {
                mClosed = true;
                if (mCompressionMode == CompressionMode.Compress)
                {
                    Flush();
                    mDeflateStream.Close();

                    mCRC = BitConverter.GetBytes(adler32.GetValue());

                    if (BitConverter.IsLittleEndian == true)
                    {
                        Array.Reverse(mCRC);
                    }

                    mRawStream.Write(mCRC, 0, mCRC.Length);
                }
                else
                {
                    mDeflateStream.Close();
                    if (mCRC == null)
                    {
                        ReadCRC();
                    }
                }

                if (mLeaveOpen == false)
                {
                    mRawStream.Close();
                }
            }
            else
            {
                //throw new InvalidOperationException("Stream already closed");
            }
        }

        public override void Flush()
        {
            if (mDeflateStream != null)
            {
                mDeflateStream.Flush();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region "Metodos publicos"
        /// <summary>
        /// Comprueba si el stream esta en formato ZLib
        /// </summary>
        /// <param name="stream">Stream a comprobar</param>
        /// <returns>Retorna True en caso de que el stream sea en formato ZLib y False en caso contrario u error</returns>
        public static bool IsZLibStream(Stream stream)
        {
            bool bResult = false;
            int CMF = 0;
            int Flag = 0;
            ZLibHeader header;

            //Comprobamos si la secuencia esta en la posición 0, de no ser así, lanzamos una excepción
            if (stream.Position != 0)
            {
                throw new ArgumentOutOfRangeException("Sequence must be at position 0");
            }

            //Comprobamos si podemos realizar la lectura de los dos bytes que conforman la cabecera
            if (stream.CanRead == true)
            {
                CMF = stream.ReadByte();
                Flag = stream.ReadByte();
                try
                {
                    header = ZLibHeader.DecodeHeader(CMF, Flag);
                    bResult = header.IsSupportedZLibStream;
                }
                catch
                {
                    //Nada
                }
            }

            return bResult;
        }
        /// <summary>
        /// Lee los últimos 4 bytes del stream ya que es donde está el CRC
        /// </summary>
        private void ReadCRC()
        {
            mCRC = new byte[4];
            mRawStream.Seek(-4, SeekOrigin.End);
            if (mRawStream.Read(mCRC, 0, 4) < 4)
            {
                throw new EndOfStreamException();
            }

            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(mCRC);
            }

            uint crcAdler = adler32.GetValue();
            uint crcStream = BitConverter.ToUInt32(mCRC, 0);

            if (crcStream != crcAdler)
            {
                throw new Exception("CRC mismatch");
            }
        }
        #endregion
        #region "Metodos privados"
        /// <summary>
        /// Inicializa el stream
        /// </summary>
        private void InicializarStream()
        {
            switch (mCompressionMode)
            {
                case CompressionMode.Compress:
                    {
                        InicializarZLibHeader();
                        mDeflateStream = new DeflateStream(mRawStream, mCompressionLevel, true);
                        break;
                    }
                case CompressionMode.Decompress:
                    {
                        if (IsZLibStream(mRawStream) == false)
                        {
                            throw new InvalidDataException();
                        }
                        mDeflateStream = new DeflateStream(mRawStream, CompressionMode.Decompress, true);
                        break;
                    }
            }
        }
        /// <summary>
        /// Inicializa el encabezado del stream en formato ZLib
        /// </summary>
        private void InicializarZLibHeader()
        {
            byte[] bytesHeader;

            //Establecemos la configuración de la cabecera
            ZLibHeader header = new ZLibHeader();

            header.CompressionMethod = 8; //Deflate
            header.CompressionInfo = 7;

            header.FDict = false; //Sin diccionario
            switch (mCompressionLevel)
            {
                case CompressionLevel.NoCompression:
                    {
                        header.FLevel = FLevel.Faster;
                        break;
                    }
                case CompressionLevel.Fastest:
                    {
                        header.FLevel = FLevel.Default;
                        break;
                    }
                case CompressionLevel.Optimal:
                    {
                        header.FLevel = FLevel.Optimal;
                        break;
                    }
            }

            bytesHeader = header.EncodeZlibHeader();

            mRawStream.WriteByte(bytesHeader[0]);
            mRawStream.WriteByte(bytesHeader[1]);
        }
        #endregion
    }
}
