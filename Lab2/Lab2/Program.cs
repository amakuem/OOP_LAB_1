using Lab2.Document;
using Lab2.Users;
using Lab2.Editor;
using System.Security.Cryptography;
using System.Net.Security;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Lab2
{
    internal class Program
    {
        public static void DeleteLinesFromBottom(int linesToDelete)
        {
            if (linesToDelete <= 0) return;

            // Получаем текущую позицию курсора
            int originalCursorTop = Console.CursorTop;
            int originalCursorLeft = Console.CursorLeft;

            // Рассчитываем стартовую позицию для очистки
            int startLine = Math.Max(0, originalCursorTop - linesToDelete + 1);

            // Очищаем каждую строку
            for (int i = startLine; i <= originalCursorTop; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.BufferWidth));
            }

            // Перемещаем курсор обратно в исходную позицию (с учётом удалённых строк)
            Console.SetCursorPosition(originalCursorLeft, startLine);
        }
        public static User SelectUserToLogin()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select user to login:");

                // Показать список пользователей
                for (int i = 0; i < UserManager.Users.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {UserManager.Users[i].Name}");
                }

                Console.WriteLine("\n0. Create a new user.");
                Console.Write($"Enter choice (0 - {UserManager.Users.Count}): ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == 0)
                    {
                        // Создание нового пользователя
                        Console.Write("Enter new user name: ");
                        string name = Console.ReadLine();
                        UserManager.AddUser(name);
                    }
                    else if (choice > 0 && choice <= UserManager.Users.Count)
                    {
                        // Возврат выбранного пользователя
                        return UserManager.Users[choice - 1];
                    }
                }

                // Обработка невалидного ввода
                Console.WriteLine("Invalid choice!");
                PressAnyButton();
            }
        }
        public static async Task<Document.Document> OpeningProcess(Document.Document currentDocument)
        {
            bool opening = true;
            while (opening)
            {
                Console.Clear();
                Console.WriteLine("Document Management System");
                Console.WriteLine("--------------------------");
                Console.WriteLine("1. Open Document");
                Console.WriteLine("2. Switch User");

                Console.WriteLine("3.Create Documnet(Admin only)");


                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            Console.WriteLine("Select storage type:");
                            Console.WriteLine("1. Local File");
                            Console.WriteLine("2. Supabase Cloud");

                            var storageChoice = Console.ReadLine();
                            try
                            {
                                if (storageChoice == "2")
                                {
                                    DocumentManager.SetStorageStrategy(new SupabaseStorageStrategy(
                                        Lab2.DB.GetUrl(),
                                        Lab2.DB.GetKey()
                                    ));
                                    Console.Write("Enter cloud file name (e.g., document.json): ");
                                }
                                else
                                {
                                    DocumentManager.SetStorageStrategy(new LocalFileStrategy());
                                    Console.Write("Enter local file path (e.g., doc.txt): ");
                                }

                                string FileName = Console.ReadLine();
                                currentDocument = await DocumentManager.OpenDocument(FileName);

                                UserManager.CheckUser(Session.CurrentUser, currentDocument);
                                try
                                {
                                    Session.SetPermision(Session.CurrentUser);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    currentDocument = null;
                                    PressAnyButton();
                                    break;
                                }


                                Console.WriteLine($"Loaded: {FileName}");

                                currentDocument.Subscribe(Session.CurrentUser);
                                Console.WriteLine($"Type: {currentDocument.type}");
                                Console.WriteLine($"Content:\n{currentDocument.GetDisplayText()}");
                            }
                            catch (FileNotFoundException ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            //catch (Postgrest.Exceptions.PostgrestException ex)
                            //{
                            //    Console.WriteLine($"Supabase error: {ex.Message}");
                            //}
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");

                            }

                            PressAnyButton();
                            opening = false;
                            break;
                        case "2":
                            Console.Clear();
                            
                            User user = SelectUserToLogin();
                            Session.Login(user);
                            Console.WriteLine($"Logged in as: {Session.CurrentUser.Name}");
                            PressAnyButton();


                            break;
                        case "3":
                            if(Session.CurrentUser.Role != UserRole.Admin)
                            {
                                Console.WriteLine("You dont have permission.");
                                PressAnyButton();
                                break;
                            }
                            Console.WriteLine("Select document type:");
                            Console.WriteLine("1. PlainText");
                            Console.WriteLine("2. Markdown");
                            Console.WriteLine("3. RichText");
                            string typeChoice = Console.ReadLine();
                            DocumentType docType;
                            switch (typeChoice)
                            {
                                case "1":
                                    docType = DocumentType.PlainText;
                                    break;
                                case "2":
                                    docType = DocumentType.Markdown;
                                    break;
                                case "3":
                                    docType = DocumentType.RichText;
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Defaulting to PlainText.");
                                    docType = DocumentType.PlainText;
                                    PressAnyButton();
                                    break;
                            }
                            currentDocument = DocumentManager.CreateNewDocument(docType);
                            currentDocument.Notify("New document created!");
                            currentDocument.Subscribe(Session.CurrentUser);
                            List<string> temp = new List<string>();
                            temp.Add(Session.CurrentUser.Name);
                            currentDocument.Editors = temp;
                            UserManager.CheckUser(Session.CurrentUser, currentDocument);
                            try
                            {
                                Session.SetPermision(Session.CurrentUser);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                currentDocument = null;
                                PressAnyButton();
                                break;
                            }
                            Console.WriteLine($"New {docType} document created.");
                            PressAnyButton();
                            opening = false;
                            break;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    PressAnyButton();
                }
            }
            return currentDocument;
        }
        static private void PressAnyButton()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        static async Task Main(string[] args)
        {
            bool start = true;
            while (start)
            {
                Lab2.Document.Document currentDocument = null;
                UndoRedoManager undoRedoManager = new UndoRedoManager();
                bool running = true, entry = true, opening = true;

                User user = SelectUserToLogin();
                Session.Login(user);
                Console.WriteLine($"Logged in as: {Session.CurrentUser.Name}");
                PressAnyButton();

                currentDocument = await OpeningProcess(currentDocument);

                Console.WriteLine($"Content:\n{currentDocument.GetDisplayText()}");

                while (running)
                {
                    Console.Clear();
                    Console.WriteLine("Document Management System");
                    Console.WriteLine("--------------------------");
                    Console.WriteLine($"Current User: {Session.CurrentUser.Name} | Role: {Session.CurrentUser.Role}");
                    Console.WriteLine("Current Document: " + (currentDocument?.filePath ?? "None"));
                    Console.WriteLine("Current Document type: " + (currentDocument != null ? currentDocument.type.ToString() : "None"));
                    Console.WriteLine("Content:");
                    if (currentDocument.text != null && currentDocument.text != "")
                    {
                        
                        string formattedText = TextFormater.FormatText(currentDocument.GetDisplayText(), currentDocument.type.ToString());
                        string[] lines = formattedText.Split('\n');
                        foreach (var line in lines)
                        {
                            Console.WriteLine("  " + line.TrimEnd('\r'));
                        }
                    }
                    else
                    {
                        Console.WriteLine("No content");
                    }
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("1. Insert Text");
                    Console.WriteLine("2. Delete Text");
                    Console.WriteLine("3. Formate Text");
                    Console.WriteLine("4. Copy Text");
                    Console.WriteLine("5. Cut Text");
                    Console.WriteLine("6. Paste Text");
                    Console.WriteLine("7. Search Word");
                    Console.WriteLine("8. Save Document");
                    Console.WriteLine("9. Delete Document(Admin only)");
                    Console.WriteLine("10. Undo");
                    Console.WriteLine("11. Redo");
                    Console.WriteLine("12. Exit");
                    Console.WriteLine("=============================");
                    Console.WriteLine("13. Manage Users (Admin only)");
                    Console.WriteLine("14. Terminal Settings");
                    Console.WriteLine("15. View Document History");
                    Console.Write("\nEnter your choice (1-15): ");

                    string choice = Console.ReadLine();

                    try
                    {
                        switch (choice)
                        {
                            case "1":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                currentDocument.ShowEveryWordIndex();
                                Console.Write("Enter character position to insert at (ignoring <b /b>, <i /i>, <u /u>): ");
                                int insertPos = int.Parse(Console.ReadLine());
                                Console.Write("Enter text to insert(CTR + X to end input): ");



                                StringBuilder input = new StringBuilder();
                                while (true)
                                {
                                    var key = Console.ReadKey(intercept: true); // Перехватываем клавишу без вывода

                                    // Проверяем комбинацию Ctrl+X
                                    if (key.Key == ConsoleKey.X && (key.Modifiers & ConsoleModifiers.Control) != 0)
                                        break;

                                    // Обработка Enter
                                    if (key.Key == ConsoleKey.Enter)
                                    {
                                        input.Append(Environment.NewLine);
                                        Console.WriteLine(); // Переводим курсор на новую строку
                                    }
                                    // Обработка Backspace (опционально)
                                    else if (key.Key == ConsoleKey.Backspace)
                                    {
                                        if (input.Length > 0 && Console.CursorLeft > 0)
                                        {
                                            input.Remove(input.Length - 1, 1);
                                            Console.Write("\b \b"); // Удаляем символ из консоли
                                        }
                                    }
                                    // Обычные символы
                                    else if (!char.IsControl(key.KeyChar))
                                    {
                                        input.Append(key.KeyChar);
                                        Console.Write(key.KeyChar); // Выводим символ
                                    }
                                }




                                //string insertText = Console.ReadLine();
                                ICommand insertCommand = new InsertCommand(currentDocument, insertPos, input.ToString());
                                undoRedoManager.ExecuteCommand(insertCommand);


                                break;

                            case "2":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                Console.Write("Enter start index to delete: ");
                                int deleteStart = int.Parse(Console.ReadLine());
                                Console.Write("Enter number of symbols to delete: ");
                                int deleteCount = int.Parse(Console.ReadLine());
                                ICommand deleteCommand = new DeleteCommand(currentDocument, deleteCount, deleteStart);
                                undoRedoManager.ExecuteCommand(deleteCommand);

                                break;
                            case "3":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                Console.Write("Enter start symbol index to formate: ");
                                int formatStart = int.Parse(Console.ReadLine());
                                Console.Write("Enter number of symbols to formate: ");
                                int formatCount = int.Parse(Console.ReadLine());
                                Console.WriteLine("Choose the formatiotion: ");
                                Console.WriteLine("1.Bold(<b /b>)");
                                Console.WriteLine("2.Italic(<i /i>)");
                                Console.WriteLine("3.Underline(<u /u>)");
                                Console.WriteLine("Enter your choice (1-2):");
                                int formatChoice = int.Parse(Console.ReadLine());

                                switch (formatChoice)
                                {
                                    case 1:
                                        ICommand formatCommand = new FormatCommand(currentDocument, formatStart, formatCount, "Bold");
                                        undoRedoManager.ExecuteCommand(formatCommand);
                                        break;
                                    case 2:
                                        ICommand formatCommand2 = new FormatCommand(currentDocument, formatStart, formatCount, "Italic");
                                        undoRedoManager.ExecuteCommand(formatCommand2);
                                        break;
                                    case 3:
                                        ICommand formatCommand3 = new FormatCommand(currentDocument, formatStart, formatCount, "Underline");
                                        undoRedoManager.ExecuteCommand(formatCommand3);
                                        break;
                                }

                                break;

                            case "4":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                
                                Console.Write("Enter start symbol index to copy: ");
                                int copyStart = int.Parse(Console.ReadLine());
                                Console.Write("Enter number of symbols to copy: ");
                                int copyCount = int.Parse(Console.ReadLine());
                                ICommand copyCommand = new CopyCommand(currentDocument, copyCount, copyStart);
                                undoRedoManager.ExecuteCommand(copyCommand);
                                Console.WriteLine("Text copied to clipboard.");

                                PressAnyButton();
                                break;

                            case "5":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                Console.Write("Enter start symbols index to cut: ");
                                int cutStart = int.Parse(Console.ReadLine());
                                Console.Write("Enter number of symbols to cut: ");
                                int cutCount = int.Parse(Console.ReadLine());
                                ICommand cutCommand = new CutCommand(currentDocument, cutCount, cutStart);
                                undoRedoManager.ExecuteCommand(cutCommand);
                                Console.WriteLine("Text cut to clipboard.");

                                break;

                            case "6":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                Console.Write("Enter symbol position to paste at: ");
                                int pastePos = int.Parse(Console.ReadLine());
                                ICommand pasteCommand = new PasteCommand(currentDocument, pastePos);
                                undoRedoManager.ExecuteCommand(pasteCommand);
                                Console.WriteLine("Text pasted from clipboard.");

                                break;

                            case "7":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                Console.Write("Enter word to search (ignore **, __, *): ");
                                string searchWord = Console.ReadLine();
                                currentDocument.SearchWord(searchWord);

                                PressAnyButton();
                                break;

                            case "8":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                Console.WriteLine("Select storage type:");
                                Console.WriteLine("1. Local File");
                                Console.WriteLine("2. Supabase Cloud");
                                var storageChoice1 = Console.ReadLine();

                                try
                                {
                                    if (storageChoice1 == "1")
                                    {
                                        DocumentManager.SetStorageStrategy(new LocalFileStrategy());
                                        Console.Write("Enter local file name to save (e.g., doc.txt): ");
                                    }
                                    else if (storageChoice1 == "2")
                                    {
                                        DocumentManager.SetStorageStrategy(new SupabaseStorageStrategy(
                                            Lab2.DB.GetUrl(),
                                            Lab2.DB.GetKey()
                                        ));
                                        Console.Write("Enter cloud file name to save (e.g., document.json): ");
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Invalid storage type");
                                    }
                                    string FileName = Console.ReadLine();
                                    await DocumentManager.SaveDocument(currentDocument, FileName);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error: {ex.Message}");
                                }

                                PressAnyButton();
                                break;

                            case "9":
                                if (!Session.PermissionStrategy.CanManageUsers())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                Console.Write("Enter filename to delete: ");
                                string deletePath = Console.ReadLine();
                                if (File.Exists(deletePath))
                                {
                                    DocumentManager.DeleteDocument(deletePath);
                                    if (currentDocument != null && currentDocument.filePath == deletePath)
                                    {
                                        currentDocument = null;
                                    }
                                    Console.WriteLine("Document deleted successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("File does not exist.");
                                }
                                PressAnyButton();


                                break;

                            case "10":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                undoRedoManager.Undo();
                                Console.WriteLine("Undo performed.");
                                PressAnyButton();
                                
                                break;

                            case "11":
                                if (!Session.PermissionStrategy.CanEdit())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                undoRedoManager.Redo();
                                Console.WriteLine("Redo performed.");
                                PressAnyButton();

                                break;
                            case "12":
                                running = false;

                                break;

                            case "13":
                                if (!Session.PermissionStrategy.CanManageUsers())
                                {
                                    Console.WriteLine("You cant do this action with your role!");
                                    PressAnyButton();
                                    break;
                                }
                                bool manage = true;
                                while (manage)
                                {
                                    Console.WriteLine("--------------------------");
                                    Console.WriteLine("Roles for this Documnet:");
                                    string curEditors = $"Editors: {string.Join(", ", currentDocument.Editors)}";
                                    string curViewers = $"Viewers: {string.Join(", ", currentDocument.Viewers)}\n";
                                    Console.WriteLine(curEditors);
                                    Console.WriteLine(curViewers);
                                    Console.WriteLine("1. Add Editor.");
                                    Console.WriteLine("2. Add Viewer.");
                                    Console.WriteLine("3. Delete Editor.");
                                    Console.WriteLine("4. Delete Viewer.\n");
                                    Console.WriteLine("0. Exit");
                                    int managChoice = int.Parse(Console.ReadLine());
                                    switch (managChoice)
                                    {
                                        case 1:
                                            DeleteLinesFromBottom(12);
                                            while (true)
                                            {
                                                Console.WriteLine($"\nAll Users: {string.Join(", ", UserManager.Users.Select((user, index) => $"{index + 1}. {user}"))}\n");
                                                Console.WriteLine("Enter the number of user to ADD: ");
                                                if (int.TryParse(Console.ReadLine(), out int choice2))
                                                {

                                                    if (choice2 > 0 && choice2 <= UserManager.Users.Count)
                                                    {
                                                        if(currentDocument.Editors.Contains(UserManager.Users[choice2 - 1].Name))
                                                        {
                                                            Console.WriteLine("This user already an Editor");
                                                            PressAnyButton();
                                                            DeleteLinesFromBottom(8);
                                                            continue;
                                                        }
                                                        currentDocument.Editors.Add(UserManager.Users[choice2 - 1].Name);
                                                        break;
                                                    }

                                                }
                                            }
                                            

                                            PressAnyButton();
                                            DeleteLinesFromBottom(8);
                                            break;

                                        case 2:
                                            DeleteLinesFromBottom(12);
                                            while (true)
                                            {
                                                Console.WriteLine($"\nAll Users: {string.Join(", ", UserManager.Users.Select((user, index) => $"{index + 1}. {user}"))}\n");
                                                Console.WriteLine("Enter the number of user to ADD: ");
                                                if (int.TryParse(Console.ReadLine(), out int choice3))
                                                {

                                                    if (choice3 > 0 && choice3 <= UserManager.Users.Count)
                                                    {
                                                        if (currentDocument.Editors.Contains(UserManager.Users[choice3 - 1].Name))
                                                        {
                                                            Console.WriteLine("This user already an Editor");
                                                            PressAnyButton();
                                                            DeleteLinesFromBottom(8);
                                                            continue;
                                                        }
                                                        if(currentDocument.Viewers.Contains(UserManager.Users[choice3 - 1].Name))
                                                        {
                                                            Console.WriteLine("This user already an Viewer");
                                                            PressAnyButton();
                                                            DeleteLinesFromBottom(8);
                                                            continue;
                                                        }
                                                        currentDocument.Viewers.Add(UserManager.Users[choice3 - 1].Name);
                                                        break;
                                                    }

                                                }
                                            }
                                            
                                            PressAnyButton();
                                            DeleteLinesFromBottom(8);
                                            break;

                                        case 3:
                                            DeleteLinesFromBottom(12);
                                            Console.WriteLine($"\nAll Editors: {string.Join(", ", currentDocument.Editors.Select((user, index) => $"{index + 1}. {user}"))}\n");
                                            Console.WriteLine("Enter the number of user to DELETE: ");
                                            if (int.TryParse(Console.ReadLine(), out int choice4))
                                            {

                                                if (choice4 > 0 && choice4 <= UserManager.Users.Count)
                                                {
                                                    currentDocument.Editors.RemoveAt(choice4 - 1);
                                                }

                                            }
                                            PressAnyButton();
                                            DeleteLinesFromBottom(8);
                                            break;

                                        case 4:
                                            DeleteLinesFromBottom(12);
                                            Console.WriteLine($"\nAll Viewers: {string.Join(", ", currentDocument.Viewers.Select((user, index) => $"{index + 1}. {user}"))}\n");
                                            Console.WriteLine("Enter the number of user to DELETE: ");
                                            if (int.TryParse(Console.ReadLine(), out int choice5))
                                            {

                                                if (choice5 > 0 && choice5 <= UserManager.Users.Count)
                                                {
                                                    currentDocument.Viewers.RemoveAt(choice5 - 1);
                                                }

                                            }
                                            PressAnyButton();
                                            DeleteLinesFromBottom(8);

                                            break;

                                        case 0:
                                            manage = false;

                                            break;
                                    }
                                }

                                //PressAnyButton();
                                break;

                            case "14":
                                TerminalSettingsManager.Instance.ShowTerminalSettingsMenu();
                                PressAnyButton();


                                break;

                            case "15":
                                Console.WriteLine("\nDocument History:");
                                Console.WriteLine("{0,-25} {1,-10} {2,-50}",
                                    "Timestamp", "Action", "Content Preview");
                                foreach (var entry1 in currentDocument.GetHistory())
                                {
                                    Console.WriteLine("{0,-25} {1,-10} {2,-50}",
                                        entry1.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                                        entry1.ActionType,
                                        entry1.Content.Truncate(90));
                                }
                                PressAnyButton();

                                break;

                        }

                    }
                    catch(Exception ex)
                    {

                    }



                }
                



            }
            
           

            




        }
    }
}
//--------------Tets-------------
//List<string> Editors = new List<string> { "admin", "user1" };
//List< string > Viewers = new List<string> { "guest" };
//Lab2.Document.Document doc = new Lab2.Document.Document(DocumentType.Markdown);
//doc.AddText("Hello Babar!");
//doc.Editors = Editors;
//doc.Viewers = Viewers;
//DocumentManager.SetStorageStrategy(new LocalFileStrategy());

