class Program {
    public static void Main(string[] args) {
        Console.Write("What type of cipher would you like to decrypt? ");
        string response = Console.ReadLine();
        Console.Write("Please enter the text to be decrypted: ");
        string ciphertext = Console.ReadLine();

        if(response == "shift") Shift_Decryption(ciphertext);
        else if (response == "affine") Affine_Decrpytion(ciphertext);
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

    public static void Affine_Decrpytion(string ciphertext) {
        Console.Write("What is b? ");
        int b = int.Parse(Console.ReadLine());
        Console.Write("What is a inverse? ");
        int a = int.Parse(Console.ReadLine());

        string plaintext = "";

        ciphertext.ToList().ForEach(x=>{
            plaintext += (char) (Modulo((a * ((x-97)-b)), 26) + 97);
        });

        Console.WriteLine(plaintext);
    }

    public static int Modulo(int a, int b) {
        return ((a % b) + b) % b;
    }
}
