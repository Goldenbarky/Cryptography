using System.Collections;
class Program {
    public static void Main(string[] args) {
        Console.Write("What type of cipher would you like to decrypt? ");
        string response = Console.ReadLine();
        Console.Write("Please enter the text to be decrypted: ");
        string ciphertext = Console.ReadLine();

        if(response == "shift") Shift_Decryption(ciphertext);
        if(response == "lfsr") Linear_Shift_Register(ciphertext);
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
        string coefficients = Console.ReadLine();
        int[] paths = Bit_String(coefficients);

        
        Queue<int> registers = new Queue<int>();
        foreach(int bit in Bit_String(initial_keystream)) {
            registers.Enqueue(bit);
        }

        List<int> keystream = new List<int>();

        for(int i = 0; i < Math.Pow(2, initial_keystream.Length); i++) {
            int prev = 0;
            for(int j = 0; j < paths.Length; j++) {
                prev = (paths[j] & registers.ToList()[i]) ^ prev;
            }
            keystream.Add(registers.Dequeue());
            registers.Enqueue(prev);

            registers.ToList().ForEach(x=>Console.Write(x));
            Console.WriteLine();
        }
    }

    //Turns a string of bits into an array of integers for bitwise operations
    public static int[] Bit_String(string str) {
        int[] bits = new int[str.Length];

        for(int i = 0; i < str.Length; i++) {
            if(str[i] != '0' && str[i] != '1') return null;
            bits[i] = (str[i] == 1) ? 1 : 0;
        }

        return bits;
    }
}