//string FileName = "hello.xml";
//await DocumentManager.SaveDocument(doc, FileName);

//doc = await DocumentManager.OpenDocument(FileName);


//-------------------------------------------------------------------

//    while (running)
//    {
//        Console.Clear();
//        Console.WriteLine("Document Management System");
//        Console.WriteLine("--------------------------");
//        Console.WriteLine($"Current User: {Session.CurrentUser.Name} | Role: {Session.CurrentUser.Role}");
//        Console.WriteLine("Current Document: " + (currentDocument?.filePath ?? "None"));
//        Console.WriteLine("Current Document type: " + (currentDocument != null ? currentDocument.type.ToString() : "None"));
//        Console.WriteLine("Content:");
//        if (currentDocument != null)
//        {
//            string formattedText = TextFormater.FormatText(currentDocument.GetDisplayText(), currentDocument.type.ToString());
//            string[] lines = formattedText.Split('\n');
//            foreach (var line in lines)
//            {
//                Console.WriteLine("  " + line.TrimEnd('\r'));
//            }
//        }
//        else
//        {
//            Console.WriteLine("No content");
//        }
//        Console.WriteLine("\nOptions:");
//        Console.WriteLine("1. Open Document");
//        Console.WriteLine("2. Save Document");
//        Console.WriteLine("3. Append Text");
//        Console.WriteLine("4. Insert Text");
//        Console.WriteLine("5. Delete Text");
//        Console.WriteLine("6. Copy Text");
//        Console.WriteLine("7. Cut Text");
//        Console.WriteLine("8. Paste Text");
//        Console.WriteLine("9. Search Word");
//        Console.WriteLine("10. Save Document");

//        Console.WriteLine("11. Undo");
//        Console.WriteLine("12. Redo");
//        Console.WriteLine("13. Exit");

//        Console.WriteLine("=============================");
//        Console.WriteLine("14. Create New Document (Admin only )");
//        Console.WriteLine("15. Delete Document");
//        Console.WriteLine("16. Manage Users (Admin only)");
//        Console.WriteLine("17. Switch User");
//        Console.WriteLine("18. Terminal Settings");
//        Console.WriteLine("19. View Document History");

//        Console.Write("\nEnter your choice (1-19): ");


//    }