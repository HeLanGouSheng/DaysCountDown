using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace CountdownDay
{
    public class EventItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "新倒数日";
        public DateTime TargetDate { get; set; } = DateTime.Today.AddDays(7);

        // 显示真实剩余天数（到期后显示负数）
        public int TotalDaysRemaining
        {
            get => (TargetDate.Date - DateTime.Today).Days;
        }

        // 是否已到期
        public bool IsExpired => TotalDaysRemaining < 0;

        // 计算工作日天数
        public int WorkDaysRemaining
        {
            get
            {
                DateTime start = DateTime.Today;
                DateTime end = TargetDate.Date;

                if (end < start)
                {
                    // 到期后计算负向工作日
                    int negativeDays = 0;
                    for (DateTime date = end; date < start; date = date.AddDays(1))
                    {
                        if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        {
                            negativeDays--;
                        }
                    }
                    return negativeDays;
                }

                int workDays = 0;
                for (DateTime date = start; date < end; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        workDays++;
                    }
                }
                return workDays;
            }
        }
    }

    public class AppDataContainer
    {
        public double BackgroundOpacity { get; set; } = 0.9;
        public double TextOpacity { get; set; } = 1.0;
        public bool ShowInTaskbar { get; set; } = true;
        public string Language { get; set; } = "zh-CN"; // 新增：保存语言类型 ("zh-CN" 或 "en-US")
        public List<EventItem> Events { get; set; } = new();
    }

    public static class StorageManager
    {
        private static readonly string FolderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "CountdownDay"
        );
        private static readonly string FilePath = Path.Combine(FolderPath, "data.json");

        public static AppDataContainer LoadData()
        {
            try
            {
                if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
                if (!File.Exists(FilePath)) return GetDefaultData();

                string json = File.ReadAllText(FilePath);
                var data = JsonSerializer.Deserialize<AppDataContainer>(json);
                return data ?? GetDefaultData();
            }
            catch
            {
                return GetDefaultData();
            }
        }

        public static void SaveData(AppDataContainer data)
        {
            try
            {
                if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存失败: {ex.Message}");
            }
        }

        private static AppDataContainer GetDefaultData()
        {
            return new AppDataContainer
            {
                BackgroundOpacity = 0.9,
                TextOpacity = 1.0,
                Events = new List<EventItem>
                {
                    new EventItem { Title = "发布到 GitHub", TargetDate = DateTime.Today.AddDays(10) }
                }
            };
        }
    }
}