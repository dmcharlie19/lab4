using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LAB4
{
    class Program
    {
        static void Main(string[] args)
        {

            
            Library_LoadBooks(); 
            LastOpenBook_load();
            UI_MainMenu(); 
        }

        static List<Book> library = new List<Book>(); 

        static Stack<Book> lastBook = new Stack<Book>(); 

        /* Пользовательский интерфейс [НАЧАЛО] */
        static void UI_MainMenu() // главное меню [функционал]
        {
            do
            {
                switch (UI_MainMenuItems())
                {
                    case 1: 
                        Console.Clear();
                        PrintBooks(library); 
                        break;
                    case 2: 
                        Console.Clear();
                        LastOpenBook_print(); 
                        break;
                    case 3: 
                        Console.Clear(); 
                        AddBook(); 
                        break;
                    case 4: 
                        Console.Clear(); 
                        Console.Write("Что будем искать?\nВведите запрос: ");
                        string find = Console.ReadLine(); 
                        BookFind(find); 
                        break;
                    case 0: 
                        Library_Save(); 
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Ошибка!");
                        break;
                }
                Console.WriteLine("\n\nДля продолжения нажмите любую клавишу...");
                Console.ReadLine();
            } while (true);
        }

        static int UI_MainMenuItems() // главное меню [пункты]
        {
            Console.Clear();
            Console.WriteLine("Меню: ");
            Console.WriteLine("1. Вывод всех доступных электронных изданий");
            Console.WriteLine("2. Вывод недавно открывавшихся изданий");
            Console.WriteLine("3. Добавление нового издания");
            Console.WriteLine("4. Поиск издания");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите команду: ");

            /* Проверка на нажатие клавиши, чтобы программа не вылетала при некорректном вводе */
            int number;
            string value = Console.ReadLine(); 
            bool result = Int32.TryParse(value, out number); 
            if (result)
                return Convert.ToInt32(value); 

            return 0; 
        }

        static void UI_MenuBook(List<Book> list) 
        {

            switch (UI_MenuBookItems())
            {
                case 1: 
                    BookSort(list, 1); 
                    break;
                case 2: 
                    BookSort(list, 2);
                    break;
                case 3: 
                    BookSort(list, 3); 
                    break;
                case 4: 
                    BookView(list); 
                    break;
                case 5: 
                    UI_MainMenuItems();
                    break;
                default:
                    break;
            }
        }

        static int UI_MenuBookItems() // вспомогательное меню [пункты]
        {

            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Отсортировать по названию");
            Console.WriteLine("2. Отсортировать по автору");
            Console.WriteLine("3. Отсортировать по дате");
            Console.WriteLine("4. Просмотр информации об издании");
            Console.WriteLine("5.Главное меню");
            Console.Write("Выберите команду: ");

            /* Проверка на нажатие клавиши, чтобы программа не вылетала при некорректном вводе */
            int number;
            string value = Console.ReadLine(); 
            bool result = Int32.TryParse(value, out number); 
            if (result)
                return Convert.ToInt32(value); 

            return 0; 
        }

        static bool UI_MenuBookView(List<Book> list, Book check)
        {
            bool result = true; // используем для зацикливания меню
            switch (UI_MenuViewItems())
            {
                case 1: // чтение файла
                    Process.Start(check.path); // открываем файл, по адресу check.path
                    LastOpenBook_add(check.id, check.name, check.author, check.year, check.path);
                    break;
                case 2: 
                    Console.Clear(); 
                    library.Remove(check); 
                    PrintBooks(list); 
                    result = false; 
                    break;
                case 3: 
                    Console.Clear(); 
                    FileInfo fileInf = new FileInfo(check.path); 
                    string[] file_name = BookName(); 
                                                     
                    string new_file_name = fileInf.DirectoryName + @"\" + file_name[0] + "_" + file_name[1] + "_" + file_name[2] + fileInf.Extension;
                    fileInf.MoveTo(new_file_name); 

                    /* переименовываем издание в коллекции */
                    library.Remove(check); 
                    int item = check.id - 1; 
                    library.Insert(item, new Book(check.id, file_name[0], file_name[1], Convert.ToInt32(file_name[2]), new_file_name)); // вставляем издание в коллекцию 

                    Console.Clear(); 
                    PrintBooks(list); 
                    result = false; 
                    break;
                case 0: 
                    Console.Clear(); 
                    PrintBooks(list); 
                    result = false; 
                    break;
                default:
                    break;
            }
            return result;
        }

        static int UI_MenuViewItems() // меню просмотра информации об издании [пункты]
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Читать");
            Console.WriteLine("2. Удалить из библиотеки");
            Console.WriteLine("3. Изменить выходные данные");
            Console.WriteLine("0. Вернуться к списку изданий");
            Console.Write("Выберите команду: ");

          
            int number;
            string value = Console.ReadLine();
            bool result = Int32.TryParse(value, out number); 
            if (result)
                return Convert.ToInt32(value); 

            return 0; 
        }
        /* Пользовательский интерфейс [КОНЕЦ] */

        /* Cтруктура электронного издания [НАЧАЛО] */
        public struct Book
        {
            public Book(int _id, string _name, string _author, int _year, string _path)
            {
                id = _id;
                name = _name;
                author = _author;
                year = _year;
                path = _path;
            }

            public int id; 
            public string name; 
            public string author; 
            public int year;
            public string path; 
        }
        /* Cтруктура электронного издания [КОНЕЦ] */

        /* Методы для работы с библиотекой [НАЧАЛО] */
        static void PrintBooks(List<Book> list) 
        {
            Console.WriteLine("| ID |        Название        |       Автор        |   Год издания  |");
            Console.WriteLine("---------------------------------------------------------------------");
            foreach (Book p in list) 
            {
                
                string output = String.Format("|{0, 4}|{1,23} |{2,19} | {3,14} |", p.id, p.name, p.author, p.year);
                Console.WriteLine(output); 
                Console.WriteLine("---------------------------------------------------------------------");
            }
            UI_MenuBook(library); 
        }

        static void BookView(List<Book> list) // просмотр информации об издании
        {
            Console.Write("\nВведите ID издания: ");
            int number;
            string value = Console.ReadLine();
            bool result = Int32.TryParse(value, out number);
            if (result)
            {
                int id = Convert.ToInt32(value);
                Book check = list.Find(
                    delegate (Book bk)
                    {
                        return bk.id == id;
                    }
                  );

                if (check.id != 0)
                {
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("| ID |        Название        |       Автор        |   Год издания  |        Путь к файлу с текстом ");
                        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------------");
                        string output = String.Format("|{0, 4}|{1,23} |{2,19} | {3,14} | {4,20} ", check.id, check.name, check.author, check.year, check.path);
                        Console.WriteLine(output);
                    } while (UI_MenuBookView(list, check));

                }
            }
            else
                return;
        }

        static void BookFind(string find) // поиск издания
        {
            Console.Clear();

            List<Book> _list = new List<Book>();
            foreach (Book b in library)
            {
                if (b.name.Contains(find) || b.author.Contains(find) || Convert.ToString(b.year).Contains(find))
                {
                    _list.Add(new Book(b.id, b.name, b.author, b.year, b.path));
                }
            }

            if (_list.Count != 0)
            {
                PrintBooks(_list);
            }
            else
                Console.WriteLine("По вашему запросу, ничего не найдено.");
        }

        static void BookSort(List<Book> list, int sortType) // тип сортировки
        {
            if (sortType == 1)
            {
                var sortedByName = list.OrderBy(t => t.name);
                SortBy(list, sortedByName);
            }
            else if (sortType == 2)
            {
                var sortedByAuthor = list.OrderBy(t => t.author);
                SortBy(list, sortedByAuthor);
            }
            else if (sortType == 3)
            {
                var sortedByYear = list.OrderBy(t => t.year);
                SortBy(list, sortedByYear);
            }
        }

        static void SortBy(List<Book> list, IOrderedEnumerable<Book> sort) // сортировка
        {
            Console.Clear();
            Console.WriteLine("| ID |        Название        |       Автор        |   Год издания  |");
            Console.WriteLine("---------------------------------------------------------------------");
            foreach (var item in sort)
            {
                string output = String.Format("|{0, 4}|{1,23} |{2,19} | {3,14} |", item.id, item.name, item.author, item.year);
                Console.WriteLine(output);
                Console.WriteLine("---------------------------------------------------------------------");
            }
            UI_MenuBook(list);
        }

        static void AddBook() // добавление издания в библиотеку
        {
            Console.Write("Путь к изданию: ");
            string file_path = Console.ReadLine();
            FileInfo fileInf = new FileInfo(file_path);
            if (fileInf.Exists)
            {
                Console.WriteLine(fileInf.DirectoryName);
                /* Если файл существует */
                string[] file_name = BookName();
                fileInf.MoveTo(fileInf.DirectoryName + @"\" + file_name[0] + "_" + file_name[1] + "_" + file_name[2] + fileInf.Extension); 
                string new_file_name = fileInf.DirectoryName + @"\" + file_name[0] + "_" + file_name[1] + "_" + file_name[2] + fileInf.Extension;
                library.Add(new Book(library.Count() + 1, file_name[0], file_name[1], Convert.ToInt32(file_name[2]), new_file_name)); 
            }
            else
            {
                /* Если файл не найден */
                Console.WriteLine("Издание не найдено!");
            }
        }

        static void LastOpenBook_load() // загрузка данных о последних открывавшихся изданиях
        {
            try
            {
                using (StreamReader sr = new StreamReader(@"..\Access\last.dat", System.Text.Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // разбираем информацию об издании
                        string[] words = line.Split('_');
                        lastBook.Push(new Book(Convert.ToInt32(words[0]), words[1], words[2], Convert.ToInt32(words[3]), words[4]));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void LastOpenBook_add(int _id, string _name, string _author, int _year, string _file) // добавление в стек открывавшегося издания
        {
            Stack<Book> _lastBook = new Stack<Book>();
            Book nB = new Book(_id, _name, _author, _year, _file);
            bool result = lastBook.Contains(nB);
            if (!result) // проверяем, нет ли в стеке этого издания
            {
                if (lastBook.Count < 3) // храним только три издания
                    lastBook.Push(nB);
                else
                {
                    /* Если в стеке больше трёх изданий, то обновляем его */
                    foreach (Book b in lastBook) _lastBook.Push(b);
                    lastBook.Clear();
                    _lastBook.Pop();
                    foreach (Book b in _lastBook) lastBook.Push(b);
                    lastBook.Push(nB);
                }
            }
            else
            {
                /* Если есть, то найдем его и переместим вверх */
                foreach (Book b in lastBook)
                {
                    if (!b.Equals(nB))
                        _lastBook.Push(b);
                }
                lastBook.Clear();
                foreach (Book b in _lastBook) lastBook.Push(b);
                lastBook.Push(nB);
            }

            /* Сохраним данные стека в файл */
            FileInfo last = new FileInfo(@"..\Access\last.dat");
            if (last.Exists) last.Delete();

            using (StreamWriter sw = new StreamWriter(@"..\Access\last.dat", false, System.Text.Encoding.UTF8))
            {
                _lastBook.Clear();
                foreach (Book b in lastBook) _lastBook.Push(b);
                foreach (Book b in _lastBook)
                {
                    string book = b.id + "_" + b.name + "_" + b.author + "_" + b.year + "_" + b.path;
                    sw.WriteLine(book);
                }
            }
        }

        static void LastOpenBook_print() // вывод недавно открывавшихся изданий
        {
            Console.WriteLine("| ID |        Название        |       Автор        |   Год издания  |");
            Console.WriteLine("---------------------------------------------------------------------");
            foreach (Book b in lastBook) // цикл, для обращения к элементам коллекции lastBook
            {
                string output = String.Format("|{0, 4}|{1,23} |{2,19} | {3,14} |", b.id, b.name, b.author, b.year);
                Console.WriteLine(output);
                Console.WriteLine("---------------------------------------------------------------------");
            }
        }
        /* Методы для работы с библиотекой [КОНЕЦ] */

        /* Загрузка библиотеки [НАЧАЛО] */
        static void Library_LoadBooks() // загрузка списка электронных изданий из файла
        {
            /* Проверяем, есть ли папка Access */
            string path = @"..\Access\"; // адрес папки
            DirectoryInfo dirBooks = new DirectoryInfo(path); // обратимся к папке по адресу
            if (!dirBooks.Exists)
                dirBooks.Create(); // если папки нет, то создадим её

            FileInfo fileBooks = new FileInfo(path + "books.dat"); // обратимся к файлу books.dat

            if (fileBooks.Exists) // проверяем, есть ли файл books.dat
            {
                /* Проверка изданий */
                try 
                {
                    int id = 1; 
                                /* Откроем файл для чтения */
                    using (StreamReader sr = new StreamReader(path + "books.dat", System.Text.Encoding.UTF8))
                    {
                        string line; // в эту строку, записываем строку из файла
                        while ((line = sr.ReadLine()) != null) // проверяем, достигнут ли конец файла
                        {
                            string[] _fileInf = line.Split('$'); // разделим полученную строку, на подстроки. Храним в массиве строк
                            FileInfo book = new FileInfo(_fileInf[4]); // обратимся к файлу по адресу, который хранится в _fileInf[4]
                            if (book.Exists) // проверяем существует ли файл
                                             /* если существует, то добавим издание в нашу коллекцию library */
                                library.Add(new Book(id++, _fileInf[2], _fileInf[1], Convert.ToInt32(_fileInf[3]), _fileInf[4]));
                        }
                        sr.Close();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); // вывод сообщения об ошибке, которая может возникнуть с файлом
                }
            }
            else
            {
                Console.WriteLine("папка не существует"); // если файл books.dat не создан, то создадим издания
            }
        }

        static void Library_Save() // сохранение списка изданий в файл
        {
            FileInfo book = new FileInfo(@"..\Access\books.dat"); // обратимся к файлу books.dat
            book.Delete(); // удалим файл, чтобы создать новый и записать в него список изданий
            using (StreamWriter sw = new StreamWriter(@"..\Access\books.dat", true, System.Text.Encoding.UTF8))
            {
                foreach (Book p in library) // цикл, для обращения к элементам коллекции library
                {
                    /* Сформируем строку, для записи в файл */
                    string S = p.id + "$" + p.name + "$" + p.author + "$" + p.year + "$" + p.path;
                    sw.WriteLine(S); // запишем строку в файл
                }
                sw.Close();
            }
        }

       
        /* Загрузка библиотеки [КОНЕЦ] */

        /* Вспомогательные методы [НАЧАЛО] */
        static string[] FileName_Split(string _f) // разбирает название файла
        {
            /* Разделим строку на подстроки и запишем в массив строк */
            string[] words = _f.Split('_'); //разделение строки происходит по символу '_'
            // убираем от года издания .расширение файла
            int n = words[2].IndexOf('.'); // получаем индекс '.' 
            words[2] = words[2].Remove(n); // удаляем то, что записано после '.' Например .txt

            return words; // возвращаем массив строк 
        }

        static string[] BookName() // выходные данные для издания
        {
            string[] NewBook = new string[3]; // массив строк
            Console.Write("Введите название издания: ");
            NewBook[0] = Console.ReadLine(); // считаем название издания
            Console.Write("Введите автора издания: ");
            NewBook[1] = Console.ReadLine(); // считаем автора издания
            Console.Write("Введите год издания: ");
            NewBook[2] = Console.ReadLine(); // считаем год издания

            return NewBook; // возвращаем массив строк
        }
        /* Вспомогательные методы [КОНЕЦ] */
    }

}
