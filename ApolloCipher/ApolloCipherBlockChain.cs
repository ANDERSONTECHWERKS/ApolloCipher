using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApolloCipher
{
    // It's a linkedList that we chain cipherblocks with
    class ApolloCipherBlockChain
    {
        private ApolloCipherBlock Terminator;

        private ApolloCipherBlock blockValue;
        private ApolloCipherBlockChain? next;
        private ApolloCipherBlockChain? prev;

        // Secret Bytes used for XORing to add confusion
        private byte SecretByte1 = 69;
        private byte SecretByte2 = 66;

        // Terminating byte will be SecretByte1 ^ SecretByte2, eventually. If this is ever zero: Something has gone wrong.
        private bool ScriptEncrypted = false;

        public ApolloCipherBlockChain(ApolloCipherBlock head)
        {
            blockValue = head;
            next = null;
            prev = null;
        }

        public ApolloCipherBlockChain(string plaintext, string password, byte SecretByte1, byte SecretByte2, bool ScriptEncrypted)
        {
            this.SecretByte1 = SecretByte1;
            this.SecretByte2 = SecretByte2;

            this.ScriptEncrypted = ScriptEncrypted;

            ApolloCipherBlock IterBlock;

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] tmpByteArr;
            
            int ByteIterator = 0;
            Int32 PlaintextLength = plaintextBytes.Length;

            while(ByteIterator < PlaintextLength) {

                tmpByteArr = new byte[32];

                // Check if we are about to run over the array (end of plaintext string)

                // To be clear: this first 'if' condition hit when we are *at the end of the string*, AKA: "We have gone over plaintext len"
                if (ByteIterator + 32 > PlaintextLength)
                {
                    // If true: We only copy to the end of the array
                    for (int i = 0; ByteIterator + i <= PlaintextLength - 1; i++)
                    {
                        tmpByteArr[i] = plaintextBytes[ByteIterator + i];
                    }

                    IterBlock = new ApolloCipherBlock(tmpByteArr,password,SecretByte1,SecretByte2, ScriptEncrypted);

                    AddBlockToTail(this,IterBlock);

                    // Lets create a termination block to add at the end of each chain. This block will contain the stringlength, used to truncate
                    // the decryped message.
                    ApolloCipherBlock TermBlock = ApolloCipherBlock.GenerateTerminatingBlock(PlaintextLength,password,SecretByte1,SecretByte2,ScriptEncrypted);
                    AddBlockToTail(this, TermBlock);

                    break;
                } else
                {
                    // Otherwise: Below is the normal block processing up until we reach the end.
                    // If false: We copy the entire block
                    for (int i = 0;  i <= 31; i++)
                    {
                        tmpByteArr[i] = plaintextBytes[ByteIterator + i];
                    }

                    IterBlock = new ApolloCipherBlock(tmpByteArr, password, SecretByte1, SecretByte2, ScriptEncrypted);
                    AddBlockToTail(this, IterBlock);

                    ByteIterator += 32;
                    continue;
                }
            }
        }

        public void SetSecret1(byte newSecret1)
        {
            this.SecretByte1 = newSecret1;
        }

        public void SetSecret2(byte newSecret2)
        {
            this.SecretByte2 = newSecret2;
        }

        public byte GetSecret1()
        {
            return this.SecretByte1;
        }

        public byte GetSecret2() {
            return this.SecretByte2;
        }

        public void AddBlockToTail(ApolloCipherBlockChain blockchain, ApolloCipherBlock block)
        {
            ApolloCipherBlockChain prev = blockchain;

            // Check head
            if(blockchain.blockValue == null)
            {
                blockchain.blockValue = block;
                blockchain.next = null;
                blockchain.prev = null;
                return;
            }

            // Check next
            if (blockchain.next == null)
            {
                blockchain.next = new ApolloCipherBlockChain(block);
            }
            else
            {
                // Iterate...
                while (blockchain.next != null)
                {
                    prev = blockchain;
                    blockchain = blockchain.next;
                }


                // assign new node to tail, assign 'previous' block to prior.
                blockchain.next = new ApolloCipherBlockChain(block);
                blockchain.next.prev = blockchain;
            }
        }

        public string GetChainPlainText()
        {
            ApolloCipherBlockChain tmpChain = this;
            string result = "";

            while (tmpChain.next != null)
            {
                result += tmpChain.blockValue.GetPlainTextString();

                tmpChain = tmpChain.next;

            }

            result += tmpChain.blockValue.GetPlainTextString();

            return result;
        }

        public string GetChainCipherText()
        {
            ApolloCipherBlockChain tmpChain = this;
            string result = "";

            while (tmpChain.next != null)
            {
                result += tmpChain.blockValue.GetCipherTextString();

                tmpChain = tmpChain.next;

            }

            result += tmpChain.blockValue.GetCipherTextString();

            return result;
        }

        public string EncryptChain()
        {
            ApolloCipherBlockChain tmpChain = this;
            string result = "";

            while (tmpChain.next != null)
            {
                tmpChain.blockValue.EncryptPlainTextBlockPwd();

                result += tmpChain.blockValue.GetCipherTextString();

                tmpChain = tmpChain.next;

            }

            tmpChain.blockValue.EncryptPlainTextBlockPwd();
            result += tmpChain.blockValue.GetCipherTextString();

            return result;
        }

        public string DecryptChain()
        {
            ApolloCipherBlockChain tmpChain = this;
            string TermIndexStr = "";
            int? TerminationIndex = null;
            byte[] indexBytes = new byte[4];
            byte[] TermIndexPlaintextByteArr;
            string Result = "";
            
            while (tmpChain.next != null)
            {
                
                tmpChain.blockValue.DecryptCipherTextBlockPwd();

                Result += tmpChain.blockValue.GetPlainTextString();

                tmpChain = tmpChain.next;

            }

            // At this point - we expect and check for the terminator block.

            // TODO: Figure out why the data gets mangled in this call to DecryptCipher...PWD()
            tmpChain.blockValue.DecryptCipherTextBlockPwd();

            // Once we detect where the string is *supposed* to end - copy that substring to the result.
            TermIndexPlaintextByteArr = tmpChain.blockValue.GetPlainTextBytes();

            indexBytes[0] = TermIndexPlaintextByteArr[0];
            indexBytes[1] = TermIndexPlaintextByteArr[1];
            indexBytes[2] = TermIndexPlaintextByteArr[2];
            indexBytes[3] = TermIndexPlaintextByteArr[3];

            TermIndexStr = Encoding.UTF8.GetString(indexBytes);
            try
            {
                TerminationIndex = Int32.Parse(TermIndexStr);

            }
            catch (Exception ex) {
                // We assume termination index is still null
                //TerminationIndex = null;
                Console.WriteLine("DEBUG: Decryption unable to find TerminationIndex! Decrypting entire message!");

                // This is where we drop the block off the end.
                //if(tmpChain.prev != null && tmpChain.prev.next != null)
                //{
                //    tmpChain.prev.next = null;
                //}

                // TODO: Need to figure out how to automatically drop the last block.
                // Probably need to implement next/prev behavior.
            }

            // Check if we were able to parse a termination index. If not - we'll just decrypt the whole thing.
            if (TerminationIndex != null)
            {
                Result = Result.Substring(0, TerminationIndex.Value);
                return Result;

            } else
            {
                Result += tmpChain.blockValue.GetPlainTextString();
                return Result;
            }
        }

        public void SwapCiphertextPlaintextChain()
        {
            ApolloCipherBlockChain tmpChain = this;

            while (tmpChain.next != null)
            {
                tmpChain.blockValue.SwapPlaintextCiphertextBlock();
            }
        }
    }
}
