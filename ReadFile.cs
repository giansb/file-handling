using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileHandling
{
    internal class ReadFile
    {

        public void ReadTxtFile(string fileName)
        {  

            long totalInput = 0;
            int totalErrors = 0;

            string regexText = @"\b\d{2}:\d{2}:\d{2}\b";
            Regex regex = new Regex(regexText);

            string firstTime = "";
            string lastTime = "";
            string outputTime = "";

            string directoryBase = AppDomain.CurrentDomain.BaseDirectory; // endereco de onde o app esta sendo executado
            string directoryProject = Directory.GetParent(directoryBase).Parent.Parent.Parent.FullName; // diretorio pai de directoryBase
            string pathFile = Path.Combine(directoryProject, "File", fileName); // caminho do arquivo

            static bool IsTxtFile(string filePath)
            {
                return Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase);
            }

            try
            {
                if(!File.Exists(pathFile))
                {
                    throw new ArgumentException("Erro: Arquivo não foi encontrado.");
                }

                if(!IsTxtFile(pathFile))
                {
                    throw new ArgumentException("Erro: Formato do arquivo inválido.");
                }

                using(var fileS = new FileStream(pathFile,FileMode.Open))
                {

                    var fileReader = new StreamReader(fileS);

                    if(fileReader.Peek() == -1)
                    {
                        throw new ArgumentException("Erro: Arquivo em branco.");
                    }

                    while(!fileReader.EndOfStream)
                    {
                        string line = fileReader.ReadLine();          

                            if (line != "" && line != null)
                            {
                                Match match = regex.Match(line);
                                totalInput++;

                                if (!match.Success)
                                {
                                    throw new ArgumentException($"Erro: formato para hora 00:00:00 não encontrado na linha {totalInput-1} do arquivo.");
                                }

                                if (line != "" && firstTime == "")
                                {
                                    firstTime = match.Value;
                                }

                                if (line.Contains("ERRO"))
                                {
                                    totalErrors++;
                                }

                                lastTime = match.Value;
                            }

                        if(line != "" && line != null)
                        {
                            var formatLastTime = DateTime.ParseExact(lastTime, "ss:mm:HH", null);
                            var formatFirstTime = DateTime.ParseExact(firstTime, "ss:mm:HH", null);

                            if (formatLastTime < formatFirstTime)
                            {
                                throw new ArgumentException("Erro: Horários/linhas fora de ordem.");
                            }

                            var time = formatLastTime - formatFirstTime;
                            int hours = time.Hours;
                            int minutes = time.Minutes;
                            int seconds = time.Seconds;

                            outputTime = $"{minutes} minutos";

                            if (hours > 0)
                            {
                                outputTime = $"{hours} horas, {minutes}";
                            }

                            if (seconds > 0)
                            {
                                outputTime = $"{outputTime} e {seconds} segundos";
                            }
                        }

                    }
                    
                }
                Console.WriteLine($"Número total de entradas de log: {totalInput}");
                Console.WriteLine($"Número total de erros: {totalErrors}");
                Console.WriteLine($"Tempo total decorrido: {outputTime}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
