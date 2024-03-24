// See https://aka.ms/new-console-template for more information


namespace ProgrammeringsUppgiftOmegapoint
{
    class Program
    {

        static void Main(string[] args)
        {

            if(args.Length == 0)
            {
                Console.WriteLine("Usage: ProgrammingsUppgiftOmegapoint.exe <file_path>");
                return;
            }

            string filePath = args[0];

            /*
             * Fyll i sökväg här om ni föredrar det istället för args-lösningen ovan
             */
            //String filePath = "";

            if(File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(filePath);
                String line;

                while((line = sr.ReadLine()) != null)
                {
                    //Kontroller för att skicka numret till rätt kontroll beroende på om det är
                    //ett personnr, samordningsnr eller orgnr
                    if(line.Length > 11)
                    {
                        if(Int32.Parse(line.Substring(4,1)) >= 2)
                        {
                            ValidityCheckOrg(line);
                        } else if (Int32.Parse(line.Substring(6,1)) >= 6)
                        {
                            ValidityCheckSamordning(line);
                        } else
                        {
                            ValidityCheck(line);
                        }

                    } else
                    {
                        if (Int32.Parse(line.Substring(2, 1)) >= 2)
                        {
                            ValidityCheckOrg(line);
                        }
                        else if (Int32.Parse(line.Substring(4, 1)) >= 6)
                        {
                            ValidityCheckSamordning(line);
                        }
                        else
                        {
                            ValidityCheck(line);
                        }
                    }
                }
            } else { 

                Console.WriteLine("Vänligen gå in i Program.cs (Main-metoden) och fyll i sökväg till .txt-filen");
            }
        }

        /*
         * ValidityCheck för personnummer
         */
        public static void ValidityCheck(String s)
        {
            //Kollar om annat än siffror, - eller + -tecken finns med
            if(!CheckForNumbers(s))
            {
                LogError(s, "Innehåller tecken annat än siffror, '-' & '+'");
                Console.WriteLine(s + "- Fel! innehåller något annat än siffror, - , +");
                return;
            }

            //Kollar om det är ett giltigt datum
            if(!IsValidDate(s))
            {
                //Loggningen sker i CheckForNumbers om det falerar
                Console.WriteLine(s + "- Fel! Felaktigt datum");
                return;
            }

            int controllNumber = int.Parse(s[s.Length - 1].ToString());
            //Kollar om kontrollsiffran stämmer
            if (controllNumber == ControllLength(s))
            {
                Console.WriteLine(s + " - Funkar!");
            }
            else
            {
                LogError(s, "Kontrollsiffran överensstämmer inte");
                Console.WriteLine(s + " - Fel! - Kontrollsiffran överensstämmer inte");
            }

        }

        /*
         * ValidityCheck för Samordningsnummer
         */
        public static void ValidityCheckSamordning(String s)
        {
            if (!CheckForNumbers(s))
            {
                LogError(s, "Innehåller annat än siffror, '-' & '+'");
                Console.WriteLine(s + "- Fel! innehåller något annat än siffror, - , +");
                return;
            }
  
            int controllNumber = int.Parse(s[s.Length - 1].ToString());
            if (controllNumber == ControllLength(s))
            {
                Console.WriteLine(s + " - Funkar!");
            }
            else
            {
                LogError(s, "Kontrollsiffran överensstämmer inte");
                Console.WriteLine(s + " - Fel! - Kontrollsiffran överensstämmer inte");
            }

        }

        /*
         * ValidityCheck för Organisationsnummer
         */
        public static void ValidityCheckOrg(String s)
        {
            if (!CheckForNumbers(s))
            {
                LogError(s, "Innehåller annat än siffror, '-' & '+'");
                Console.WriteLine(s + "- Fel! innehåller något annat än siffror, - , +");
                return;
            }

            //Kollare om det är ett giltigt organisationsnummer
            if(!ValidateNumber(s))
            {
                LogError(s, "Felaktigt Organisationsnummer");
                Console.WriteLine(s + "- Fel! Felaktigt organisationsnummer");
                return;
            }

            int controllNumber = int.Parse(s[s.Length - 1].ToString());
            if (controllNumber == ControllLength(s))
            {
                Console.WriteLine(s + " - Funkar!");
            }
            else
            {
                LogError(s, "Kontrollsiffran överensstämmer inte");
                Console.WriteLine(s + " - Fel! - Kontrollsiffran överensstämmer inte");
            }
        }

