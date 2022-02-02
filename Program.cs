using System.Collections;
class Program {
    public static void Main(string[] args) {
        Console.Write("What would you like to do?\n1) Decrypt a message\n2) Calculate a GCD\n3) Calculate a modulo\n");
        string response = Console.ReadLine();
        
        string[] nums;
        switch (response) {
            case "1":
                Decryption();
                break;
            case "2":
                Console.Write("Please enter the numbers (ex. 3, 4): ");
                nums = Console.ReadLine().Split(", ");
                Console.WriteLine(GCD(int.Parse(nums[0]), int.Parse(nums[1])));
                break;
            case "3":
                Console.Write("Please enter the numbers (ex. 7 % 2): ");
                nums = Console.ReadLine().Split(" % ");
                Console.WriteLine(Modulo(int.Parse(nums[0]), int.Parse(nums[1])));
                break;
            case "4":
                Console.Write("Enter initial bits (110): ");
                string initial_keystream = Console.ReadLine();
                Console.Write("Please enter the P values (010): ");
                string coefficients = Console.ReadLine();
                if(initial_keystream.Length != coefficients.Length)
                    Console.WriteLine("The two inputs must be the same length");
                else 
                    Linear_Shift_Register(initial_keystream, coefficients);
                break;
            default:
                Console.WriteLine("That is not an option");
                break;
        }
    }

    public static void Decryption() {
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

    public static void Linear_Shift_Register(string initial_keystream, string coefficients) {
        int[] paths = Bit_String(coefficients);

        Queue<int> registers = new Queue<int>();
        foreach(int bit in Bit_String(initial_keystream)) {
            registers.Enqueue(bit);
        }

        List<int> keystream = new List<int>();
        HashSet<string> prev_registers = new HashSet<string>();

        int i = 0;
        while(true) {
            int prev = 0;
            int max_cycle = (int) Math.Pow(2, paths.Length) - 1;
            for(int j = 0; j < paths.Length; j++) {
                prev = (paths[j] & registers.ToList()[j]) ^ prev;
            }
            keystream.Add(registers.Dequeue());
            registers.Enqueue(prev);

            List<int> register_list = registers.ToList();
            register_list.Reverse();
            string register_string = "";
            register_list.ForEach(x=>register_string += x);
            Console.WriteLine(register_string);

            if(!prev_registers.Add(register_string)) {
                Console.WriteLine($"Sequence repeats after {i} cycles. Max cycle length for this lfsr is {max_cycle}");
                break;
            }
            i++;
        }

        keystream.ForEach(x=>Console.Write(x));
        Console.WriteLine();
    }

    //Turns a string of bits into an array of integers for bitwise operations
    public static int[] Bit_String(string str) {
        int[] bits = new int[str.Length];

        for(int i = 0; i < str.Length; i++) {
            if(str[i] != '0' && str[i] != '1') return null;
            bits[i] = (str[i] == '1') ? 1 : 0;
        }

        return bits;
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

    public static int GCD(int a, int b) {
        if(b == 0)
            return a;
        else
            return GCD(b, Modulo(a, b));
    }
}
