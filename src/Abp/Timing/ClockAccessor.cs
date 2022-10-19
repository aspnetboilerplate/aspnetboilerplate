using System;

namespace Abp.Timing
{
    public class ClockAccessor : IClockAccessor
    {
        private IClockProvider _provider;

        public DateTime Now => Provider.Now;
        public DateTimeKind Kind => Provider.Kind;
        public bool SupportsMultipleTimezone => Provider.SupportsMultipleTimezone;

        public DateTime Normalize(DateTime dateTime)
        {
            return Provider.Normalize(dateTime);
        }
        
        public IClockProvider Provider
        {
            get => _provider;
            set => _provider = value ?? throw new ArgumentNullException(nameof(value), "Can not set Clock.Provider to null!");
        }

    }
}