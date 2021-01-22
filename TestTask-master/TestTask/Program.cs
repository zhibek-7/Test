﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            IReadOnlyStream inputStream1 = GetInputStream("D:\\text1");
            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);


            IReadOnlyStream inputStream2 = GetInputStream("D:\\text1DoubleLetter");
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);
            Console.ReadKey();

           
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
          
            stream.ResetPositionToStart();
  
            List<LetterStats> letterStats= new List<LetterStats>();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                string str =Regex.Replace(c.ToString(), @"[^a-zA-Z]+", "");

                LetterStats letter = letterStats.Find(l => l.Letter == c.ToString());

                //LetterStats letter = letterStats.Where(l => l.Letter.Contains(c.ToString()) && l.Count>0).FirstOrDefault();
                if (letter.Letter == null)
                {
                    if (str != "") {
                        letterStats.Add(new LetterStats { Letter = str, Count = 1 });
                    }
                    
                }
                else
                {
                   
                    int indexLetterStat = letterStats.IndexOf(letterStats.Single(i => i.Letter == str));
                    letter = IncStatistic(letter);
                    //letter.Count = letter.Count;
                    letterStats[indexLetterStat] = letter;
                }

            }
            stream.CloseStream();

            return letterStats;

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            List<LetterStats> letterStats = new List<LetterStats>();
           


            while (!stream.IsEof)
            {
                char[] allChar = stream.ReadAllChar();

                

                for (int i = 0; i < allChar.Length; i++)
                {
                   
                    int k = i + 1;
                   
                    if(allChar.Length!=k)
                   {
                        string c1 = allChar[i].ToString().ToLower();
                        string c2 = allChar[k].ToString().ToLower();

                        string str1 =Regex.Replace(c1, @"[^a-zA-Z]+", "");
                        string str2 = Regex.Replace(c2, @"[^a-zA-Z]+", "");
                        if (str1!=""&& str2!="") {

                            if (c1 == c2)
                            {
                              
                                LetterStats letter = letterStats.Find(l => l.Letter == c1 + c2);

                                if (letter.Letter == null)
                                {
                                    letterStats.Add(new LetterStats { Letter = c1 + c2, Count = 1 });
                                }
                                else
                                {
                                    int indexLetterStat = letterStats.IndexOf(letterStats.Single(j => j.Letter == c1 + c2));
                                    letter = IncStatistic(letter);
                                    //letter.Count = letter.Count;
                                    letterStats[indexLetterStat] = letter;

                                }
                            }
                        }
                      
                    }
                }
            }
            return letterStats;

           
            //    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
            //}

            ////return ???;

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.

            string vowels = @"[aeiouAEIOU]+"; //regular expression to match vowels
            string consonants = @"[^aeiouAEIOU]+"; //regular expression to match consonants
          
            string rgx = "";


           switch (charType)
            {
                case CharType.Consonants:
                    rgx = consonants;
                    break;
                case CharType.Vowel:
                    rgx = vowels;
                    break;
            }

            foreach (LetterStats letter in letters.ToArray())
            {
                int matches = new Regex(rgx).Matches(letter.Letter).Count;
                if (matches>0)
                {
                    letters.Remove(letter);
                }
               
            }

           


        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
           foreach (LetterStats letterStat in letters.OrderBy(x=>x.Letter)) {

                Console.WriteLine(letterStat.Letter +" - "+ letterStat.Count + "\n");
            }
            
          
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
           // throw new NotImplementedException();
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static LetterStats IncStatistic(LetterStats letterStat)
        {
           letterStat.Count++;
           return letterStat;
        }


    }
}
