using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Gtk;

public static class Globals {
    public static string LOGIN;
    public static string CURRENT_DATABASE;
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
    public string RunCommandWithBash(string command)
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
                if (RunCommandWithBash(scpCommand).Contains("NOT"))
                {
                    Console.WriteLine("Error");
                }
                else
                {
                    Console.WriteLine("Success");
                    Globals.LOGIN = usernameEntry.Text;
                    var databaseWindow = new DatabaseSelectionWindow();
                    databaseWindow.ShowAll();
                    Destroy();
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
    public string RunCommandWithBash(string command)
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
        string input = RunCommandWithBash(command);
        
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
    public string RunCommandWithBash(string command)
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
        
        executeButton.Clicked += (sender, e) =>
        {
            var command = commandEntry.Text;
            if (command == "help") {
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
                    string output = RunCommandWithBash(cmd);
                    Console.WriteLine("Output is:\"" + output + "\"");
                    textView.Buffer.Text += output + "\n";
                }
                if (command == "get_database_tables")
                {
                    string cmd = $"venv/bin/python viever.py get_database_tables {databaseName}.db";
                    string output = RunCommandWithBash(cmd);
                    //Console.WriteLine("Output is:\"" + output + "\"");
                    textView.Buffer.Text += output + "\n";
                }

                if (command.Contains("get_table_columns"))
                {
                    string[] arguments = command.Split(' ');
                    
                    string cmd = $"venv/bin/python viever.py get_table_columns {databaseName}.db {arguments[1]}";
                    string output = RunCommandWithBash(cmd);
                    //Console.WriteLine("Output is:\"" + output + "\"");
                    textView.Buffer.Text += output + "\n";
                }
                if (command.Contains("get_table_data"))
                {
                    string[] arguments = command.Split(' ');
                    string cmd = $"venv/bin/python viever.py get_table_data {databaseName}.db {arguments[1]}";
                    string output = RunCommandWithBash(cmd);
                    //Console.WriteLine("Output is:\"" + output + "\"");
                    textView.Buffer.Text += output + "\n";
                }
            }

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