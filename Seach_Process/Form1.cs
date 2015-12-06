using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using System.Runtime.InteropServices;

using System.IO;

using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;



namespace Seach_Process
{
    public partial class Form1 : Form
    {
        public bool flag_cbank = false;
        public bool flag_rts = false;

          [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(
                 string lpClassName,
                 string lpWindowName
            );

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(
               IntPtr hWndParent,
               IntPtr hWndChild,
               string lpszClass,
               string lpszWindow
          );

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(
               IntPtr hWnd,
               int uCmd
          );


        [DllImport("user32.dll")]
          public static extern int GetWindowText(
                 IntPtr hWnd,
                 StringBuilder lpString,
                 int nMaxCount
            );

        [DllImport("user32.dll")]
        public static extern int GetClassName(
               IntPtr hWnd,
               StringBuilder lpClassName,
               int nMaxCount            
          );
        



        public Form1()
        {
            bool flag_cbank=false;
            bool flag_rts = false;

            InitializeComponent();
         //   this.ShowInTaskbar = false;
        }


        private void SendEmail(string str,int i)
        {
            //Авторизация на SMTP сервере
            SmtpClient Smtp = new SmtpClient("ip", 25);
            Smtp.Credentials = new NetworkCredential("user", "pass");
            //Smtp.EnableSsl = false;

            //Формирование письма
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress("from");
            Message.To.Add(new MailAddress("to"));

            if (i==0) 
            Message.Subject = "[РОБОТ] Система мониторинга за процессами. Пропал процесс";
            else
            Message.Subject = "[РОБОТ] Система мониторинга за процессами. Процесс восстановлен";
            Message.Body = str;

            //Прикрепляем файл
            // string file = "C:\\file.zip";
            //Attachment attach = new Attachment(file, MediaTypeNames.Application.Octet);

            // Добавляем информацию для файла
            //ContentDisposition disposition = attach.ContentDisposition;
            //disposition.CreationDate = System.IO.File.GetCreationTime(file);
            //disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            //disposition.ReadDate = System.IO.File.GetLastAccessTime(file);

            //            Message.Attachments.Add(attach);

            Smtp.Send(Message);//отправка
        }



        public void Seach_Error(int i)
        {
            string HostName = Environment.MachineName;
            if ((i == 1)&&(flag_cbank==false))
            {
                SendEmail("Пропал процесс CBMAIN.EX на сервере " + HostName + "! Проверьте сервис!!!",0);
                flag_cbank = true;
            }
            if ((i == 2)&&(flag_rts==false))
            {
                SendEmail("Пропал процесс RTS.EXE на сервере " + HostName + "! Проверьте сервис!!!",0);
                flag_rts = true;
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
   
            
      //      procs = Process.GetProcessesByName("cbmain1.ex"); if (procs.Length == 0) Seach_Error(1);

     //       procs = Process.GetProcessesByName("rts.exe"); if (procs.Length == 0) Seach_Error(2);

            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Process[] procs1;
            procs1 = Process.GetProcessesByName("cbmain.ex");
            if (procs1.Length == 0)
            {
                Seach_Error(1);
            }
            else if (flag_cbank == true)
            {
                flag_cbank = false;
                string HostName = Environment.MachineName;
                SendEmail("Процесс CBANK.EXE на сервере " + HostName + " восстановлен!", 1);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Монитор сервиса Cbmain.ex")
            {
                timer1.Interval = Convert.ToInt16(numericUpDown1.Value) * 1000 * 60;
                timer1.Enabled = true;
                button1.Text = "Остановить Монитор сервиса Cbmain.ex";
            }
            else
            {
                timer1.Enabled = false;
                button1.Text = "Монитор сервиса Cbmain.ex";
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Process[] procs2;
            procs2 = Process.GetProcessesByName("rts");
            if (procs2.Length == 0)
            {
                Seach_Error(2);
            }
            else if (flag_rts == true)
            {
                flag_rts = false;
                string HostName = Environment.MachineName;
                SendEmail("Процесс RTS.EXE на сервере " + HostName + " восстановлен!", 1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Монитор сервиса Rts.exe")
            {
                timer2.Interval = Convert.ToInt16(numericUpDown1.Value) * 1000 * 60;
                timer2.Enabled = true;
                button2.Text = "Остановить Монитор сервиса Rts.exe";
            }
            else
            {
                timer2.Enabled = false;
                button2.Text = "Монитор сервиса Rts.exe";
            }
        }



        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                {
                    this.ShowInTaskbar=false;
                    notifyIcon1.Visible=true;
                }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IntPtr iHandle, iHandle2, iHandle3;
                int i = 0;
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            StringBuilder Buff2 = new StringBuilder(nChars);
            iHandle = FindWindow(null, "Текущие сессии");
            iHandle2 = FindWindowEx(iHandle, IntPtr.Zero, "TPanel", null);
             i = GetWindowText(iHandle, Buff, nChars);
            do {
            //    iHandle2=GetWindow(iHandle, 7);

                iHandle3 = FindWindowEx(iHandle2, IntPtr.Zero, "TLabel", null);
                i = GetClassName(iHandle3,Buff2, nChars);
                i = GetWindowText(iHandle3, Buff, nChars);
                MessageBox.Show(Buff2.ToString() + "  " + Buff.ToString());

            }
            while (Buff.ToString()!="6");


       //      iHandle2=FindWindowEx(iHandle,null,"Button",0);

        //     int i = GetWindowText(iHandle2, Buff, nChars);
        //    i = GetClassName(iHandle,"Button", nChars);
           //  i = GetWindowText(iHandle, Buff, nChars);
            
            MessageBox.Show(Convert.ToString(iHandle));

        //    HWND Wnd;
        //    Wnd = FindWindow(null, "SQL Navigator");

        }



    }
}