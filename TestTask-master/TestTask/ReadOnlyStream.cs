using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;

        private StreamReader reader;

        private bool EndOfStream;


        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;


            FileStream fs = new FileStream(fileFullPath, FileMode.Open);


            _localStream = fs;

            StreamReader sr = new StreamReader(_localStream, Encoding.GetEncoding(1251));
            reader = sr;

            // TODO : Заменить на создание реального стрима для чтения файла!

        }




        public void CloseStream()
        {
           reader.Close();
        }



        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {

            get {
                
               if (this.reader.EndOfStream)
                {
                    return EndOfStream;
                }
                else {
                    return false;
                }
               
                
            } // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set {

                EndOfStream=true;

            }
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {

            try
            {

              char c =new char();
              c = (char)reader.Read();
              return c;
                           
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                return (char)0;
            }

            
            // TODO : Необходимо считать очередной символ из _localStream
            //  throw new NotImplementedException();
        }





        public char[] ReadAllChar()
        {
            char[] chars;
            chars = reader.ReadToEnd().ToCharArray();
            return chars;
        }


        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            _localStream.Position = 0;
            IsEof = false;
        }
    }
}
