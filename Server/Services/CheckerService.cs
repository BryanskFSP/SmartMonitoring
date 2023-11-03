using Quartz;
using Quartz.Impl;
using SmartMonitoring.Server.Jobs;

namespace SmartMonitoring.Server.Services;

public class CheckerService
{
    private JobFactory JobFactory;

    public CheckerService(JobFactory jobFactory)
    {
        JobFactory = jobFactory;
    }

    public async Task Start()
    {
        IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();
        scheduler.JobFactory = JobFactory;

        IJobDetail job = JobBuilder.Create<CheckerJob>().Build();
 
        ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
            .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
            .StartNow()                            // запуск сразу после начала выполнения
            .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                .WithIntervalInSeconds(15)          // через 15 секунд
                .RepeatForever())                   // бесконечное повторение
            .Build();                               // создаем триггер
 
        await scheduler.ScheduleJob(job, trigger);        // начинаем выполнение работы
    }
}