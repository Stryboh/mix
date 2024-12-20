using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Gtk;

public static class Globals
{
    public static string LOGIN;
    public static string ROLE;
    public static string SERVER_HOST = "127.0.0.1"; // Замените на адрес сервера
    public static int SERVER_PORT = 8888;
    
    public static string RunCommandWithSocket(string command)
    {
        try
        {
            using (var client = new UdpClient())
            {
                var serverEndpoint = new IPEndPoint(IPAddress.Parse(SERVER_HOST), SERVER_PORT);
                byte[] sendBuffer = Encoding.UTF8.GetBytes(command);

                // Отправка команды на сервер
                client.Send(sendBuffer, sendBuffer.Length, serverEndpoint);

                // Получение ответа от сервера
                var serverResponse = client.Receive(ref serverEndpoint);
                string response = Encoding.UTF8.GetString(serverResponse);
                return response;
            }
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

class LoginWindow : Window
{

    public LoginWindow() : base("Login")
    {
        SetDefaultSize(300, 200);
        SetPosition(WindowPosition.Center);

        var vbox = new VBox();
        var usernameEntry = new Entry { PlaceholderText = "Username" };
        var socketIpEntry = new Entry { PlaceholderText = "Socket Ip" };
        var socketPortEntry = new Entry { PlaceholderText = "Socket Port" };
        var passwordEntry = new Entry { PlaceholderText = "Password", Visibility = false };
        var loginButton = new Button("Login");
        string configFilePath = "config_socket_srv.txt";
        
        if (File.Exists(configFilePath))
        {
            string[] lines = File.ReadAllLines(configFilePath);
            Globals.SERVER_HOST = lines[0];
            Globals.SERVER_PORT = int.Parse(lines[1]);
            socketIpEntry.Text = Globals.SERVER_HOST;
            socketPortEntry.Text = Globals.SERVER_PORT.ToString();
        }
        else
        {
            Globals.SERVER_HOST = "127.0.0.1";
            Globals.SERVER_PORT = 8888;
            socketIpEntry.Text = Globals.SERVER_HOST;
            socketPortEntry.Text = Globals.SERVER_PORT.ToString();

            // Значения по умолчанию, если файл отсутствует
        }        loginButton.Clicked += (sender, e) =>
        {
            
            Globals.SERVER_HOST = socketIpEntry.Text ;
            Globals.SERVER_PORT = int.Parse(socketPortEntry.Text);
            File.WriteAllText(configFilePath, Globals.SERVER_HOST + "\n" + Globals.SERVER_PORT);

            string command = $"venv/bin/python viever.py check_login {usernameEntry.Text} {passwordEntry.Text}";
            try
            {
                if (Globals.RunCommandWithSocket(command).Contains("client"))
                {
                    Globals.ROLE = "client";
                    Console.WriteLine("Hello, client");
                    Globals.LOGIN = usernameEntry.Text;
                    var databaseWindow = new DatabaseSelectionWindow();
                    databaseWindow.ShowAll();
                    Destroy();
                }

                else if (Globals.RunCommandWithSocket(command).Contains("moderator"))
                {
                    Globals.ROLE = "moderator";
                    Console.WriteLine("Hello, moderator");
                    Globals.LOGIN = usernameEntry.Text;
                    var databaseWindow = new DatabaseSelectionWindow();
                    databaseWindow.ShowAll();
                    Destroy();
                }

                else if (Globals.RunCommandWithSocket(command).Contains("devops"))
                {
                    Globals.ROLE = "devops";
                    Console.WriteLine("Hello, devops");
                    Globals.LOGIN = usernameEntry.Text;
                    var mainAppWindow = new MainAppWindow(null);
                    mainAppWindow.ShowAll();
                    Destroy();
                    
                }

                else if (Globals.RunCommandWithSocket(command).Contains("developer"))
                {
                    Globals.ROLE = "developer";
                    Console.WriteLine("Hello, developer");
                    Globals.LOGIN = usernameEntry.Text;
                    var databaseWindow = new DatabaseSelectionWindow();
                    databaseWindow.ShowAll();
                    Destroy();
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }
            catch (Exception ex)
            {
            }
        };

        vbox.PackStart(usernameEntry, false, false, 5);
        vbox.PackStart(passwordEntry, false, false, 5);
        vbox.PackStart(loginButton, false, false, 5);

        vbox.PackStart(socketIpEntry, false, false, 5);
        vbox.PackStart(socketPortEntry, false, false, 5);
        Add(vbox);
        ShowAll();
    }
}

class DatabaseSelectionWindow : Window
{

    static string[] ParseStrings(string input)
    {
        string pattern = @"'([^']*)'";
        MatchCollection matches = Regex.Matches(input, pattern);
        List<string> extractedStrings = new List<string>();
        foreach (Match match in matches)
            extractedStrings.Add(match.Groups[1].Value);
        return extractedStrings.ToArray();
    }

    public DatabaseSelectionWindow() : base("Select Database")
    {
        SetDefaultSize(400, 300);
        SetPosition(WindowPosition.Center);
        string command = $"venv/bin/python viever.py get_available_databases {Globals.LOGIN}";
        string input = Globals.RunCommandWithSocket(command);
        string[] result = ParseStrings(input);

        var vbox = new VBox();
        foreach (var db in result)
        {
            var button = new Button(db);
            button.Clicked += (sender, e) =>
            {
                var mainAppWindow = new MainAppWindow(db);
                mainAppWindow.ShowAll();
                Destroy();
            };
            vbox.PackStart(button, false, false, 5);
        }

        var logoutButton = new Button("Logout");
        logoutButton.Clicked += (sender, e) =>
        {
            var loginAppWindow = new LoginWindow();
            loginAppWindow.ShowAll();
            Destroy();
        };
        vbox.PackStart(logoutButton , false, false, 5);
        Add(vbox);
        ShowAll();
    }
}

class MainAppWindow : Window
{
    public MainAppWindow(string databaseName) : base($"Main App - {databaseName}")
    {
        SetDefaultSize(600, 400);
        SetPosition(WindowPosition.Center);

        var vbox = new VBox();
        var textView = new TextView { Editable = false };
        textView.Buffer.Text = $"Connected to {databaseName}";
        vbox.PackStart(textView, true, true, 5);
        var commandEntry = new Entry { PlaceholderText = "Enter command..." };
        var executeButton = new Button("Execute");
        executeButton.Clicked += (sender, e) =>
        {
            var command = commandEntry.Text;
            if (Globals.ROLE == "client")
            {
                if (command == "help")
                {
                    textView.Buffer.Text = "";
                    textView.Buffer.Text += $"\n> {command}\nCommand executed.";
                    textView.Buffer.Text += "\n\tget_database_tables";
                    textView.Buffer.Text += "\n\tget_table_columns <table_name>";
                    textView.Buffer.Text += "\n\tget_table_data <table_name>";
                    textView.Buffer.Text += "\n\tget_structure";
                }
                else
                {
                    if (command == "get_structure")
                    {
                        string cmd = $"venv/bin/python viever.py get_structure {databaseName}.db";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command == "get_database_tables")
                    {
                        string cmd = $"venv/bin/python viever.py get_database_tables {databaseName}.db";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_columns"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_columns {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_data"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_data {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }
                }    
            }
            
 
            if (Globals.ROLE == "moderator")
            {
                if (command == "help")
                {
                    textView.Buffer.Text = "";
                    textView.Buffer.Text += $"\n> {command}\nCommand executed.";
                    textView.Buffer.Text += "\n\tget_database_tables";
                    textView.Buffer.Text += "\n\tget_table_columns <table_name>";
                    textView.Buffer.Text += "\n\tget_table_data <table_name>";
                    textView.Buffer.Text += "\n\tget_structure";
                    textView.Buffer.Text += "\n\tadd_data_to_table <table_name> <columns> <data>";
                    textView.Buffer.Text += "\n\terase_data_from_table <table_name> [condition]";
                    textView.Buffer.Text += "\n\tmodify_data_in_table <table_name> <column> <value> [condition]";
                    textView.Buffer.Text += "\n\tset_data_none_in_table <table_name> <column> [condition]";
                }
                else
                {
                    if (command == "get_structure")
                    {
                        string cmd = $"venv/bin/python viever.py get_structure {databaseName}.db";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command == "get_database_tables")
                    {
                        string cmd = $"venv/bin/python viever.py get_database_tables {databaseName}.db";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_columns"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_columns {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_data"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_data {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    
                    if (command.Contains("add_data_to_table"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        
                        string cmd = $"venv/bin/python filler.py add_data_to_table {databaseName}.db {ar}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }
                    if (command.Contains("erase_data_from_table"))
                    {
                         string ar = "";
                         string[] arguments = command.Split(' ');
                         for (int i = 2; i < arguments.Length; ++i)
                             ar += arguments[i] + " ";
                         string cmd = $"venv/bin/python filler.py erase_data_from_table {databaseName}.db {ar}";
                         string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                         textView.Buffer.Text += output + "\n";                       
                    }
                    if (command.Contains("modify_data_in_table"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        string cmd = $"venv/bin/python filler.py modify_data_in_table {databaseName}.db {ar}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";                       
                    }
                    if (command.Contains("set_data_none_in_table"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        string cmd = $"venv/bin/python filler.py set_data_none_in_table {databaseName}.db {ar}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";                       
                    }
                }    
            }
                    
            if (Globals.ROLE == "devops")
            {
                if (command == "help")
                {
                    textView.Buffer.Text = "";
                    textView.Buffer.Text += $"\n> {command}\nCommand executed.";
                    textView.Buffer.Text += "\n\tget_database_tables";
                    textView.Buffer.Text += "\n\tget_table_columns <table_name>";
                    textView.Buffer.Text += "\n\tget_table_data <table_name>";
                    textView.Buffer.Text += "\n\tget_structure";
                    textView.Buffer.Text += "\n\tcheck_connectivity <file_1_path> <table_1_name> <column_1_name> <file_2_path> <table_2_name> <column_2_name>";
                    textView.Buffer.Text += "\n\ttransfer_data <old_file_path> <old_table_name> <old_column_name> <new_file_path> <new_table_name> <new_column_name>";
                    textView.Buffer.Text += "\n\tauto_transfer_data <old_file_path> <new_file_path>";
                }
                else
                {
                    if (command == "get_structure")
                    {
                        string cmd = $"venv/bin/python viever.py get_structure {databaseName}.db";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command == "get_database_tables")
                    {
                        string cmd = $"venv/bin/python viever.py get_database_tables {databaseName}.db";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_columns"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_columns {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }

                    if (command.Contains("get_table_data"))
                    {
                        string[] arguments = command.Split(' ');
                        string cmd = $"venv/bin/python viever.py get_table_data {databaseName}.db {arguments[1]}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }
                    
                    if (command.Contains("check_connectivity"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        
                        string cmd = $"venv/bin/python migrate.py check_connectivity {ar}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }
                    
                    if (command.Contains("transfer_data"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        string cmd = $"venv/bin/python migrate.py transfer_data {ar}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }
                    
                    if (command.Contains("auto_transfer_data"))
                    {
                        string ar = "";
                        string[] arguments = command.Split(' ');
                        for (int i = 2; i < arguments.Length; ++i)
                            ar += arguments[i] + " ";
                        string cmd = $"venv/bin/python migrate.py auto_transfer_data {ar}";
                        string output = Globals.RunCommandWithSocket(cmd);
                        textView.Buffer.Text = "";
                        textView.Buffer.Text += output + "\n";
                    }                   
                }
            }
            //TODO
            if (Globals.ROLE == "developer")
            {
            }
            textView.Buffer.Text += $"\n>";
            commandEntry.Text = string.Empty;
        };
        var backButton = new Button("Back");
        backButton.Clicked += (sender, e) =>
        {
            var DatabaseSelectionAppWindow = new DatabaseSelectionWindow();
            DatabaseSelectionAppWindow.ShowAll();
            Destroy();
        };
        vbox.PackStart(backButton , false, false, 5);
        Add(vbox);
        ShowAll();
        
        
        var hbox = new HBox();
        hbox.PackStart(commandEntry, true, true, 5);
        hbox.PackStart(executeButton, false, false, 5);
        vbox.PackStart(hbox, false, false, 5);
        Add(vbox);
        ShowAll();
    }
}
