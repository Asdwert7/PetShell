
using static System.Console;

namespace ASDshell
{
    class Program
    {
        private const int ASD_RL_BUFSIZE = 1024;
        /// <summary>
        /// Тут просто залупим три команды
        /// </summary>

        string[] asd_read_line()
        {
            int bufsize = ASD_RL_BUFSIZE;
            int position = 0;

            char[] buffer = new char[bufsize];
            char ch;
            int c;

            while (1)
            {
                c = Read();
                ch = (char)c; 

                // If we hit EOF, replace it with a null character and return.
                if (c == -1 )
                {
                    
                }
            }
        }
        void asd_loop()
        {
            string line;
            string[] arrayLines;
            int status;

            do
            {
                WriteLine("> ");
                line = asd_read_line();
                arrayLines = asd_split_lines();
                status = asd_execute();
            } while (status);
        }
        static void Main()
        {

        }

    }
}
