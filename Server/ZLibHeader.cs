using System;

namespace NightDriver
{
    public enum FLevel
    {
        Faster = 0,
        Fast = 1,
        Default = 2,
        Optimal = 3,
    }
    public sealed class ZLibHeader
    {
        #region "Variables globales"
        private bool mIsSupportedZLibStream;
        private byte mCompressionMethod; //CMF 0-3
        private byte mCompressionInfo; //CMF 4-7
        private byte mFCheck; //Flag 0-4 (Check bits for CMF and FLG)
        private bool mFDict; //Flag 5 (Preset dictionary)
        private FLevel mFLevel; //Flag 6-7 (Compression level)
        #endregion
        #region "Propiedades"
        public bool IsSupportedZLibStream
        {
            get
            {
                return mIsSupportedZLibStream;
            }
            set
            {
                mIsSupportedZLibStream = value;
            }
        }
        public byte CompressionMethod
        {
            get
            {
                return mCompressionMethod;
            }
            set
            {
                if (value > 15)
                {
                    throw new ArgumentOutOfRangeException("Argument cannot be greater than 15");
                }
                mCompressionMethod = value;
            }
        }
        public byte CompressionInfo
        {
            get
            {
                return mCompressionInfo;
            }
            set
            {
                if (value > 15)
                {
                    throw new ArgumentOutOfRangeException("Argument cannot be greater than 15");
                }
                mCompressionInfo = value;
            }
        }
        public byte FCheck
        {
            get
            {
                return mFCheck;
            }
            set
            {
                if (value > 31)
                {
                    throw new ArgumentOutOfRangeException("Argument cannot be greater than 31");
                }
                mFCheck = value;
            }
        }
        public bool FDict
        {
            get
            {
                return mFDict;
            }
            set
            {
                mFDict = value;
            }
        }
        public FLevel FLevel
        {
            get
            {
                return mFLevel;
            }
            set
            {
                mFLevel = value;
            }
        }
        #endregion
        #region "Constructor"
        public ZLibHeader()
        {

        }
        #endregion
        #region "Metodos privados"
        private void RefreshFCheck()
        {
            byte byteFLG = 0x00;

            byteFLG = (byte)(Convert.ToByte(FLevel) << 1);
            byteFLG |= Convert.ToByte(FDict);

            FCheck = Convert.ToByte(31 - Convert.ToByte((GetCMF() * 256 + byteFLG) % 31));
        }
        private byte GetCMF()
        {
            byte byteCMF = 0x00;

            byteCMF = (byte)(CompressionInfo << 4);
            byteCMF |= CompressionMethod;

            return byteCMF;
        }
        private byte GetFLG()
        {
            byte byteFLG = 0x00;

            byteFLG = (byte)(Convert.ToByte(FLevel) << 6);
            byteFLG |= (byte)(Convert.ToByte(FDict) << 5);
            byteFLG |= FCheck;

            return byteFLG;
        }
        #endregion
        #region "Metodos publicos"
        public byte[] EncodeZlibHeader()
        {
            byte[] result = new byte[2];

            RefreshFCheck();

            result[0] = GetCMF();
            result[1] = GetFLG();

            return result;
        }
        #endregion
        #region "Metodos estáticos"
        public static ZLibHeader DecodeHeader(int pCMF, int pFlag)
        {
            ZLibHeader result = new ZLibHeader();

            //Ensure that parameters are bytes
            pCMF = pCMF & 0x0FF;
            pFlag = pFlag & 0x0FF;

            //Decode bytes
            result.CompressionInfo = Convert.ToByte((pCMF & 0xF0) >> 4);
            result.CompressionMethod = Convert.ToByte(pCMF & 0x0F);

            result.FCheck = Convert.ToByte(pFlag & 0x1F);
            result.FDict = Convert.ToBoolean(Convert.ToByte((pFlag & 0x20) >> 5));
            result.FLevel = (FLevel)Convert.ToByte((pFlag & 0xC0) >> 6);

            result.IsSupportedZLibStream = result.CompressionMethod == 8 && result.CompressionInfo == 7 && (pCMF * 256 + pFlag) % 31 == 0 && result.FDict == false;

            return result;
        }
        #endregion
    }
}
