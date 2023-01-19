using Newtonsoft.Json;
using Plugin.Maui.Audio;
using System.Globalization;
using System.Text;

namespace MarioMAUI
{
    public class HANDMADE
    {
        private int ind;
        string taskStr;
        string answerStr;
        public string level;
        public char operation;
        public float[] a;
        public float[] b;
        public byte[] dARGB;
        public byte[] lARGB;
        public byte[] tARGB;
        public byte[] aARGB;
        public Color dBrush;
        public Color lBrush;
        public Color tBrush;
        public Color aBrush;

        CultureInfo culture; // <-по документации
        string specifier = "F";

        public HANDMADE()
        {
            culture = CultureInfo.CreateSpecificCulture("ru_RU");
            dARGB = new byte[] { 255, 0, 0, 0 };
            lARGB = new byte[] { 255, 0, 0, 255 };
            tARGB = new byte[] { 255, 255, 0, 0 };
            aARGB = new byte[] { 255, 0, 255, 0 };
        }

        public void ResetInd()
        {
            ind = 0;
        }
        public void dataTuning()
        {
            //if (data.Length % 2 == 1)
            //{
            //    Array.Resize(ref data, data.Length + 1);
            //    data[data.Length - 1] = 1; // чтобы цифра не пропадала)
            //}
        }
        public (int n, string taskStr, string answerStr) GetNextTask()
        {
            int i = ind;
            ind++;
            float c = 0;
            if (ind <= a.Length)
            {
                taskStr = a[i].ToString() + operation + b[i].ToString() + "=";
                switch (operation)
                {
                    case '+':
                        c = a[i] + b[i];
                        break;
                    case '-':
                        c = a[i] - b[i];
                        break;
                    case '*':
                        c = a[i] * b[i];
                        break;
                    case '/':
                        c = a[i] / b[i];
                        break;
                }
                answerStr = c.ToString(specifier, culture); // можно поменять количество знаков после запятой в настройках Windows (начиналось в Windows)
                answerStr = answerStr.TrimEnd('0');
                answerStr = answerStr.TrimEnd('.');
            }
            else
            {
                i = -1;
                ind = 0;
            }
            return (i, taskStr, answerStr);
        }
    }

    // Первоначально данные предполагались случайными.
    // Потом стало понятно, что удобнее управлять данными из внешнего файла и иметь предопределённые задания.
    // Появился файл Task.json и class HANDMADE.
    public class RNDM
    {
        public RNDM()
        {
            dARGB = new byte[] { 255, 0, 0, 0 };
            lARGB = new byte[] { 255, 0, 0, 255 };
            tARGB = new byte[] { 255, 255, 0, 0 };
            aARGB = new byte[] { 255, 0, 255, 0 };
        }

        public string level;
        public char operation;
        public int min;
        public int max;
        public int count;
        public byte[] dARGB;
        public byte[] lARGB;
        public byte[] tARGB;
        public byte[] aARGB;
    }

    public class Task
    {
        public int iLevel;
        public string App { get; set; }
        public string Ver { get; set; }
        public string PlayNo { get; set; }
        public string PlayYes { get; set; }
        public string PlayTada { get; set; }
        public string PlayTadadadaaam { get; set; }
        public DateTime Date { get; set; }

        public HANDMADE[] HandMadeTask;
        public RNDM[] RndmTask;
        public HANDMADE[] tasks;

