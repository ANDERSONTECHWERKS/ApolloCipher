using System.ComponentModel.Design;
using System.Diagnostics;
using ApolloCipher;
using static System.Formats.Asn1.AsnWriter;

namespace ScriptLockerMSTests
{

    [TestClass]
    public class EncryptDecryptTests
    {
        string LitanyAgainstFear = "" +
            "I must not fear.\r\n" +
            "Fear is the mind-killer.\r\n" +
            "Fear is the little-death that brings total obliteration.\r\n" +
            "I will face my fear.\r\n" +
            "I will permit it to pass over me and through me.\r\n" +
            "And when it has gone past, I will turn the inner eye to see its path.\r\n" +
        "Where the fear has gone there will be nothing. Only I will remain.";

        string DefendTheArticles = "I swore an oath.To defend the articles." +
            "The articles say there is an election in seven months." +
            " Now, if you are telling me we are throwing out the law," +
            " then I am not a captain, you are not a commander," +
            " and you are not the president." +
            " And I don't owe either of you a damned explanation for anything.";

        string Plaintext;
        string Ciphertext;
        string Password = "Testicles";
        string Password2 = "Butthole";
        ApollosScriptLockerProg PicProg;
        ApollosScriptLockerProg PicProg2;

        [TestMethod]
        public void TestEncryptDecrypt()
        {
            Console.WriteLine("TestEncryptDecrypt starting with the following script:\n" + LitanyAgainstFear);
            PicProg = new ApollosScriptLockerProg(LitanyAgainstFear, false,Password);
            
            // First, check that it's not null.
            Assert.IsNotNull(PicProg);

            Debug.WriteLine($"Instantiating cipher program successful.");

            Debug.WriteLine($"Prog1 GetCurrentPlaintext at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentPlaintext());

            Debug.WriteLine($"Prog1 GetCurrentCiphertext at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentCiphertext());

            Ciphertext = PicProg.EncryptLoadedScript();
            Debug.WriteLine($"Prog1 Encrypted plaintext, returning Ciphertext:\n {Ciphertext}");

            Plaintext = PicProg.DecryptLoadedScript();
            Debug.WriteLine($"Prog1 Decrypted ciphertext, returning Plaintext:\n {Plaintext}");

            Assert.IsTrue(Plaintext.Equals(LitanyAgainstFear));
        }

        [TestMethod]
        public void TestEncryptDecryptx2()
        {
            Console.WriteLine("TestEncryptDecrypt starting with the following script:\n" + LitanyAgainstFear);
            PicProg = new ApollosScriptLockerProg(LitanyAgainstFear, false, Password);

            // Make the two encryptor/decryptors feed each other ciphertext and return it to normal.

            // First, check that it's not null.
            Assert.IsNotNull(PicProg);
            Debug.WriteLine($"Instantiating cipher program successful.");

            // Show us the status of PicProg1
            Debug.WriteLine($"GetCurrentPlaintext1 at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentPlaintext());

            PicProg.EncryptLoadedScript();

            Debug.WriteLine($"GetCurrentCiphertext1 is:\n");
            Debug.WriteLine(PicProg.GetCurrentCiphertext());


            // Instantiate PicProg2, use Ciphertext from PicProg as plaintext into PicProg2
            Debug.WriteLine($"Instantiating PicProg2 using PicProg Ciphertext as plaintext input!\n");
            Ciphertext = PicProg.GetCurrentCiphertext();

            PicProg2 = new ApollosScriptLockerProg(Ciphertext, true, Password2);

            // Next, show us the status of PicProg2
            Debug.WriteLine($"GetCurrentPlaintext2 at instantiation is:\n");
            Debug.WriteLine(PicProg2.GetCurrentPlaintext());

            Ciphertext = PicProg2.EncryptLoadedScript();

            Debug.WriteLine($"GetCurrentCiphertext2 is:\n");
            Debug.WriteLine(PicProg2.GetCurrentCiphertext());

            Plaintext = PicProg2.DecryptLoadedScript();
            Debug.WriteLine($"Decrypted ciphertext2, returning Plaintext:\n {Plaintext}");

            // Just doing this to re-load the program with new Ciphertext (which is stored in the plaintext variable)
            // This simulates someone else taking your encryptedText, decrypting it with your password, then decrypting their own.

            PicProg = new ApollosScriptLockerProg(Plaintext, true, Password);

            Plaintext = PicProg.GetCurrentPlaintext();

            Plaintext = PicProg.DecryptLoadedScript();

            Debug.WriteLine($"Decrypted ciphertext1, returning Plaintext:\n{Plaintext}");

            Assert.IsTrue(Plaintext.Equals(LitanyAgainstFear));
        }

        [TestMethod]
        public void TestFileOperations()
        {
            Console.WriteLine("TestEncryptDecrypt starting with the following script:\n" + LitanyAgainstFear);
            PicProg = new ApollosScriptLockerProg(false, "testPassword", "isyBastion.txt");

            // First, check that it's not null.
            Assert.IsNotNull(PicProg);

            Debug.WriteLine($"Instantiating cipher program successful.");

            Debug.WriteLine($"Prog1 GetCurrentPlaintext at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentPlaintext());

            Debug.WriteLine($"Prog1 GetCurrentCiphertext at instantiation is:\n");
            Debug.WriteLine(PicProg.GetCurrentCiphertext());

            Ciphertext = PicProg.EncryptLoadedScript();
            Debug.WriteLine($"Prog1 Encrypted plaintext, returning Ciphertext:\n {Ciphertext}");

            Plaintext = PicProg.DecryptLoadedScript();
            Debug.WriteLine($"Prog1 Decrypted ciphertext, returning Plaintext:\n {Plaintext}");

            Assert.IsTrue(Plaintext.Equals(LitanyAgainstFear));
        }

    }
}