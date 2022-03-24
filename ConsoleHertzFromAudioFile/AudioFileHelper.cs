using System;
using System.IO;
using System.Media;

/// <summary>
/// Summary description for AudioFileHelper
/// </summary>
public static class AudioFileHelper
{
    // looking for free lib to open any audio file type
    // i don't want to have a only-supported-windows library

    // [Bloc de déclaration d'un fichier au format WAVE]
    public static int ChunkID { get; set; } // (4 octets) : Constante « RIFF »  (0x52,0x49,0x46,0x46)
    public static int FileSize { get; set; } // (4 octets) : Taille du fichier moins 8 octets
    public static int RiffType { get; set; } // (4 octets) : Format = « WAVE »  (0x57,0x41,0x56,0x45)
    // [Bloc décrivant le format audio]
    public static int FmtID { get; set; } // (4 octets) : Identifiant « fmt␣ »  (0x66,0x6D, 0x74,0x20)
    public static int FmtSize { get; set; } // (4 octets) : Nombre d'octets du bloc - 16  (0x10)
    public static int FmtCode { get; set; } // (2 octets) : Format du stockage dans le fichier (1: PCM entier, 3: PCM flottant, 65534: WAVE_FORMAT_EXTENSIBLE) 
    public static int Channels { get; set; } // (2 octets) : Nombre de canaux (de 1 à 6, cf. ci-dessous)
    public static int SampleRate { get; set; } // (4 octets) : Fréquence d'échantillonnage (en hertz) [Valeurs standardisées : 11 025, 22 050, 44 100 et éventuellement 48 000 et 96 000]
    public static int ByteRate { get; set; } // (4 octets) : Nombre d'octets à lire par seconde (c.-à-d., Frequence * BytePerBloc)
    public static int FmtBlockAlign { get; set; } // (2 octets) : Nombre d'octets par bloc d'échantillonnage (c.-à-d., tous canaux confondus : NbrCanaux * BitsPerSample/8)
    public static int BitDepth { get; set; } // (2 octets) : Nombre de bits utilisés pour le codage de chaque échantillon (8, 16, 24)
    // [Bloc des données]
    public static int DataID { get; set; } // (4 octets) : Constante « data »  (0x64,0x61,0x74,0x61)
    public static int Bytes { get; set; } // (4 octets) : Nombre d'octets des données (c.-à-d. "Data[]", c.-à-d. taille_du_fichier - taille_de_l'entête  (qui fait 44 octets normalement)

    // https://stackoverflow.com/a/34667370
    public static bool ReadWav(string filename, out float[] L, out float[] R)
    {
        L = R = null;

        try
        {
            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                BinaryReader reader = new BinaryReader(fs);

                // chunk 0
                ChunkID = reader.ReadInt32();
                FileSize = reader.ReadInt32();
                RiffType = reader.ReadInt32();

                // chunk 1
                FmtID = reader.ReadInt32();
                FmtSize = reader.ReadInt32(); // bytes for this chunk (expect 16 or 18)

                // 16 bytes coming...
                FmtCode = reader.ReadInt16();
                Channels = reader.ReadInt16();
                SampleRate = reader.ReadInt32();
                ByteRate = reader.ReadInt32();
                FmtBlockAlign = reader.ReadInt16();
                BitDepth = reader.ReadInt16();

                if (FmtSize == 18)
                {
                    // Read any extra values
                    int fmtExtraSize = reader.ReadInt16();
                    reader.ReadBytes(fmtExtraSize);
                }

                // chunk 2 
                DataID = reader.ReadInt32();
                Bytes = reader.ReadInt32();

                // Small check if my readInts were good
                if (!(Bytes == FileSize - ((FmtSize == 18 ? 2 : 0) + 44)))
                    return false;

                // DATA!
                byte[] byteArray = reader.ReadBytes(Bytes);

                int bytesForSamp = BitDepth / 8;
                int nValues = Bytes / bytesForSamp;


                float[] asFloat = null;
                switch (BitDepth)
                {
                    case 64:
                        double[] asDouble = new double[nValues];
                        Buffer.BlockCopy(byteArray, 0, asDouble, 0, Bytes);
                        asFloat = Array.ConvertAll(asDouble, e => (float)e);
                        break;
                    case 32:
                        asFloat = new float[nValues];
                        Buffer.BlockCopy(byteArray, 0, asFloat, 0, Bytes);
                        break;
                    case 16:
                        Int16[] asInt16 = new Int16[nValues];
                        Buffer.BlockCopy(byteArray, 0, asInt16, 0, Bytes);
                        asFloat = Array.ConvertAll(asInt16, e => e / (float)(Int16.MaxValue + 1));
                        break;
                    default:
                        return false;
                }

                switch (Channels)
                {
                    case 1:
                        L = asFloat;
                        R = null;
                        return true;
                    case 2:
                        // de-interleave
                        int nSamps = nValues / 2;
                        L = new float[nSamps];
                        R = new float[nSamps];
                        for (int s = 0, v = 0; s < nSamps; s++)
                        {
                            L[s] = asFloat[v++];
                            R[s] = asFloat[v++];
                        }
                        return true;
                    default:
                        return false;
                }
            }
        }
        catch
        {
            Console.WriteLine("...Failed to load: " + filename);
            return false;
        }
    }
}
