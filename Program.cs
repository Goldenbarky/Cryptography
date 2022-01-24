using System.Collections;
class Program {
    public static void Main(string[] args) {
        Console.Write("What type of cipher would you like to decrypt? ");
        string response = Console.ReadLine();
        Console.Write("Please enter the text to be decrypted: ");
        string ciphertext = Console.ReadLine();

        if(response == "shift") Shift_Decryption(ciphertext);
    }

    public static void Shift_Decryption(string ciphertext) {
        for(int i = 1; i < 26; i++) {
            string plaintext = "";

            ciphertext.ToList().ForEach(x=>{
                if((int)x == 32) plaintext += x;
                else plaintext += (char)(x+i-122+96);
            });

            Console.WriteLine($"Attempt {i}: {plaintext}");
        }
    }

    public static void Linear_Shift_Register(string initial_keystream) {
        Console.Write("Please enter the P values (010): ");
        string coefficients = Console.Read();
        bool[] paths = Bit_String(coefficients);

        Stack<bool> registers = new Stack<bool>();
        foreach(bool bit in Bit_String(initial_keystream)) {
            registers.Push(bit);
        }

        string keystream;

        for(int i = 0; i < Math.Pow(2, initial_keystream.Length); i++) {
            keystream += (registers.Peek);


        }
    }

    public static bool[] Bit_String(string str) {
        bool[] bits = new bool[str.Length];

        for(int i = 0; i < str.Length; i++) {
            if(str[i] != "0" && str[i] != "1") return null;
            bits[i] = (str[i] == 1) ? true : false;
        }

        return bits;
    }
}
