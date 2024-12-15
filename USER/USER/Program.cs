using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Gtk;

public static class Globals
{
    public static string LOGIN;
    public static string ROLE;
    public static string RunCommandWithBash(string command)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{command}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WorkingDirectory = Environment.CurrentDirectory // Текущая директория
                    }
                };
    
                process.Start();
    
                // Читаем вывод
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
    
                process.WaitForExit();
    
                // Если есть ошибка, добавляем её в вывод
                Console.WriteLine(output);
                return string.IsNullOrWhiteSpace(error) ? output : $"{output}\nERROR: {error}";
            }
            catch (Exception ex)
            {
                return $"Exception occurred: {ex.Message}";
            }
        }
}

class Program
{
    static void Main(string[] args)
    {
        Application.Init();
        var loginWindow = new LoginWindow();
        Application.Run();
    }
}

// Экран логина
class LoginWindow : Window
{

    public LoginWindow() : base("Login")
    {
        SetDefaultSize(300, 200);
        SetPosition(WindowPosition.Center);

        var vbox = new VBox();
        var usernameEntry = new Entry { PlaceholderText = "Username" };
        var passwordEntry = new Entry { PlaceholderText = "Password", Visibility = false };
        var loginButton = new Button("Login");

        loginButton.Clicked += (sender, e) =>
        {
            string scpCommand = $"venv/bin/python viever.py check_login {usernameEntry.Text} {passwordEntry.Text}";
            try
            {
             
                //user: viewer
                //moderator: filler viewer
                //devops: migrate viever
                //developer: ща оформлю
                if (Globals.RunCommandWithBash(scpCommand).Contains("client"))
                {
                    Globals.ROLE = "client";
                    Console.WriteLine("Hello, client");
                    Globals.LOGIN = usernameEntry.Text;
                    var databaseWindow = new DatabaseSelectionWindow();
                    databaseWindow.ShowAll();
                    Destroy();
                    //'client', 'moderator', 'devops', 'developer'
                }

                else if (Globals.RunCommandWithBash(scpCommand).Contains("moderator"))
                {
                    Globals.ROLE = "moderator";
                    Console.WriteLine("Hello, moderator");
                    Globals.LOGIN = usernameEntry.Text;
                    var databaseWindow = new DatabaseSelectionWindow();
                    databaseWindow.ShowAll();
                    Destroy();
                    //'client', 'moderator', 'devops', 'developer'
                }

                else if (Globals.RunCommandWithBash(scpCommand).Contains("devops"))
                {
                    Globals.ROLE = "devops";
                    Console.WriteLine("Hello, devops");
                    Globals.LOGIN = usernameEntry.Text;
                    var databaseWindow = new DatabaseSelectionWindow();
                    databaseWindow.ShowAll();
                    Destroy();
                    //'client', 'moderator', 'devops', 'developer'
                }

                else if (Globals.RunCommandWithBash(scpCommand).Contains("developer"))
                {
                    Globals.ROLE = "developer";
                    Console.WriteLine("Hello, developer");
                    Globals.LOGIN = usernameEntry.Text;
                    var databaseWindow = new DatabaseSelectionWindow();
                    databaseWindow.ShowAll();
                    Destroy();
                    //'client', 'moderator', 'devops', 'developer'
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        };

        vbox.PackStart(usernameEntry, false, false, 5);
        vbox.PackStart(passwordEntry, false, false, 5);
        vbox.PackStart(loginButton, false, false, 5);

        Add(vbox);
        ShowAll();
    }
}

// Экран выбора базы данных
class DatabaseSelectionWindow : Window
{

    static string[] ParseStrings(string input)
    {
        // Регулярное выражение для извлечения текста внутри кавычек
        string pattern = @"'([^']*)'";
        MatchCollection matches = Regex.Matches(input, pattern);

        // Создание списка для хранения результатов
        List<string> extractedStrings = new List<string>();

        foreach (Match match in matches)
        {
            // Добавляем найденное значение в список
            extractedStrings.Add(match.Groups[1].Value);
        }

        // Преобразуем список в массив
        return extractedStrings.ToArray();
    }

    public DatabaseSelectionWindow() : base("Select Database")
    {
        SetDefaultSize(400, 300);
        SetPosition(WindowPosition.Center);

        string command = $"venv/bin/python viever.py get_available_databases {Globals.LOGIN}";
        string input = Globals.RunCommandWithBash(command);

        string[] result = ParseStrings(input);
        foreach (string str in result)
        {
            Console.WriteLine(str);
        }

        var vbox = new VBox();
        //var databases = new[] { "Database1", "Database2", "Database3" }; // Список баз данных

        //foreach (var db in databases)
        foreach (var db in result)
        {
            var button = new Button(db);
            button.Clicked += (sender, e) =>
            {
                var mainAppWindow = new MainAppWindow(db);
                Console.WriteLine("Selected:", db);

                mainAppWindow.ShowAll();
                Destroy();
            };
            vbox.PackStart(button, false, false, 5);
        }

        Add(vbox);
        ShowAll();
    }
}

// Главный экран приложения
class MainAppWindow : Window
{

    public MainAppWindow(string databaseName) : base($"Main App - {databaseName}")
    {
        SetDefaultSize(600, 400);
        SetPosition(WindowPosition.Center);

        var vbox = new VBox();

        // Текстовое поле
        var textView = new TextView { Editable = false };
        textView.Buffer.Text = $"Connected to {databaseName}";
        vbox.PackStart(textView, true, true, 5);

        // Терминал команд
        var commandEntry = new Entry { PlaceholderText = "Enter command..." };
        var executeButton = new Button("Execute");

        //string scpCommand = $"venv/bin/python viever.py check_login {usernameEntry.Text} {passwordEntry.Text}";
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        executeButton.Clicked += (sender, e) =>
        {
            var command = commandEntry.Text;
            if (Globals.ROLE == "client")
            {
                if (command == "help")
                {
                    textView.Buffer.Text += $"\n> {command}\nCommand executed.";
                    textView.Buffer.Text += "\n\tget_database_table";
                    textView.Buffer.Text += "\n\tget_table_columns table1";
                    textView.Buffer.Text += "\n\tget_table_data table1";
                    textView.Buffer.Text += "\n\tget_structure";
                }
                else
                {
                    Console.WriteLine(command);
                    if (command == "get_structure")
                    {
                        Console.WriteLine($"Command is {command}");
                        string cmd = $"venv/bin/python viever.py get_structure {databaseName}.db";
                        string output = Globals.RunCommandWithBash(cmd);
                        Console.WriteLine("Output is:\"" + output + "\"");
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command == "get_database_tables")
                    {
                        string cmd = $"venv/bin/python viever.py get_database_tables {databaseName}.db";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_columns"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_columns {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_data"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_data {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }
                }    
            }
            
 //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~           
            if (Globals.ROLE == "moderator")
            {
                if (command == "help")
                {
                    textView.Buffer.Text += $"\n> {command}\nCommand executed.";
                    textView.Buffer.Text += "\n\tget_database_table";
                    textView.Buffer.Text += "\n\tget_table_columns table1";
                    textView.Buffer.Text += "\n\tget_table_data table1";
                    textView.Buffer.Text += "\n\tget_structure";
                    textView.Buffer.Text += "\n\t~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
                    textView.Buffer.Text += "\n\tadd_data_to_table";
                    textView.Buffer.Text += "\n\terase_data_from_table";
                    textView.Buffer.Text += "\n\tmodify_data_in_table";
                    textView.Buffer.Text += "\n\tset_data_none_in_table";
                }
                else
                {
                    // viever
                    Console.WriteLine(command);
                    if (command == "get_structure")
                    {
                        Console.WriteLine($"Command is {command}");
                        string cmd = $"venv/bin/python viever.py get_structure {databaseName}.db";
                        string output = Globals.RunCommandWithBash(cmd);
                        Console.WriteLine("Output is:\"" + output + "\"");
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command == "get_database_tables")
                    {
                        string cmd = $"venv/bin/python viever.py get_database_tables {databaseName}.db";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_columns"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_columns {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_data"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_data {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~                    
                    // TODO filler commands
                    if (command.Contains("add_data_to_table"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        
                        string cmd = $"venv/bin/python filler.py add_data_to_table {databaseName}.db {ar}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }
                    if (command.Contains("erase_data_from_table"))
                    {
                         string ar = "";
                         string[] arguments = command.Split(' ');
                         for (int i = 2; i < arguments.Length; ++i)
                             ar += arguments[i] + " ";
                         string cmd = $"venv/bin/python filler.py erase_data_from_table {databaseName}.db {ar}";
                         string output = Globals.RunCommandWithBash(cmd);
                         textView.Buffer.Text += output + "\n";                       
                    }
                    if (command.Contains("modify_data_in_table"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        string cmd = $"venv/bin/python filler.py modify_data_in_table {databaseName}.db {ar}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";                       
                    }
                    if (command.Contains("set_data_none_in_table"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        string cmd = $"venv/bin/python filler.py set_data_none_in_table {databaseName}.db {ar}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";                       
                    }
                }    
            }
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    // TODO filler commands
            if (Globals.ROLE == "devops")
            {
                if (command == "help")
                {
                    textView.Buffer.Text += $"\n> {command}\nCommand executed.";
                    textView.Buffer.Text += "\n\tget_database_table";
                    textView.Buffer.Text += "\n\tget_table_columns table1";
                    textView.Buffer.Text += "\n\tget_table_data table1";
                    textView.Buffer.Text += "\n\tget_structure";
                    textView.Buffer.Text += "\n\t~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";
                }
                else
                {
                    // viever
                    Console.WriteLine(command);
                    if (command == "get_structure")
                    {
                        Console.WriteLine($"Command is {command}");
                        string cmd = $"venv/bin/python viever.py get_structure {databaseName}.db";
                        string output = Globals.RunCommandWithBash(cmd);
                        Console.WriteLine("Output is:\"" + output + "\"");
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command == "get_database_tables")
                    {
                        string cmd = $"venv/bin/python viever.py get_database_tables {databaseName}.db";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_columns"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_columns {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_data"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_data {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~                    
                    // TODO filler commands
                    /*if (command.Contains("add_data_to_table"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        
                        string cmd = $"venv/bin/python migrate.py check_connectivity {databaseName}.db {ar}";
                        string output = Globals.RunCommandWithBash(cmd);
                        textView.Buffer.Text += output + "\n";
                    }*/
                    
                }
            }
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                    // TODO filler commands
            if (Globals.ROLE == "developer")
            {
                
            }
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            textView.Buffer.Text += $"\n>";
            commandEntry.Text = string.Empty;
        };

        var hbox = new HBox();
        hbox.PackStart(commandEntry, true, true, 5);
        hbox.PackStart(executeButton, false, false, 5);

        vbox.PackStart(hbox, false, false, 5);
        Add(vbox);
        ShowAll();
    }
}