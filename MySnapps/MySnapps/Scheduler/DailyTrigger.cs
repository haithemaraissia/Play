using System;
using System.Threading.Tasks;

namespace MySnapps.Scheduler
{
    public class DailyTrigger
    {
        readonly TimeSpan _triggerHour;

        public DailyTrigger(int hour, int minute = 0, int second = 0)
        {
            _triggerHour = new TimeSpan(hour, minute, second);
            InitiateAsync();
        }

        async void InitiateAsync()
        {
            while (true)
            {
                var triggerTime = DateTime.Today + _triggerHour - DateTime.Now;
                if (triggerTime < TimeSpan.Zero)
                    triggerTime = triggerTime.Add(new TimeSpan(24, 0, 0));
                await Task.Delay(triggerTime);
                OnTimeTriggered?.Invoke();
            }
        }

        public event Action OnTimeTriggered;
    }
}
