using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApolloCipher
{
    // A simple cipher, written by Apollo.
    internal class ApolloCipherBlock
    {
        // These are intermediate representations
        public string CipherTextString = "";
        public string PlainTextString = "";

        // This is the data
        private byte[] CipherTextByteArr = new byte[32];
        private byte[] PlainTextByteArr = new byte[32];
        private byte[] PasswordByteArr = new byte[32];

        private int CryptoIterator = 0;

        private byte SecretByte1;
        private byte SecretByte2;

        private bool EncryptedBool = false;

        // Used only to generate terminating blocks
        private ApolloCipherBlock(int strLen, string password, byte SecretByte1, byte SecretByte2, bool blockEncrypted)
        {
            this.EncryptedBool = blockEncrypted;

            if (blockEncrypted)
            {
                CryptoIterator = 0;

                this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                this.CipherTextByteArr = new byte[32];
                Encoding.UTF8.GetBytes(strLen.ToString()).CopyTo(CipherTextByteArr,0);
                this.CipherTextString = strLen.ToString();

                this.PlainTextByteArr = new byte[32];
                Encoding.UTF8.GetBytes(strLen.ToString()).CopyTo(PlainTextByteArr, 0);
                this.PlainTextString = strLen.ToString();

                // Get our secrets from the chain
                this.SecretByte1 = SecretByte1;
                this.SecretByte2 = SecretByte2;

            }
            else
            {
                CryptoIterator = 0;

                this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                this.CipherTextByteArr = new byte[32];
                Encoding.UTF8.GetBytes(strLen.ToString()).CopyTo(CipherTextByteArr,0);
                this.CipherTextString = strLen.ToString();

                this.PlainTextByteArr = new byte[32];
                Encoding.UTF8.GetBytes(strLen.ToString()).CopyTo(PlainTextByteArr, 0);
                this.PlainTextString = strLen.ToString();

                // Get our secrets from the chain
                this.SecretByte1 = SecretByte1;
                this.SecretByte2 = SecretByte2;
            }

        }

        public ApolloCipherBlock(string data, string password, byte SecretByte1, byte SecretByte2, bool encryptBlock)
        {
            CryptoIterator = 0;

            this.EncryptedBool = encryptBlock;

            if (encryptBlock)
            {
                this.CipherTextByteArr = Encoding.UTF8.GetBytes(data);
                this.CipherTextString = data;

                this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                this.PlainTextByteArr = null;
                this.PlainTextString = "";
            }
            else
            {
                this.CipherTextByteArr = null;
                this.CipherTextString = "";

                this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                this.PlainTextString = data;
                this.PlainTextByteArr = Encoding.UTF8.GetBytes(data);
            }

            // Get our secrets from the chain
            this.SecretByte1 = SecretByte1;
            this.SecretByte2 = SecretByte2;
        }

        public ApolloCipherBlock(byte[] dataBytes, string password, byte SecretByte1, byte SecretByte2, bool blockEncrypted)
        {
            this.EncryptedBool = blockEncrypted;

            if (dataBytes.Length <= 32)
            {
                if (blockEncrypted)
                {
                    CryptoIterator = 0;

                    this.CipherTextByteArr = dataBytes;
                    this.CipherTextString = Encoding.UTF8.GetString(dataBytes, 0, 32);

                    this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                    this.PlainTextByteArr = new byte[32];
                    this.PlainTextString = "";

                    // Get our secrets from the chain
                    this.SecretByte1 = SecretByte1;
                    this.SecretByte2 = SecretByte2;

                }
                else
                {
                    CryptoIterator = 0;

                    this.PlainTextByteArr = dataBytes;
                    this.PlainTextString = Encoding.UTF8.GetString(dataBytes, 0, 32);

                    this.PasswordByteArr = Encoding.UTF8.GetBytes(password);

                    this.CipherTextByteArr = new byte[32];
                    this.CipherTextString = "";

                    // Get our secrets from the chain
                    this.SecretByte1 = SecretByte1;
                    this.SecretByte2 = SecretByte2;

                }

            }
            else
            {
                throw new ArgumentException("Cipher Block Constructor reports incorrect block length!");
            }
        }

        public static ApolloCipherBlock GenerateTerminatingBlock(int strLen, string password, byte Secret1, byte Secret2, bool DataEncrypted)
        {
            return new ApolloCipherBlock(strLen, password, Secret1, Secret2, DataEncrypted);
        }

        public void PrintPlainTextByteVals()
        {
            Console.WriteLine($"Current plaintext bytes are:");

            foreach (byte byteValue in PlainTextByteArr)
            {
                Console.WriteLine(byteValue.ToString() + $"-> {(char)byteValue}");
            }
        }

        public void PrintCipherTextByteVals()
        {
            Console.WriteLine($"Current ciphertext bytes are:");
            foreach (byte byteValue in CipherTextByteArr)
            {
                Console.WriteLine(byteValue.ToString() + $"-> {(char)byteValue}");
            }
        }

        public void SetPlainTextManual(string inputPlaintext)
        {
            PlainTextString = inputPlaintext;
            PlainTextByteArr = Encoding.UTF8.GetBytes(inputPlaintext);
        }

        public void SetCipherTextManual(string inputCipherText)
        {
            CipherTextString = inputCipherText;
            CipherTextByteArr = Encoding.UTF8.GetBytes(inputCipherText);

        }

        public string GetPlainTextString()
        {

            return Encoding.UTF8.GetString(PlainTextByteArr, 0, 32).TrimEnd('\0');
        }

        public string GetCipherTextString()
        {
            return Encoding.UTF8.GetString(CipherTextByteArr, 0, 32).TrimEnd('\0');
        }

        public byte[] GetCipherTextBytes()
        {
            return CipherTextByteArr;
        }

        public byte[] GetPlainTextBytes()
        {
            return PlainTextByteArr;
        }

        public void DecryptCipherTextBlockPwd()
        {
            byte[] tempArr = new byte[32];
            byte tempByte;
            byte tempPasswordByte;

            CryptoIterator = 0;
            CipherTextByteArr.CopyTo(tempArr, 0);

            // Where we undo the XORing with our password
            for (int i = 0; i < tempArr.Length; i++)
            {
                tempByte = tempArr[i];

                if (CryptoIterator < PasswordByteArr.Length)
                {
                    tempPasswordByte = PasswordByteArr[CryptoIterator];
                    tempByte = (byte)((tempByte ^ tempPasswordByte));
                    CryptoIterator++;
                }
                else
                {
                    // We reset *first*, because we assume the above "if" has already shifted the byte behind us
                    CryptoIterator = 0;
                    tempPasswordByte = PasswordByteArr[CryptoIterator];
                    tempByte = (byte)((tempByte ^ tempPasswordByte));
                }

                tempByte = (byte)(tempByte ^ SecretByte2);
                tempByte = (byte)(tempByte ^ SecretByte1);

                tempArr[i] = tempByte;
            }

            // Blank the ciphertext once we're done with it.
            CipherTextString = "";
            Array.Clear(CipherTextByteArr, 0, CipherTextByteArr.Length - 1);

            PlainTextString = Encoding.UTF8.GetString(tempArr, 0, 32);
            PlainTextByteArr = tempArr;
        }

        public void EncryptPlainTextBlockPwd()
        {
            // Lets make a cipher for fun
            // Make the temp buffer. We will make it 32-bytes. 
            byte[] tempArr = new byte[32];
            byte tempByte;
            byte tempPasswordByte;

            CryptoIterator = 0;

            PlainTextByteArr.CopyTo(tempArr, 0);

            for (int i = 0; i < tempArr.Length; i++)
            {
                tempByte = tempArr[i];

                // First, XOR with secrets 1 and 2
                tempByte = (byte)(tempByte ^ SecretByte1);
                tempByte = (byte)(tempByte ^ SecretByte2);

                // Next, XOR with password bytes
                if (CryptoIterator < PasswordByteArr.Length)
                {
                    tempPasswordByte = PasswordByteArr[CryptoIterator];
                    tempByte = (byte)((tempByte ^ tempPasswordByte));
                    CryptoIterator++;
                }
                else
                {
                    // We reset *first*, because we assume the above "if" has already shifted the byte behind us
                    CryptoIterator = 0;
                    tempPasswordByte = PasswordByteArr[CryptoIterator];
                    tempByte = (byte)((tempByte ^ tempPasswordByte));
                }

                tempArr[i] = tempByte;
            }

            PlainTextString = "";
            Array.Clear(PlainTextByteArr, 0, PlainTextByteArr.Length - 1);

            CipherTextByteArr = tempArr;
            CipherTextString = Encoding.UTF8.GetString(tempArr, 0, 32);

        }

        public void SwapPlaintextCiphertextBlock()
        {
            string tmpstr = this.PlainTextString;
            byte[] tmpbyte = this.PlainTextByteArr;

            this.PlainTextString = this.CipherTextString;
            this.PlainTextByteArr = this.CipherTextByteArr;

            this.CipherTextString = tmpstr;
            this.CipherTextByteArr = tmpbyte;
        }
    }
}
