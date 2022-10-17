﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grant.Core.Services.Impl
{
    public class TaskService : ITaskService
    {
        public async Task<string> AutoBackupTask()
        {
            string result = "";
            string time = DateTime.Now.ToString("ddMMyy_HHmm");
            string dbName = "\"grant7\"";
            string path = "\"C:/backups/grant";

            //System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = @"C:/Program Files/PostgreSQL/9.4/bin/pg_dump.exe";
            startInfo.Arguments = "--host DbServer --port 5432 --username \"postgres\" -w --format tar --blobs --verbose --file " + path + time + ".backup\" " + dbName;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardError)
                {
                    // если файл бекапа не создается и надо получить ошибку, возвращаемую pg_dump
                    result = reader.ReadToEnd();
                   
                    //_smsSender.SendTelegram($"Создание резервной копии бд:{path + time}.backup");
                }
            }

            return result;

        }
    }
}