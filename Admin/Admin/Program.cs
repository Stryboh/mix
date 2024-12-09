using Gtk;
using System.Diagnostics;

public class Admin : Window
{
    private TextView textView;
    private Button sendButton;
    private Button configButton;
    private string filePath = "data.txt";
    private string configFilePath = "config.txt";

    private string remoteHost = "";
    private string remoteUser = "";
    private string remotePassword = "";

    public Admin() : base("Редактор и отправка файла")
    {
        SetDefaultSize(600, 400);
        SetPosition(WindowPosition.Center);

        LoadConfig();

        var vbox = new VBox();
        textView = new TextView
        {
            WrapMode = WrapMode.Word
        };
        vbox.PackStart(textView, true, true, 0);

        sendButton = new Button("Отправить файл");
        sendButton.Clicked += OnSendButtonClicked;
        vbox.PackStart(sendButton, false, false, 10);

        configButton = new Button("Настройки");
        configButton.Clicked += OnConfigButtonClicked;
        vbox.PackStart(configButton, false, false, 10);

        Add(vbox);
        ShowAll();

        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            textView.Buffer.Text = content;
        }
        else
            File.WriteAllText(filePath, string.Empty);
    }

    private void LoadConfig()
    {
        if (File.Exists(configFilePath))
        {
            string[] configLines = File.ReadAllLines(configFilePath);
            if (configLines.Length >= 3)
            {
                remoteHost = configLines[0];
                remoteUser = configLines[1];
                remotePassword = configLines[2];
            }
        }
    }

    private void SaveConfig()
    {
        try
        {
            File.WriteAllLines(configFilePath, new string[] { remoteHost, remoteUser, remotePassword });
            Console.WriteLine("Конфигурация сохранена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при сохранении конфигурации: " + ex.Message);
        }
    }

    private void OnSendButtonClicked(object sender, EventArgs e)
    {
        string content = textView.Buffer.Text;
        try
        {
            File.WriteAllText(filePath, content);
            UploadFile();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при сохранении файла: " + ex.Message);
        }
    }

    private void OnConfigButtonClicked(object sender, EventArgs e)
    {
        var configWindow = new Window("Настройки")
        {
            DefaultSize = new Gdk.Size(300, 200),
            WindowPosition = WindowPosition.Center
        };

        var vbox = new VBox();

        var hostEntry = new Entry { Text = remoteHost };
        vbox.PackStart(new Label("IP:"), false, false, 0);
        vbox.PackStart(hostEntry, false, false, 10);

        var userEntry = new Entry { Text = remoteUser };
        vbox.PackStart(new Label("User:"), false, false, 0);
        vbox.PackStart(userEntry, false, false, 10);

        var passwordEntry = new Entry { Text = remotePassword, Visibility = false };
        vbox.PackStart(new Label("Password:"), false, false, 0);
        vbox.PackStart(passwordEntry, false, false, 10);

        var saveButton = new Button("Сохранить");
        saveButton.Clicked += (sender, e) =>
        {
            remoteHost = hostEntry.Text;
            remoteUser = userEntry.Text;
            remotePassword = passwordEntry.Text;
            SaveConfig();
            configWindow.Destroy();
        };
        vbox.PackStart(saveButton, false, false, 10);

        configWindow.Add(vbox);
        configWindow.ShowAll();
    }

    private void UploadFile()
    {
        string remotePath = "~/Proga/data.txt";

        string scpCommand = $"sshpass -p \"{remotePassword}\" scp {filePath} {remoteUser}@{remoteHost}:{remotePath}";

        try
        {
            Process process = new Process();
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{scpCommand}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            process.OutputDataReceived += (sender, e) => { Console.WriteLine(e.Data); };
            process.ErrorDataReceived += (sender, e) => { Console.WriteLine(e.Data); };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode == 0)
                Console.WriteLine("Файл успешно отправлен на сервер.");
            else
                Console.WriteLine("Ошибка при отправке файла.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при отправке файла: " + ex.Message);
        }
    }

    public static void Main(string[] args)
    {
        Application.Init();
        new Admin();
        Application.Run();
    }
}