        public int Initial()
        {
            tasks = new HANDMADE[HandMadeTask.Length + RndmTask.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = new HANDMADE();
            }
            HandMadeTask.CopyTo(tasks, 0);

            Random random = new Random();

            for (int j = HandMadeTask.Length, i = 0; i < RndmTask.Length; j++, i++)
            {
                tasks[j].level = RndmTask[i].level;
                tasks[j].operation = RndmTask[i].operation;
                tasks[j].a = new float[RndmTask[i].count];
                tasks[j].b = new float[RndmTask[i].count];
                for (int k = 0; k < RndmTask[i].count; k++)
                {
                    tasks[j].a[k] = random.Next(RndmTask[i].min, RndmTask[i].max);
                    tasks[j].b[k] = random.Next(RndmTask[i].min, RndmTask[i].max);
                }
                for (int k = 0; k < 4; k++)
                {
                    tasks[j].dARGB[k] = RndmTask[i].dARGB[k];
                    tasks[j].lARGB[k] = RndmTask[i].lARGB[k];
                    tasks[j].tARGB[k] = RndmTask[i].tARGB[k];
                    tasks[j].aARGB[k] = RndmTask[i].aARGB[k];
                }
            }
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i].dBrush = Color.FromRgba(tasks[i].dARGB[1], tasks[i].dARGB[2], tasks[i].dARGB[3], tasks[i].dARGB[0]);
                tasks[i].lBrush = Color.FromRgba(tasks[i].lARGB[1], tasks[i].lARGB[2], tasks[i].lARGB[3], tasks[i].lARGB[0]);
                tasks[i].tBrush = Color.FromRgba(tasks[i].tARGB[1], tasks[i].tARGB[2], tasks[i].tARGB[3], tasks[i].tARGB[0]);
                tasks[i].aBrush = Color.FromRgba(tasks[i].aARGB[1], tasks[i].aARGB[2], tasks[i].aARGB[3], tasks[i].aARGB[0]);
            }
            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].a.Length != tasks[i].b.Length)
                {
                    return -1;
                }
            }

            return 0;
        }
        public (int n, string taskStr, string answerStr, string buff) GetNextTask()
        {
            int i = -2;
            string taskStr = "";
            string answerStr = "";

            if (iLevel < tasks.Length)
            {
                (i, taskStr, answerStr) = tasks[iLevel].GetNextTask();
                if (i == -1) // коряво
                {
                    iLevel++;
                    if (iLevel < tasks.Length)
                    {
                        (i, taskStr, answerStr) = tasks[iLevel].GetNextTask();
                    }
                }
            }
            StringBuilder sb = new StringBuilder(answerStr);
            for (int j = 0; j < answerStr.Length; j++) sb[j] = '*';
            string buff = sb.ToString();

            return (i, taskStr, answerStr, buff);
        }
    }

    public partial class MainPage : ContentPage
    {
        Task task;

        int n;
        string taskStr;
        string buff;
        string answerStr;
        int indAnsw;
        string levelStr;

        // https://www.youtube.com/watch?v=oIYnEuZ9oew - Thumbs up.
        private readonly IAudioManager audioManager;
        IAudioPlayer PlayNo;
        IAudioPlayer PlayYes;
        IAudioPlayer PlayTada;
        IAudioPlayer PlayTadadadaaam;

        bool flagEnd = false;

        public MainPage(IAudioManager audioManager)
        {
            InitializeComponent();
            this.audioManager = audioManager;
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            NextTask();

            Next.BackgroundColor = Colors.Gray;
            Next.TextColor = Colors.Black;
            Next.IsEnabled = false;
        }
        private async void LoadTask()
        {
            try
            {
                using System.IO.Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Task.json");
                using var reader = new StreamReader(fileStream);
                task = JsonConvert.DeserializeObject<Task>(reader.ReadToEnd());

                verSTR.Text = task.Ver;

                int r = task.Initial();
                if (r == -1)
                {
                    taskSTR.Text = "Error JSON";
                    answerSTR.Text = "";
                    return;
                }

                PlayNo = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(task.PlayNo));
                PlayYes = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(task.PlayYes));
                PlayTada = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(task.PlayTada));
                PlayTadadadaaam = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(task.PlayTadadadaaam));

                task.iLevel = Preferences.Get("iLevel", 0);
                if (task.iLevel >= task.tasks.Length) task.iLevel = task.tasks.Length - 1;

                (n, taskStr, answerStr, buff) = task.GetNextTask();

                levelStr = task.tasks[task.iLevel].level;

                lvl.TextColor = task.tasks[task.iLevel].lBrush;
                level.TextColor = task.tasks[task.iLevel].lBrush;
                levelExt.TextColor = task.tasks[task.iLevel].lBrush;
                level.Text = levelStr;
                levelExt.Text = "(" + task.tasks[task.iLevel].a.Length.ToString() + "\\" + (n + 1).ToString() + ")";

                desk.Background = task.tasks[task.iLevel].dBrush;

                taskSTR.TextColor = task.tasks[task.iLevel].tBrush;
                taskSTR.Text = taskStr;
                answerSTR.TextColor = task.tasks[task.iLevel].aBrush;
                answerSTR.Text = buff;
            }
            catch (Exception ex)
            {
                lvl.Text = "";
                taskSTR.Text = "Error";
                answerSTR.Text = "";
                verSTR.Text = ex.Message;
            }
        }

        private void NextTask()
        {
            if (flagEnd) { return; }

            (n, taskStr, answerStr, buff) = task.GetNextTask();

            if (n == 0)
            {
                PlayTada.Play();
            }

            if (task.iLevel < task.tasks.Length)
            {
                Preferences.Set("iLevel", task.iLevel);

                desk.Background = task.tasks[task.iLevel].dBrush;

                lvl.TextColor = task.tasks[task.iLevel].lBrush;
                level.TextColor = task.tasks[task.iLevel].lBrush;
                levelExt.TextColor = task.tasks[task.iLevel].lBrush;
                level.Text = task.tasks[task.iLevel].level;
                levelExt.Text = "(" + task.tasks[task.iLevel].a.Length.ToString() + "\\" + (n + 1).ToString() + ")";

                taskSTR.TextColor = task.tasks[task.iLevel].tBrush;
                taskSTR.Text = taskStr;

                indAnsw = 0;

                answerSTR.TextColor = task.tasks[task.iLevel].aBrush;
                answerSTR.Text = buff;
            }
            else
            {
                level.Text = "--";
                levelExt.Text = "";

                taskSTR.Text = "УЧИ ДИФУРЫ";
                answerSTR.Text = "!!";

                verSTR.Text = "www.mariomaui.ru";
                flagEnd = true;
                PlayTadadadaaam.Play();
            }
        }

        //---------------------------------------------------------------------------------------------
        // ну, ладно...
        private void Button_Clicked_1(object sender, EventArgs e) { NewChar('1'); }
        private void Button_Clicked_2(object sender, EventArgs e) { NewChar('2'); }
        private void Button_Clicked_3(object sender, EventArgs e) { NewChar('3'); }
        private void Button_Clicked_4(object sender, EventArgs e) { NewChar('4'); }
        private void Button_Clicked_5(object sender, EventArgs e) { NewChar('5'); }
        private void Button_Clicked_6(object sender, EventArgs e) { NewChar('6'); }
        private void Button_Clicked_7(object sender, EventArgs e) { NewChar('7'); }
        private void Button_Clicked_8(object sender, EventArgs e) { NewChar('8'); }
        private void Button_Clicked_9(object sender, EventArgs e) { NewChar('9'); }
        private void Button_Clicked_minus(object sender, EventArgs e) { NewChar('-'); }
        private void Button_Clicked_0(object sender, EventArgs e) { NewChar('0'); }
        private void Button_Clicked_dot(object sender, EventArgs e) { NewChar('.'); }
        //---------------------------------------------------------------------------------------------

        private void NewChar(char c)
        {
            if (buff == null) return;
            if (indAnsw >= buff.Length) return;

            StringBuilder sbAnsw = new StringBuilder(answerStr);
            StringBuilder sb = new StringBuilder(buff);
            if (c == sbAnsw[indAnsw])
            {
                sb[indAnsw] = sbAnsw[indAnsw];
                if (indAnsw < buff.Length) indAnsw++;
                PlayYes.Play();
            }
            else
            {
                sb[indAnsw] = '*';
                PlayNo.Play();
            }

            buff = sb.ToString();
            answerSTR.Text = sb.ToString();

            if (indAnsw == buff.Length)
            {
                Next.BackgroundColor = Colors.Blue;
                Next.TextColor = Colors.Red;
                Next.IsEnabled = true;
            }
        }

        private void VerticalStackLayout_Loaded(object sender, EventArgs e)
        {
            LoadTask();
        }
    }
}


