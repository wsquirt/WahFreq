using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleHertzFromAudioFile
{
    public static class FrequenceHelper
    {
        public static bool FourierTransform()
        {
            string f0 = "1/T";
            string T = "nombre d'échantillons dans l'enregistrement";
            string fourierTransformFormula = "a0 + E[k=1;+inf](ak * cos(2 * pi * k * f0 * t) + bk * sin(2 * pi * k * f0 * t))";
            return true;
        }
    }
}
