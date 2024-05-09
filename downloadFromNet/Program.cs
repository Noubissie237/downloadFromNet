using System;
using System.Net;

namespace downloadFromNet
{
    class Program
    {
        public delegate string UrlValidation(string url);

        public static string PurgeUrl(string url)
        {
            return url.Trim().ToLower();
        }
        public static string ValidationUrl(string url)
        {
            url = url.Trim().ToLower();

            if (url.All(char.IsDigit))
                return "ERREUR : Vous devez entrer une url et non un nombre";

            if (string.IsNullOrWhiteSpace(url) || url.Length < 8)
                return "ERREUR : Vous devez entrer une url (https://...)";
            
            if (url[..5] != "https")
                return "ERREUR : Nous ne pouvons télécharger qu'a partir des sites sécurisés (https)";

            if (url[..8] != "https://")
                return "ERREUR : L'url doit commencer par https:// ";

            return null;
        }
        public static string ValidationInt(string choice)
        {
            if (!(choice.Any(char.IsDigit)))
                return "ERREUR : Les lettre ne sont pas autorisées, vous devez entrer un chiffre";

            if (choice.Length > 1)
                return "ERREUR : Vous devez entrer un seul chiffre";

            if (choice != "1" && choice != "2" && choice != "3" && choice != "4")
                return "ERREUR : Veuillez choisir entre 1, 2, 3 et 4 selon votre préférence";

            return null;
        }
        public static string ValidationFilename(string  filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return "Vous devez entrer un nom de fichier";
            return null;
        }
        public static string GetString(string message, UrlValidation ValidationFunction)
        {
            Console.Write(message);
            string reponse = Console.ReadLine();

            string error = ValidationFunction(reponse);

            if (error != null)
            {
                Console.WriteLine(error);
                return GetString(message, ValidationFunction);
            }
            return reponse;
        }
        public static void DownloadFromNet(string url, string filename)
        {
            var webClient = new WebClient();
            try
            {
                webClient.DownloadFile(url, filename);
                Console.WriteLine("Téléchargement terminé !");
                Console.WriteLine("Chemin : "+filename);
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                { 
                    Console.WriteLine("Verifiez votre connexion internet ou que l'url de téléchargement soit valide !");
                }

                var statusCode = (HttpWebResponse)ex.Response;
                if (statusCode.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("ERREUR : Fichier non trouvé");
                }
            }
        }
        public static void Main(string[] args)
        {
            Console.WriteLine(" --------- TELECHARGEZ EN LIGNE --------- ");
            Console.WriteLine();

            Console.WriteLine("Que souhaitez vous télécharger ? ");
            Console.WriteLine("\t1.Un document (pdf) \t2.Une Image \t3.Une vidéo. \t4.Une musique");
            string choicePath = GetString("Choix : ", ValidationInt);

            string path = "";
            
            switch (choicePath)
            {
                case "1":
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); break;
                case "2":
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); break;
                case "3":
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos); break;
                case "4":
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); break;

                default:
                    Console.WriteLine("Une erreur inconnue est survenue"); break;
            }

            string url = PurgeUrl(GetString("Entrez l'url : ", ValidationUrl));
            Console.WriteLine();

            string tmpname = "";
            string filename = "";

            while(true)
            {
                tmpname = GetString("Entrez le nom du fichier :  ", ValidationFilename);

                if (choicePath == "1")
                    tmpname += ".pdf";
                else if (choicePath == "2")
                    tmpname += ".jpg";
                else if (choicePath == "3")
                    tmpname += ".mp4";
                else
                    tmpname += ".mp3";

                filename = Path.Combine(path, tmpname);

                if (!File.Exists(filename))
                    break;
                Console.WriteLine($"Un fichier nommé {filename} existe déjà, veuillez donner un autre nom");
            }

            Console.Clear();
            Console.WriteLine("Téléchagement en cours...");
            Console.WriteLine();

            DownloadFromNet(url, filename);

        }
    }
}