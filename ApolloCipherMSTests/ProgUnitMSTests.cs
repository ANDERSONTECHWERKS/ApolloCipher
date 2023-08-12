using System.Diagnostics;
using ApolloCipher;

namespace ScriptLockerMSTests
{

    [TestClass]
    public class EncryptDecryptTests
    {
        string TestScript = "What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills." +
            " I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target." +
            " I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words." +
            " You think you can get away with saying that shit to me over the Internet? Think again, fucker." +
            " As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot." +
            " The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands." +
            " Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit." +
            " If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot." +
            " I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.";

        string Plaintext;
        string Ciphertext;
        string Password = "Testicles";
        string Password2 = "Butthole";

        ApollosScriptLockerProg PicProg;
        ApollosScriptLockerProg PicProg2;

        [TestMethod]
        public void TestEncryptDecrypt()
        {
            Console.WriteLine("TestEncryptDecrypt starting with the following script:\n" + TestScript);
            PicProg = new ApollosScriptLockerProg(TestScript, false,Password);
            
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

            Assert.IsTrue(Plaintext.Equals(TestScript));
        }

        [TestMethod]
        public void TestEncryptDecryptx2()
        {
            Console.WriteLine("TestEncryptDecrypt starting with the following script:\n" + TestScript);
            PicProg = new ApollosScriptLockerProg(TestScript, false, Password);

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

            Assert.IsTrue(Plaintext.Equals(TestScript));
        }

        [TestMethod]
        public void TestFileOperations()
        {
            Console.WriteLine("TestEncryptDecrypt starting with the following script:\n" + TestScript);
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

            Assert.IsTrue(Plaintext.Equals(TestScript));
        }

    }
}