        //Metod för att kolla om det är ett giltigt datum
        public static bool IsValidDate(string s)
        {
            int year, month, day;
            String origin = s;

            if(s.Length > 11) 
            { 
                // Extraherar år
                year = int.Parse(s.Substring(0, 4));
                s = s.Substring(4); // Tar bort siffrorna som tilldelas år
            } else
            {
                //Funkar endast för 1900-talet 
                year = int.Parse("19" + s.Substring(0,2));
                s = s.Substring(2);
            }

            // Extraherar månad
            month = int.Parse(s.Substring(0, 2));
            s = s.Substring(2);

            // Extraherar dag
            day = int.Parse(s.Substring(0, 2));


            //Kontrollerar om dagens finns eller inte
            try { 
                DateTime date = new DateTime(year, month, day);
                return true;
            } catch(Exception e)
            {
                LogError(origin, e.Message);
                return false;
            }
        }

        //Kontrollerar att allt i strängen är siffror
        //Om char är mindre än 0 eller större än 9 kollas det om n är '-' eller '+'
        public static bool CheckForNumbers(string pNumber)
        {
            foreach (char n in pNumber)
            {
                if (n < '0' || n > '9')
                {
                    if (n == '-' || n == '+')
                        continue;


                    return false;
                }
            }

            return true;

        }

        /*
         * Används för att kontrollera längden på en sträng. Om längre än 10 (alltså 12-siffrigt personnummer eller
         *      har specialtecken) så kommer dessa plockas bort innan de skickas till Luhns Algoritm. 
         *      Plockar även bort de första 2 siffrorna i året om det är YYYY
         */
        public static int ControllLength(String s)
        {
            if(s.Length > 10)
            {
                if (s.Contains('-') || s.Contains('+'))
                {
                    s = RemoveSpecialCharacters(s);
                    if (s.Length > 10)
                    {
                        s = RemoveHundreds(s);
                    }
                } 
                else
                {
                    s = RemoveHundreds(s);
                }
            } 
            else
            {
                if(s.Contains('-') || s.Contains('+'))
                {
                    s = RemoveSpecialCharacters(s);
                }
            }
            s = s.Remove(s.Length - 1);

            return LuhnsAlgorithm(s);
        }

        //Tar bort bindel-sträck eller '+'-tecken från strängen 
        public static String RemoveSpecialCharacters(String s)
        {
            if (s.Contains("-"))
                return s.Replace("-", "");
            else
                return s.Replace("+", "");
        }

        //Tar bort 18 , 19, 20 eller 16 i början av strängen
        public static String RemoveHundreds(String s)
        {
            if(s.StartsWith("18") || s.StartsWith("19") || s.StartsWith("20") || s.StartsWith("16"))
                return s.Remove(0,2);
            return s;
        }

        //Validerar om det är ett giltigt organisationsnummer (mittersta siffrorna är större eller lika med 20)
        public static bool ValidateNumber(String s)
        {
            int number;
            if(s.Substring(0,2).Equals("16"))
            {
                number = int.Parse(s.Substring(4, 2));
            } else
            {
                number = int.Parse(s.Substring(2, 2));
            }
            
            if (number >= 20)
                return true;

            return false;
        }

        /*
         * Luhns Algoritm   
         */
        public static int LuhnsAlgorithm(String personNumber)
        {
            int sum = 0;
            bool isSecond = false;

            for (int i = 0; i <= personNumber.Length-1; i++)
            {
                int temp = personNumber[i] - '0';

                if (!isSecond)
                {
                    temp *= 2;
                }

                sum += temp / 10;
                sum += temp % 10;

                isSecond = !isSecond;
            }

            return (10 - (sum % 10)) % 10;
        }

        /*
         * Metod för loggning - filen återfinns under: bin -> Debug -> net6.0 -> "error_log.txt"
         * Loggningen rensas atm inte, utan fylls bara på med nya fel
         */
        public static void LogError(String number, String errorMessage)
        {
            String logEntry = $"{DateTime.Now}: {number} - {errorMessage}";
            String logFilePath = "error_log.txt";
            using (StreamWriter writer = new StreamWriter(logFilePath, true)) 
            {
                writer.WriteLine(logEntry);
            }
        }
    }
}

